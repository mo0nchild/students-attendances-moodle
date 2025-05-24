using Attendances.Domain.Core.Models;

namespace Attendances.Domain.Sync.Entities;

public class LessonSyncItem : BaseEntity
{
    public SyncSource Source { get; set; } = SyncSource.Local;
    public SyncAction Action { get; set; } = SyncAction.Create;
    
    public DateTime QueuedAt { get; set; } = DateTime.UtcNow;
    public SyncStatus Status { get; set; } = SyncStatus.Processing;
    public long? ExternalId { get; set; } = default;
    public Guid? InternalUuid { get; set; } = default;
    public long EntityVersion { get; set; } = default;
    
    public string? ErrorMessage { get; set; } = null;
    
    public virtual LessonSyncInfo LessonInfo { get; set; } = new();
}

public enum SyncAction
{
    Create,
    Update,
    Delete
}

public enum SyncStatus
{
    Processing,
    LocalSaved,
    FullSync,
    Failed
}

public enum SyncSource
{
    Local,
    External,
}