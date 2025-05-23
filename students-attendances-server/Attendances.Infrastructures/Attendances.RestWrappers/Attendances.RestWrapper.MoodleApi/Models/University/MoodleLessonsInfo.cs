using Attendances.Application.Commons.Infrastructures.Models;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Attendances.RestWrapper.MoodleApi.Models.University;

internal class MoodleLessonInfo
{
    [JsonProperty("id")]
    public required long ExternalId { get; set; }
    
    [JsonProperty("attendanceid")]
    public required long AttendanceId { get; set; }
    
    [JsonProperty("description")]
    public required string Description { get; set; }
    
    [JsonProperty("sessdate")]
    public required long StartTime { get; set; }
    
    [JsonProperty("duration")]
    public required long Duration { get; set; }
    
    [JsonProperty("courseid")]
    public required long CourseId { get; set; }
    
    [JsonProperty("groupid")]
    public required long GroupId { get; set; }

    [JsonProperty("statuses")]
    public required List<MoodleLessonStatusInfo> Statuses { get; set; }
    
    [JsonProperty("attendance_log")]
    public required List<MoodleLessonAttendanceInfo> Attendances  { get; set; }
    
    [JsonProperty("users")]
    public required JArray Users { get; set; }
}

internal class MoodleLessonStatusInfo
{
    [JsonProperty("id")]
    public required long ExternalId { get; set; }
    
    [JsonProperty("acronym")]
    public required string Acronym { get; set; }
    
    [JsonProperty("description")]
    public required string Description { get; set; }
}

internal class MoodleLessonAttendanceInfo
{
    [JsonProperty("id")]
    public required long ExternalId { get; set; }
    
    [JsonProperty("studentid")]
    public required long StudentId { get; set; }
    
    [JsonProperty("remarks")]
    public required string Remarks { get; set; }
    
    [JsonProperty("statusid")]
    public required string StatusId { get; set; }
}

public class MoodleLessonInfoProfile : Profile 
{
    public MoodleLessonInfoProfile()
    {
        CreateMap<MoodleLessonInfo, LessonExternalInfo>()
            .ForMember(dest => dest.Attendances, opt => opt.Ignore());
    }
}