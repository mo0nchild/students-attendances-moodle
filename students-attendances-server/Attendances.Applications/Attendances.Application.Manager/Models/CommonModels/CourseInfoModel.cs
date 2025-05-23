using Attendances.Domain.University.Entities.Courses;
using AutoMapper;

namespace Attendances.Application.Manager.Models.CommonModels;

public class CourseInfoModel
{
    public required long ExternalId { get; set; }
    
    public required DateTime CreatedTime { get; set; }
    public required DateTime ModifiedTime { get; set; }
    
    public required string ShortName { get; set; }
    public required string FullName { get; set; }
    public required string Format { get; set; }
    
    public required DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; } = default;
    
    public List<UserInfoModel> Teachers { get; set; } = new();
    public List<GroupInfoModel> Groups { get; set; } = new();
    
    public List<AttendanceModuleInfo> AttendanceModules { get; set; } = new();
}

public class CourseInfoModelProfile : Profile
{
    public CourseInfoModelProfile()
    {
        CreateMap<CourseInfo, CourseInfoModel>();
    }
}