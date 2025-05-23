using Attendances.Domain.Sync.Entities;

namespace Attendances.Application.Sync.Interfaces;

public interface ILessonSyncProcessor
{
    Task ProcessAsync(SyncProcessingType syncType, CancellationToken cancellationToken);
}

public enum SyncProcessingType
{
    Global,
    Local,
}