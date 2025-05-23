using Attendances.Domain.Sync.Entities;

namespace Attendances.Application.Sync.Interfaces;

public interface ILessonSyncManager
{
    Task<Guid> AddEventAsync(LessonSyncInfo lessonInfo, SyncSource source, SyncAction action);
    Task<LessonSyncItem?> GetSyncInfoAsync(Guid eventUuid);
}