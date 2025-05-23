using Attendances.Domain.Core.Models;
using Attendances.Domain.University.Entities.Users;

namespace Attendances.Domain.University.Entities.Courses;

public class GroupInfo : BaseEntity
{
    public long ExternalId { get; set; } = default;
    
    public string GroupName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public virtual CourseInfo Course { get; set; } = new();
    public virtual List<UserInfo> Students { get; set; } = new();
}