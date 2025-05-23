using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Notifications.Commons;
using Attendances.Application.Notifications.Infrastructures.Interfaces;
using Attendances.Domain.Core.Factories;
using Attendances.Domain.Core.MessageBus;
using Attendances.Domain.Messages.EventMessages;
using Attendances.Domain.University.Entities.Courses;
using Attendances.Domain.University.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// ReSharper disable MethodSupportsCancellation

namespace Attendances.Application.Notifications.Services.Handlers;

internal class GroupEventHandler : EventHandlerBase
{
    private readonly RepositoryFactoryInterface<IUniversityRepository> _repositoryFactory;
    private readonly IMapper _mapper;
    private readonly IExternalProvider _externalProvider;

    public GroupEventHandler(RepositoryFactoryInterface<IUniversityRepository> repositoryFactory,
        IMapper mapper,
        IExternalProvider externalProvider,
        ILogger<GroupEventHandler> logger)
    {
        _repositoryFactory = repositoryFactory;
        _mapper = mapper;
        _externalProvider = externalProvider;
        Logger = logger;
    }
    private ILogger<GroupEventHandler> Logger { get; }

    [EventHandler("group_created")]
    public async Task GroupCreatedHandler(GroupCreatedEvent payload, CancellationToken _)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var courseRecord = await dbContext.Courses.FirstOrDefaultAsync(item => item.ExternalId == payload.CourseId);
        if (courseRecord == null) return;
        
        var groupInfo = (await _externalProvider.GetGroupsByCourseIdAsync(payload.CourseId))
            .FirstOrDefault(item => item.ExternalId == payload.RecordId);

        if (groupInfo != null)
        {
            var mappedGroup = _mapper.Map<GroupInfo>(groupInfo);
            mappedGroup.Course = courseRecord;
            
            await dbContext.Groups.AddRangeAsync(mappedGroup);
            await dbContext.SaveChangesAsync();
        }
    }
    
    [EventHandler("group_updated")]
    public async Task GroupUpdatedHandler(GroupUpdatedEvent payload, CancellationToken _)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        
        var groupInfo = (await _externalProvider.GetGroupsByCourseIdAsync(payload.CourseId))
            .FirstOrDefault(item => item.ExternalId == payload.RecordId);
        if (groupInfo == null)
        {
            throw new ProcessException($"group_updated: could not find group with id {payload.RecordId}");
        }
        var groupRecord = await dbContext.Groups.FirstOrDefaultAsync(item => item.ExternalId == payload.CourseId);
        if (groupRecord != null)
        {
            var mappedGroup = _mapper.Map<GroupInfo>(groupInfo);
            mappedGroup.Uuid = groupRecord.Uuid;
            mappedGroup.ModifiedTime = DateTime.UtcNow;
            mappedGroup.CreatedTime = groupRecord.CreatedTime;
            
            dbContext.Groups.UpdateRange(mappedGroup);
            await dbContext.SaveChangesAsync();
        }
    }
    
    [EventHandler("group_deleted")]
    public async Task GroupDeletedHandler(GroupDeletedEvent payload, CancellationToken _)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        
        var groupInfo = (await _externalProvider.GetGroupsByCourseIdAsync(payload.CourseId))
            .FirstOrDefault(item => item.ExternalId == payload.RecordId);

        if (groupInfo != null) return;
        dbContext.Groups.RemoveRange(dbContext.Groups.Where(item => item.ExternalId == payload.RecordId));
        await dbContext.SaveChangesAsync();
    }

    [EventHandler("group_member_added")]
    public async Task GroupMemberAddedHandler(GroupMemberAddedEvent payload, CancellationToken _)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        if (!dbContext.Courses.Any(item => item.ExternalId == payload.CourseId)) return;
        
        var groupInfo = (await _externalProvider.GetGroupsByCourseIdAsync(payload.CourseId))
            .FirstOrDefault(item => item.ExternalId == payload.RecordId);
        var userInfo = (await _externalProvider.GetStudentsByCourseIdAsync(payload.CourseId))
            .FirstOrDefault(item => item.ExternalId == payload.UserId);

        if (groupInfo == null) throw new ProcessException("group_member_added: could not find group");
        if (userInfo == null) throw new ProcessException("group_member_added: could not find student");
        
        if (groupInfo.MemberList.MemberIds.Contains(userInfo.ExternalId))
        {
            var groupRecord = await dbContext.Groups.FirstAsync(item => item.ExternalId == payload.RecordId);
            var userRecord = await dbContext.Users.FirstAsync(item => item.ExternalId == payload.UserId);
            groupRecord.Students.Add(userRecord);
            
            await dbContext.SaveChangesAsync();
        }
    }
    
    [EventHandler("group_member_removed")]
    public async Task GroupMemberRemovedHandler(GroupMemberRemovedEvent payload, CancellationToken _)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        
        var groupInfo = (await _externalProvider.GetGroupsByCourseIdAsync(payload.CourseId))
            .FirstOrDefault(item => item.ExternalId == payload.RecordId);
        
        if (groupInfo == null) throw new ProcessException("group_member_removed: could not find group");
        if (!groupInfo.MemberList.MemberIds.Contains(payload.UserId))
        {
            var groupRecord = await dbContext.Groups.FirstAsync(item => item.ExternalId == payload.RecordId);
            var userRecord = await dbContext.Users.FirstOrDefaultAsync(student => student.ExternalId == payload.UserId);

            if (userRecord != null)
            {
                groupRecord.Students.Remove(userRecord);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}