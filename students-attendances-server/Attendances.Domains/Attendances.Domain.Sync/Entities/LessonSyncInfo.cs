namespace Attendances.Domain.Sync.Entities;

public class LessonSyncInfo
{
    public long ExternalId { get; set; } = default;
    public string Description { get; set; } = string.Empty;
    
    public DateTime StartTime { get; set; } = default;
    public DateTime EndTime { get; set; } = default;
    
    public long AttendanceId { get; set; } = default;
    public long Version { get; set; } = default;
    
    public long TeacherId { get; set; } = default;
    
    public long CourseId { get; set; } = default;
    public long? GroupId { get; set; } = default;
    
    public virtual List<AttendanceSyncInfo> Attendances { get; set; } = new();
}

public class AttendanceSyncInfo
{
    public string Acronym { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
    
    public long StudentId { get; set; } = default;
}