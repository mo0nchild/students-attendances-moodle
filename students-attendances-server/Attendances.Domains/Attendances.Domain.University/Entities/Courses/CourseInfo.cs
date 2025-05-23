using Attendances.Domain.Core.Models;
using Attendances.Domain.University.Entities.Lessons;
using Attendances.Domain.University.Entities.Users;

namespace Attendances.Domain.University.Entities.Courses;

public class CourseInfo : BaseEntity
{
    public long ExternalId { get; set; } = default;
    
    public string ShortName { get; set; } = string.Empty;
    
    public string FullName { get; set; } = string.Empty;
    
    public string Format { get; set; } = string.Empty;
    
    public DateTime StartDate { get; set; } = default;

    public DateTime? EndDate { get; set; } = default;
    
    public List<AttendanceModuleInfo> AttendanceModules { get; set; } = new();
    
    public virtual List<UserInfo> Students { get; set; } = new();
    public virtual List<UserInfo> Teachers { get; set; } = new();
    
    public virtual List<GroupInfo> Groups { get; set; } = new();
    
    public virtual List<LessonInfo> Lessons { get; set; } = new();
}


public class AttendanceModuleInfo
{
    public long ExternalId { get; set; } = default;
    public string Name { get; set; } = string.Empty;
    public GroupMode GroupMode { get; set; }
}

public enum GroupMode
{
    None = 0,
    Isolated,
    Visible
}