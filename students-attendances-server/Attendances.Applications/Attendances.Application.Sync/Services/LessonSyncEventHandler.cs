using System.Security.Cryptography;
using System.Text;
using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Sync.Infrastructures.Interfaces;
using Attendances.Application.Sync.Infrastructures.Models;
using Attendances.Application.Sync.Interfaces;
using Attendances.Domain.Core.Factories;
using Attendances.Domain.Sync.Entities;
using Attendances.Domain.Sync.Repositories;
using Attendances.Domain.University.Entities.Lessons;
using Attendances.Domain.University.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Attendances.Application.Sync.Services;

internal class LessonSyncEventHandler
{
    private readonly RepositoryFactoryInterface<IUniversityRepository> _universityRepository;
    private readonly IMapper _mapper;
    private readonly ILessonExternal _lessonExternal;

    private readonly IMapper _mapperToDomain = new MapperConfiguration(builder =>
    {
        builder.CreateMap<LessonSyncInfo, LessonInfo>();
        builder.CreateMap<AttendanceSyncInfo, AttendanceInfo>();
    }).CreateMapper();
    
    public LessonSyncEventHandler(RepositoryFactoryInterface<IUniversityRepository> universityRepository, 
        IMapper mapper,
        ILessonExternal lessonExternal,
        ILogger<LessonSyncEventHandler> logger)
    {
        _universityRepository = universityRepository;
        _mapper = mapper;
        Logger = logger;
        _lessonExternal = lessonExternal;
    }
    private ILogger<LessonSyncEventHandler> Logger { get; }
    
    public async Task CreateLessonHandler(LessonSyncItem lessonEvent, SyncProcessingType syncType,
        Func<long, Task>? newIdCallback = null)
    {
        using var dbContext = await _universityRepository.CreateRepositoryAsync();
        var courseInfo = await dbContext.Courses
            .FirstOrDefaultAsync(item => item.ExternalId == lessonEvent.LessonInfo.CourseId);
        if (courseInfo == null)
        {
            Logger.LogWarning($"Course {lessonEvent.LessonInfo.CourseId} not found");
            return;
        }
        await using var transaction = await dbContext.BeginTransactionAsync();
        try
        {
            var mappedLesson = _mapperToDomain.Map<LessonInfo>(lessonEvent.LessonInfo);
            mappedLesson.Course = courseInfo;
            if (lessonEvent.LessonInfo.GroupId.HasValue && lessonEvent.LessonInfo.GroupId != 0)
            {
                var groupInfo = await dbContext.Groups
                    .FirstOrDefaultAsync(item => item.ExternalId == lessonEvent.LessonInfo.GroupId);
                if (groupInfo == null)
                {
                    Logger.LogWarning($"Group {lessonEvent.LessonInfo.GroupId} not found");
                    throw new ProcessException($"Group {lessonEvent.LessonInfo.GroupId} not found");
                }
                mappedLesson.Group = groupInfo;
            }

            if (lessonEvent.Source == SyncSource.Local && syncType == SyncProcessingType.Global)
            {
                var request = _mapper.Map<CreateLessonExternal>(lessonEvent.LessonInfo);
                mappedLesson.ExternalId = await _lessonExternal.CreateLessonAsync(request);
                await (newIdCallback?.Invoke(mappedLesson.ExternalId) ?? Task.CompletedTask);
            }
            if (lessonEvent.Status == SyncStatus.Processing)
            {
                await dbContext.Lessons.AddRangeAsync(mappedLesson);
                await dbContext.SaveChangesAsync();
                
                if (lessonEvent is { Source: SyncSource.External })
                {
                    await (newIdCallback?.Invoke(lessonEvent.LessonInfo.ExternalId) ?? Task.CompletedTask);
                }
            } 
            await transaction.CommitAsync();
            
        }
        catch (ProcessException error)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task UpdateLessonHandler(LessonSyncItem lessonEvent, SyncProcessingType syncType,
        Func<long, Task>? newIdCallback = null)
    {
        using var dbContext = await _universityRepository.CreateRepositoryAsync();
        
        var lessonRecord = await dbContext.Lessons.FirstOrDefaultAsync(item => item.ExternalId == lessonEvent.ExternalId);
        if (lessonRecord == null)
        {
            Logger.LogWarning($"Lesson {lessonEvent.ExternalId} not found");
            throw new ProcessException($"Lesson {lessonEvent.ExternalId} not found");
        }
        await using var transaction = await dbContext.BeginTransactionAsync();
        try
        {
            if (lessonEvent.EntityVersion > lessonRecord.Version)
            {
                var mappedLesson = _mapperToDomain.Map<LessonInfo>(lessonEvent.LessonInfo);
                if (lessonEvent.Source == SyncSource.Local && syncType == SyncProcessingType.Global)
                {
                    var request = _mapper.Map<UpdateLessonExternal>(lessonEvent.LessonInfo);
                    
                    lessonRecord.ExternalId = await _lessonExternal.UpdateLessonAsync(request);
                    await (newIdCallback?.Invoke(lessonRecord.ExternalId) ?? Task.CompletedTask);
                }
                if (lessonEvent.Status == SyncStatus.Processing)
                {
                    lessonRecord.ModifiedTime = DateTime.UtcNow;
                    lessonRecord.Version = lessonEvent.EntityVersion;
                    
                    lessonRecord.Description = mappedLesson.Description;
                    lessonRecord.StartTime = mappedLesson.StartTime;
                    lessonRecord.EndTime = mappedLesson.EndTime;
                    lessonRecord.Attendances = mappedLesson.Attendances;
                    
                    dbContext.Lessons.Update(lessonRecord);
                    await dbContext.SaveChangesAsync();
                }
                Logger.LogInformation("Lesson sync update complete");
            }
            else
            {
                Logger.LogWarning($"Event version {lessonEvent.EntityVersion} " +
                                    $"is smaller than record version {lessonRecord.Version}");
            }
            await transaction.CommitAsync();
        }
        catch (ProcessException error)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task DeleteLessonHandler(LessonSyncItem lessonEvent, SyncProcessingType syncType)
    {
        using var dbContext = await _universityRepository.CreateRepositoryAsync();
        
        var lessonRecord = await dbContext.Lessons.FirstOrDefaultAsync(item => item.ExternalId == lessonEvent.ExternalId);
        if (lessonRecord == null)
        {
            Logger.LogWarning($"Lesson {lessonEvent.LessonInfo.CourseId} not found");
            return;
        }

        if (lessonEvent.Source == SyncSource.Local && syncType == SyncProcessingType.Global)
        {
            var request = _mapper.Map<DeleteLessonExternal>(lessonEvent.LessonInfo);
            await _lessonExternal.DeleteLessonAsync(request);
        }
        if (lessonEvent.Status == SyncStatus.Processing)
        {
            dbContext.Lessons.Remove(lessonRecord);
            await dbContext.SaveChangesAsync();
        }
    }
    public async Task RollbackAsync(LessonSyncItem failedItem, ISyncRepository syncRepository)
    {
        var dbContext = await _universityRepository.CreateRepositoryAsync();
        Logger.LogWarning($"Start rollback");

        // Пытаемся найти предыдущую версию
        var rollbackTarget = await syncRepository.LessonSyncItems
            .Where(item => item.ExternalId == failedItem.ExternalId && item.EntityVersion < failedItem.EntityVersion
                                                                    && item.Status == SyncStatus.FullSync)
            .OrderByDescending(item => item.EntityVersion)
            .FirstOrDefaultAsync();

        if (rollbackTarget == null)
        {
            // Нет прошлой версии: удаляем или логируем ошибку
            var lessonRecord = await dbContext.Lessons.FirstOrDefaultAsync(x => x.ExternalId == failedItem.ExternalId);
            if (lessonRecord != null)
            {
                dbContext.Lessons.Remove(lessonRecord);
                await dbContext.SaveChangesAsync();
            }
            Logger.LogWarning($"Rollback failed: no previous version");
            failedItem.Status = SyncStatus.Failed;
            failedItem.ErrorMessage = "Rollback failed: no previous version.";
            return;
        }
        // Откат к предыдущей версии
        
        var restored = _mapperToDomain.Map<LessonInfo>(rollbackTarget.LessonInfo);
        dbContext.Lessons.Update(restored);
        await dbContext.SaveChangesAsync();
        Logger.LogWarning($"Rollback complete");
    }
    
    private string ComputeHash(object obj)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)));
        return Convert.ToHexString(hashBytes);
    }
}