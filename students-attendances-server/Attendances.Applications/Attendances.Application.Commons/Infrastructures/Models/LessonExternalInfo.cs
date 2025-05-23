using Attendances.Domain.University.Entities.Lessons;
using AutoMapper;

namespace Attendances.Application.Commons.Infrastructures.Models;

public class LessonExternalInfo
{
    public required long ExternalId { get; set; }
    public required long AttendanceId { get; set; }

    public required string Description { get; set; }
    public required long StartTime { get; set; }
    public required long Duration { get; set; }
    
    public required long CourseId { get; set; }
    public required long GroupId { get; set; }

    public List<LessonAttendancesInfo> Attendances { get; set; } = new();
}

public class LessonAttendancesInfo
{
    public required long ExternalId { get; set; }
    public required long StudentId { get; set; }
    
    public required string Remarks { get; set; }
    public required string Acronym { get; set; }
    public required string Description { get; set; }
}

public class LessonExternalInfoProfile : Profile
{
    public LessonExternalInfoProfile()
    {
        CreateMap<LessonExternalInfo, LessonInfo>()
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => 
                ConvertMoscowUnixToUtc(src.StartTime)))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src =>
                ConvertMoscowUnixToUtc(src.StartTime + src.Duration)))
            .ForMember(dest => dest.Attendances, opt => opt.Ignore());
    }
    private static DateTime ConvertMoscowUnixToUtc(long moscowUnixTime)
    {
        DateTimeOffset moscowTime = DateTimeOffset.FromUnixTimeSeconds(moscowUnixTime);
        DateTimeOffset utcTime = moscowTime.AddHours(3);
        return utcTime.UtcDateTime;
    }
}
public class LessonAttendancesInfoProfile : Profile
{
    public LessonAttendancesInfoProfile() => CreateMap<LessonAttendancesInfo, AttendanceInfo>();
}