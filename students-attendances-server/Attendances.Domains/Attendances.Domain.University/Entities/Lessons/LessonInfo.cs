using Attendances.Domain.Core.Models;
using Attendances.Domain.University.Entities.Courses;

namespace Attendances.Domain.University.Entities.Lessons;

public class LessonInfo : BaseEntity
{
    public long ExternalId { get; set; } = default;
    public string Description { get; set; } = string.Empty;
    
    public DateTime StartTime { get; set; } = default;
    public DateTime EndTime { get; set; } = default;
    public long Version { get; set; } = default;
    
    public long AttendanceId { get; set; } = default;

    public virtual CourseInfo Course { get; set; } = new();
    public virtual GroupInfo? Group { get; set; } = default;
    
    public virtual List<AttendanceInfo> Attendances { get; set; } = new();
}