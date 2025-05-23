using System.Diagnostics.CodeAnalysis;
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
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

// ReSharper disable MethodSupportsCancellation

namespace Attendances.Application.Sync.Services;

internal class LessonSyncService : ILessonSyncProcessor, ILessonSyncManager
{
    private readonly RepositoryFactoryInterface<ISyncRepository> _syncFactory;
    private readonly IMapper _mapper;
    private readonly LessonSyncEventHandler _syncEventHandler;
    
    private readonly IMapper _mapperToDomain = new MapperConfiguration(builder =>
    {
        builder.CreateMap<LessonSyncInfo, LessonInfo>()
            .ForMember(dest => dest.Attendances, opt => opt.Ignore());
    }).CreateMapper();
    
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private SyncProcessingType _currentSyncType = default;

    public LessonSyncService(RepositoryFactoryInterface<ISyncRepository> syncFactory,
        LessonSyncEventHandler syncEventHandler,
        IMapper mapper,
        ILogger<LessonSyncService> logger)
    {
        _syncFactory = syncFactory;
        _syncEventHandler = syncEventHandler;
        _mapper = mapper;
        Logger = logger;
    }
    private ILogger<LessonSyncService> Logger { get; }
    
    public async Task<Guid> AddEventAsync(LessonSyncInfo lessonInfo, SyncSource source, SyncAction action)
    {
        using var dbContext = await _syncFactory.CreateRepositoryAsync();
        await _semaphore.WaitAsync();
        try
        {
            var newLessonSync = new LessonSyncItem()
            {
                ExternalId = action != SyncAction.Create ? lessonInfo.ExternalId : null,
                Source = source,
                Action = action,
                EntityVersion = lessonInfo.Version + 1,
                LessonInfo = lessonInfo,
            };
            await dbContext.LessonSyncItems.AddRangeAsync(newLessonSync);
            await dbContext.SaveChangesAsync();
            Logger.LogInformation($"Lesson event registered [{action}] to database, source - [{source}]");
            
            return newLessonSync.Uuid;
        }
        finally { _semaphore.Release(); }
    }

    public async Task<LessonSyncItem?> GetSyncInfoAsync(Guid eventUuid)
    {
        using var dbContext = await _syncFactory.CreateRepositoryAsync();
        return await dbContext.LessonSyncItems.FirstOrDefaultAsync(item => item.Uuid == eventUuid);
    }

    public async Task ProcessAsync(SyncProcessingType syncType, CancellationToken cancellationToken)
    {
        using var dbContext = await _syncFactory.CreateRepositoryAsync();
        _currentSyncType = syncType;
        
        var pendingEvents = await dbContext.LessonSyncItems.Where(item => item.Status != SyncStatus.FullSync)
            .ToListAsync(cancellationToken);
        
        if (!pendingEvents.Any()) return;
        
        var processingEvent = syncType switch
        {
            SyncProcessingType.Global => pendingEvents.Where(item => item.Status == SyncStatus.Processing
                                                                     || item.Status == SyncStatus.LocalSaved),
            SyncProcessingType.Local => pendingEvents.Where(item => item.Status == SyncStatus.Processing),
            _ => throw new ArgumentOutOfRangeException(nameof(syncType), syncType, null)
        };
        var lessonSyncItems = processingEvent.ToList();
        foreach (var @event in lessonSyncItems.OrderBy(item => item.QueuedAt))
        {
            Logger.LogInformation($"Processing synchronize {@event.Action}, source - [{@event.Source}], " +
                                  $"status - [{@event.Status}], id - [{@event.ExternalId ?? 0}]");
            
            async Task NewIdCallback(long newLessonId)
            {
                if (@event.Action == SyncAction.Create)
                {
                    @event.ExternalId = newLessonId;
                    dbContext.LessonSyncItems.Update(@event);
                }
                else
                {
                    var relatedEvents = await dbContext.LessonSyncItems.Where(item => item.ExternalId == @event.ExternalId)
                        .ToListAsync();
                    foreach (var evt in relatedEvents) evt.ExternalId = newLessonId;

                    dbContext.LessonSyncItems.UpdateRange(relatedEvents);
                }
                await dbContext.SaveChangesAsync();
            }
            try
            {
                switch (@event.Action)
                {
                    case SyncAction.Create:
                        await _syncEventHandler.CreateLessonHandler(@event, syncType, async id => await NewIdCallback(id));
                        break;
                    case SyncAction.Update:
                        await _syncEventHandler.UpdateLessonHandler(@event, syncType, async id => await NewIdCallback(id));
                        break;
                    case SyncAction.Delete:
                        await _syncEventHandler.DeleteLessonHandler(@event, syncType);
                        break;
                }
                @event.Status = syncType == SyncProcessingType.Global ? SyncStatus.FullSync : SyncStatus.LocalSaved;
                
                dbContext.LessonSyncItems.Update(@event);
                await dbContext.SaveChangesAsync();
            }
            catch (ProcessException error)
            {
                if (@event.Status == SyncStatus.LocalSaved)
                {
                    await _syncEventHandler.RollbackAsync(@event, dbContext);
                    Logger.LogWarning(error, "Rollback applied for failed event.");
                    throw;
                }
                Logger.LogWarning(error, $"Error processing lesson sync event: {error.Message}");
                @event.Status = SyncStatus.Failed;
                @event.ErrorMessage = error.Message;
            
                dbContext.LessonSyncItems.Update(@event);
                await dbContext.SaveChangesAsync();
                throw;
            }
            Logger.LogInformation($"Processing synchronize complete {@event.Action}");
        }
    }
}