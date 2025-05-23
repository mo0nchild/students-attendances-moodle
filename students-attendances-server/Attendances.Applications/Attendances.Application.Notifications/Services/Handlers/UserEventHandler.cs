using Attendances.Application.Commons.Infrastructures.Models;
using Attendances.Application.Notifications.Commons;
using Attendances.Application.Notifications.Infrastructures.Interfaces;
using Attendances.Domain.Core.Factories;
using Attendances.Domain.Core.MessageBus;
using Attendances.Domain.Messages.EventMessages;
using Attendances.Domain.University.Entities.Courses;
using Attendances.Domain.University.Entities.Users;
using Attendances.Domain.University.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// ReSharper disable MethodSupportsCancellation

namespace Attendances.Application.Notifications.Services.Handlers;

internal class UserEventHandler : EventHandlerBase
{
    private readonly RepositoryFactoryInterface<IUniversityRepository> _repositoryFactory;
    private readonly IMapper _mapper;
    private readonly IExternalProvider _externalProvider;

    public UserEventHandler(RepositoryFactoryInterface<IUniversityRepository> repositoryFactory,
        IMapper mapper,
        IExternalProvider externalProvider, 
        ILogger<UserEventHandler> logger)
    {
        Logger = logger;
        _repositoryFactory = repositoryFactory;
        _mapper = mapper;
        _externalProvider = externalProvider;
    }
    private ILogger<UserEventHandler> Logger { get; }
    
    [EventHandler("user_role_assigned")]
    public async Task UserRoleAssignedHandler(UserRoleAssignedEvent payload, CancellationToken _)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var courseRecord = await dbContext.Courses.FirstOrDefaultAsync(item => item.ExternalId == payload.CourseId);
        if (courseRecord == null) return;
        
        var userRecord = await dbContext.Users.FirstOrDefaultAsync(item => item.ExternalId == payload.UserId);
        var userInfo = (await _externalProvider.GetStudentsByCourseIdAsync(payload.CourseId))
            .FirstOrDefault(item => item.ExternalId == payload.UserId);
        if (userInfo != null)
        {
            if (userRecord == null)
            {
                var mappedUser = _mapper.Map<UserInfo>(userInfo);
                await dbContext.Users.AddRangeAsync(mappedUser);
                await dbContext.SaveChangesAsync();
                
                AssignRoles(mappedUser, userInfo.Roles, courseRecord);
            }
            else AssignRoles(userRecord, userInfo.Roles, courseRecord);
        } 
        await dbContext.SaveChangesAsync();
    }

    [EventHandler("user_role_unassigned")]
    public async Task UserRoleUnassigned(UserRoleAssignedEvent payload, CancellationToken _)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        
        var courseRecord = await dbContext.Courses.FirstOrDefaultAsync(item => item.ExternalId == payload.CourseId);
        if (courseRecord == null) return;
        
        var userRecord = await dbContext.Users.Include(item => item.CoursesAsStudent)
            .Include(item => item.CoursesAsTeacher)
            .FirstOrDefaultAsync(item => item.ExternalId == payload.UserId);
        
        var userInfo = (await _externalProvider.GetStudentsByCourseIdAsync(payload.CourseId))
            .FirstOrDefault(item => item.ExternalId == payload.UserId);
        
        if (userInfo != null)
        {
            var userRoles = userInfo.Roles.Select(item => item.Name).ToList();
            if (userRecord != null)
            {
                if (!userRoles.Any(r => r.Contains("teacher"))) userRecord.CoursesAsTeacher.Remove(courseRecord);
                if (!userRoles.Any(r => r.Contains("student"))) userRecord.CoursesAsStudent.Remove(courseRecord);

                if (!userRecord.CoursesAsTeacher.Any() && !userRecord.CoursesAsStudent.Any())
                {
                    dbContext.RfidMarkers.RemoveRange(dbContext.RfidMarkers.Where(item =>
                        item.UserId == userRecord.ExternalId));
                    dbContext.Users.Remove(userRecord);
                }
            } 
        } 
        else if (userRecord != null)
        {
            dbContext.RfidMarkers.RemoveRange(dbContext.RfidMarkers.Where(item =>
                item.UserId == userRecord.ExternalId));
            dbContext.Users.Remove(userRecord);
        }
        await dbContext.SaveChangesAsync();
    }

    [EventHandler("user_updated")]
    public async Task UserUpdatedHandler(UserUpdatedEvent payload, CancellationToken _)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var userInfo = await _externalProvider.GetUserByIdAsync(payload.RecordId);
        if (userInfo == null) return;
        
        var userRecord = await dbContext.Users.FirstOrDefaultAsync(item => item.ExternalId == payload.RecordId);
        if (userRecord != null)
        {
            var mappedUser = _mapper.Map<UserInfo>(userInfo);
            mappedUser.Uuid = userRecord.Uuid;
            mappedUser.CreatedTime = userRecord.CreatedTime;
            mappedUser.ModifiedTime = DateTime.UtcNow;
            
            dbContext.Users.UpdateRange(mappedUser);
            await dbContext.SaveChangesAsync();
        }
    }

    [EventHandler("user_password_updated")]
    public async Task UserPasswordUpdatedHandler(UserPasswordUpdatedEvent payload, CancellationToken _)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        
        var accountRecord = await dbContext.Accounts
            .Include(item => item.User)
            .FirstOrDefaultAsync(item => item.User != null && item.User.ExternalId == payload.RecordId);

        if (accountRecord != null)
        {
            dbContext.Accounts.RemoveRange(accountRecord);
            await dbContext.SaveChangesAsync();
        }
    }
    
    private void AssignRoles(UserInfo userRecord, IEnumerable<ExternalRoleInfo> roles, CourseInfo course)
    {
        var externalRoleInfos = roles.ToList();
        if (externalRoleInfos.Any(r => r.ShortName.Contains("student"))) userRecord.CoursesAsStudent.Add(course);
        if (externalRoleInfos.Any(r => r.ShortName.Contains("teacher"))) userRecord.CoursesAsTeacher.Add(course);
    }
}