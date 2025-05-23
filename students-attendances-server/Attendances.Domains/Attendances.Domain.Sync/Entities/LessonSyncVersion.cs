using Attendances.Domain.Core.Models;

namespace Attendances.Domain.Sync.Entities;

public class LessonSyncVersion : BaseEntity
{
    public long LessonId { get; set; } = default;

    public long LocalVersion { get; set; } = default;
    public long RemoteVersion { get; set; } = default;
    
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}