using Attendances.Domain.University.Entities.Courses;
using AutoMapper;
using Newtonsoft.Json;

namespace Attendances.Application.Commons.Infrastructures.Models;

public class CourseExternalInfo
{
    [JsonProperty("id")]
    public required long ExternalId { get; set; }
    
    [JsonProperty("shortname")]
    public required string ShortName { get; set; }
    
    [JsonProperty("fullname")]
    public required string FullName { get; set; }
    
    [JsonProperty("format")]
    public required string Format { get; set; }
    
    [JsonProperty("startdate")]
    public required long StartDate { get; set; }
    
    [JsonProperty("enddate")]
    public required long EndDate { get; set; }
    
    [JsonIgnore]
    public List<AttendanceModuleInfo> AttendanceModules { get; set; } = new();
}

public class CourseExternalInfoProfile : Profile
{
    public CourseExternalInfoProfile()
    {
        CreateMap<CourseExternalInfo, CourseInfo>()
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src =>
                DateTimeOffset.FromUnixTimeSeconds(src.StartDate).UtcDateTime))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src =>
                DateTimeOffset.FromUnixTimeSeconds(src.EndDate).UtcDateTime));
    }
} 