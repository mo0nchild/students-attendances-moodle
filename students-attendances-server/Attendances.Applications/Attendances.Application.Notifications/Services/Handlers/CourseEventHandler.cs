using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Notifications.Commons;
using Attendances.Application.Notifications.Commons.Helpers;
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

internal class CourseEventHandler : EventHandlerBase
{
    private readonly RepositoryFactoryInterface<IUniversityRepository> _repositoryFactory;
    private readonly IMapper _mapper;
    private readonly IExternalProvider _externalProvider;
    private readonly CourseFetchHelper _courseFetchHelper;

    public CourseEventHandler(RepositoryFactoryInterface<IUniversityRepository> repositoryFactory,
        IMapper mapper,
        IExternalProvider externalProvider, 
        ILogger<CourseEventHandler> logger)
    {
        _repositoryFactory = repositoryFactory;
        _mapper = mapper;
        _externalProvider = externalProvider;
        _courseFetchHelper = new CourseFetchHelper(externalProvider, mapper, logger);
        Logger = logger;
    }
    private ILogger<CourseEventHandler> Logger { get; }

    [EventHandler("course_created")]
    [EventHandler("module_created")]
    public async Task HandleModuleCreated(ModuleCreatedEvent payload, CancellationToken _)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var courseInfo = await _externalProvider.GetCourseByIdAsync(payload.CourseId);
        if (courseInfo == null) return;
        
        await using var transaction = await dbContext.BeginTransactionAsync();
        try
        {
            var courseRecord = await dbContext.Courses.FirstOrDefaultAsync(item => item.ExternalId == payload.CourseId);
            if (courseRecord != null)
            {
                courseRecord.AttendanceModules = courseInfo.AttendanceModules;
                courseRecord.ModifiedTime = DateTime.UtcNow;

                dbContext.Courses.UpdateRange(courseRecord);
            }
            else await _courseFetchHelper.FetchCourseRelationsAsync(dbContext, courseInfo);

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (ProcessException error)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [EventHandler("module_updated")]
    public async Task HandleModuleUpdated(ModuleDeletedEvent payload, CancellationToken cancellationToken)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        
        var courseInfo = await _externalProvider.GetCourseByIdAsync(payload.CourseId);
        if (courseInfo == null) return;
        
        await using var transaction = await dbContext.BeginTransactionAsync();
        try
        {
            var courseRecord = await dbContext.Courses.FirstOrDefaultAsync(item => item.ExternalId == payload.CourseId);
            if (courseRecord != null)
            {
                courseRecord.AttendanceModules = courseInfo.AttendanceModules;
                courseRecord.ModifiedTime = DateTime.UtcNow;

                dbContext.Courses.UpdateRange(courseRecord);
            }
            else await _courseFetchHelper.FetchCourseRelationsAsync(dbContext, courseInfo);

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (ProcessException error)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    [EventHandler("module_deleted")]
    public async Task HandleModuleDeleted(ModuleDeletedEvent payload, CancellationToken cancellationToken)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        
        var courseRecord = await dbContext.Courses.FirstOrDefaultAsync(item => item.ExternalId == payload.CourseId);
        await using var transaction = await dbContext.BeginTransactionAsync();
        try
        {
            var courseInfo = await _externalProvider.GetCourseByIdAsync(payload.CourseId);
            if (courseInfo != null)
            {
                if (courseRecord != null)
                {
                    courseRecord.AttendanceModules = courseInfo.AttendanceModules;
                    courseRecord.ModifiedTime = DateTime.UtcNow;
                    dbContext.Courses.UpdateRange(courseRecord);
                    
                    var lessonsInfo = await _externalProvider.GetLessonsByCourseIdAsync(payload.CourseId);
                    var lessonIds = lessonsInfo.Select(lesson => lesson.ExternalId);

                    var removedLesson = dbContext.Lessons
                        .Include(item => item.Course)
                        .Where(item => !lessonIds.Contains(item.ExternalId) && item.Course.ExternalId == payload.CourseId);
                    dbContext.Lessons.RemoveRange(removedLesson); 
                }
                else await _courseFetchHelper.FetchCourseRelationsAsync(dbContext, courseInfo);
            }
            else if (courseRecord != null) dbContext.Courses.RemoveRange(courseRecord);

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (ProcessException error)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    [EventHandler("course_updated")]
    public async Task HandleCourseUpdated(CourseUpdatedEvent payload, CancellationToken stoppingToken)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var courseRecord = await dbContext.Courses.FirstOrDefaultAsync(item => item.ExternalId == payload.CourseId);
        
        await using var transaction = await dbContext.BeginTransactionAsync();
        try
        {
            var courseInfo = await _externalProvider.GetCourseByIdAsync(payload.CourseId);
            if (courseInfo != null)
            {
                if (courseRecord != null)
                {
                    var mappedInfo = _mapper.Map<CourseInfo>(courseInfo);
                    mappedInfo.Uuid = courseRecord.Uuid;
                    mappedInfo.ModifiedTime = DateTime.UtcNow;
                    mappedInfo.CreatedTime = courseRecord.CreatedTime;
                
                    dbContext.Courses.UpdateRange(mappedInfo);
                }
                else await _courseFetchHelper.FetchCourseRelationsAsync(dbContext, courseInfo);
            }
            else dbContext.Courses.RemoveRange(dbContext.Courses.Where(item => item.ExternalId == payload.CourseId));
            
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (ProcessException error)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    [EventHandler("course_deleted")]
    public async Task HandleCourseDeleted(CourseDeletedEvent payload, CancellationToken stoppingToken)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var courseInfo = await _externalProvider.GetCourseByIdAsync(payload.CourseId);
        if (courseInfo == null)
        {
            dbContext.Courses.RemoveRange(dbContext.Courses.Where(item => item.ExternalId == payload.CourseId));
            await dbContext.SaveChangesAsync();
        }
    }
}