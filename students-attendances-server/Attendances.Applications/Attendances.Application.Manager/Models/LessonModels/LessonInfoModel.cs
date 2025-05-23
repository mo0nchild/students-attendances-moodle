using Attendances.Application.Manager.Models.CommonModels;
using Attendances.Domain.University.Entities.Lessons;
using AutoMapper;

namespace Attendances.Application.Manager.Models.LessonModels;

public class LessonInfoModel
{
    public required long ExternalId { get; set; }
    public required DateTime CreatedTime { get; set; }
    public required DateTime ModifiedTime { get; set; }
    
    public required string Description { get; set; }
    
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public required long Version { get; set; }
    
    public required long AttendanceId { get; set; }
    
    public GroupInfoModel? GroupInfo { get; set; } = default;
    public List<AttendanceInfoModel> Attendances { get; set; } = new();
}

public class AttendanceInfoModel
{
    public required string Acronym { get; set; }
    public required string Description { get; set; }
    public required string Remarks { get; set; }
    
    public required long StudentId { get; set; }
}

public class LessonInfoModelProfile : Profile
{
    public LessonInfoModelProfile()
    {
        CreateMap<AttendanceInfo, AttendanceInfoModel>();
        CreateMap<LessonInfo, LessonInfoModel>();
    }
}