using Attendances.Domain.Core.Models;
using Attendances.Domain.University.Entities.Courses;

namespace Attendances.Domain.University.Entities.Users;

public class UserInfo : BaseEntity
{
    public long ExternalId { get; set; } = default;
    
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public virtual List<RoleInfo> Roles { get; set; } = new();
    
    public virtual List<CourseInfo> CoursesAsStudent { get; set; } = new();
    public virtual List<CourseInfo> CoursesAsTeacher { get; set; } = new();
    public virtual List<GroupInfo> Groups { get; set; } = new();
}

public class RoleInfo
{
    public long RoleId { get; set; } = default;

    public string Name { get; set; } = string.Empty;
    
    public string ShortName { get; set; } = string.Empty;
}