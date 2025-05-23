using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Notifications.Commons;
using Attendances.Application.Notifications.Infrastructures.Interfaces;
using Attendances.Application.Sync.Interfaces;
using Attendances.Domain.Core.Factories;
using Attendances.Domain.Core.MessageBus;
using Attendances.Domain.Messages.EventMessages;
using Attendances.Domain.Sync.Entities;
using Attendances.Domain.University.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Attendances.Application.Notifications.Services.Handlers;

internal class LessonEventHandler : EventHandlerBase
{
    private readonly RepositoryFactoryInterface<IUniversityRepository> _repositoryFactory;
    private readonly IMapper _mapper;
    private readonly ILessonSyncManager _lessonSyncManager;

    private readonly IExternalProvider _externalProvider;

    public LessonEventHandler(RepositoryFactoryInterface<IUniversityRepository> repositoryFactory,
        IMapper mapper,
        ILessonSyncManager lessonSyncManager, 
        IExternalProvider externalProvider,
        ILogger<LessonEventHandler> logger)
    {
        Logger = logger;
        _externalProvider = externalProvider;
        _repositoryFactory = repositoryFactory;
        _mapper = mapper;
        _lessonSyncManager = lessonSyncManager;
    }
    private ILogger<LessonEventHandler> Logger { get; }

    [EventHandler("lesson_updated")]
    [EventHandler("attendance_taken")]
    public async Task LessonUpdatedHandler(LessonUpdatedEvent payload, CancellationToken _)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var lessonRecord = await dbContext.Lessons
            .FirstOrDefaultAsync(item => item.ExternalId == payload.RecordId, cancellationToken: _);
        if (lessonRecord == null) return;

        var lessonsList = await _externalProvider.GetLessonsByCourseIdAsync(payload.CourseId);
        var lessonInfo = lessonsList.FirstOrDefault(item => item.ExternalId == payload.RecordId);
        if (lessonInfo == null)
        {
            throw new ProcessException($"lesson_updated: could not find lesson with id {payload.RecordId}");
        }
        var studentsMap = await dbContext.Users.Include(item => item.CoursesAsStudent)
            .Where(item => item.CoursesAsStudent.Any(it => it.ExternalId == payload.CourseId))
            .ToDictionaryAsync(item => item.ExternalId, item => item, cancellationToken: _);

        var attendances = lessonInfo.Attendances.Where(item => studentsMap.ContainsKey(item.StudentId))
            .Select(item => new AttendanceSyncInfo()
            {
                Acronym = item.Acronym,
                Remarks = item.Remarks,
                StudentId = item.StudentId
            }).ToList();
        
        await _lessonSyncManager.AddEventAsync(new LessonSyncInfo()
        {
            ExternalId = lessonInfo.ExternalId,
            Description = lessonInfo.Description,
            StartTime = ConvertUnixToUtc(lessonInfo.StartTime),
            EndTime = ConvertUnixToUtc(lessonInfo.StartTime + lessonInfo.Duration),
            AttendanceId = lessonInfo.AttendanceId,
            GroupId = lessonInfo.GroupId == 0 ? null : lessonInfo.GroupId,
            CourseId = lessonInfo.CourseId,
            Version = lessonRecord.Version,
            Attendances = attendances,
        }, SyncSource.External, SyncAction.Update);
    }
    
    [EventHandler("lesson_added")]
    public async Task LessonAddedHandler(LessonAddedEvent payload, CancellationToken _)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        
        var lessonInfo = (await _externalProvider.GetLessonsByCourseIdAsync(payload.CourseId))
            .FirstOrDefault(item => item.ExternalId == payload.RecordId);

        if (lessonInfo == null) return;
        await _lessonSyncManager.AddEventAsync(new LessonSyncInfo()
        {
            ExternalId = lessonInfo.ExternalId,
            Description = lessonInfo.Description,
            StartTime = ConvertUnixToUtc(lessonInfo.StartTime),
            EndTime = ConvertUnixToUtc(lessonInfo.StartTime + lessonInfo.Duration),
            AttendanceId = lessonInfo.AttendanceId,
            GroupId = lessonInfo.GroupId == 0 ? null : lessonInfo.GroupId,
            CourseId = lessonInfo.CourseId,
        }, SyncSource.External, SyncAction.Create);
    }
    
    [EventHandler("lesson_deleted")]
    public async Task LessonDeletedHandler(LessonDeletedEvent payload, CancellationToken _)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        
        var lessonInfo = (await _externalProvider.GetLessonsByCourseIdAsync(payload.CourseId))
            .FirstOrDefault(item => item.ExternalId == payload.RecordId);
        
        if (lessonInfo != null) return;
        await _lessonSyncManager.AddEventAsync(new LessonSyncInfo()
        {
            ExternalId = payload.RecordId,
        }, SyncSource.External, SyncAction.Delete);
    }
    
    private static DateTime ConvertUnixToUtc(long moscowUnixTime)
    {
        var utcTime = DateTimeOffset.FromUnixTimeSeconds(moscowUnixTime).AddHours(3);
        return utcTime.UtcDateTime;
    }
}