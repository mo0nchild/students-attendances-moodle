using Attendances.Domain.Core.MessageBus;
using Newtonsoft.Json;

namespace Attendances.Domain.Messages.EventMessages;

public class LessonAddedEvent : EventMessageBase
{
    [JsonProperty("datetime")]
    public required string StartTime { get; set; }
    
    [JsonProperty("description")]
    public required string Description { get; set; }
    
    [JsonProperty("course_id")]
    public required long CourseId { get; set; }
    
    [JsonProperty("group_id")]
    public required long GroupId { get; set; }
    
    [JsonProperty("attendance_id")]
    public required string AttendanceId { get; set; }
}

public class LessonUpdatedEvent : EventMessageBase
{
    [JsonProperty("datetime")]
    public required string StartTime { get; set; }
    
    [JsonProperty("description")]
    public required string Description { get; set; }
    
    [JsonProperty("course_id")]
    public required long CourseId { get; set; }
    
    [JsonProperty("group_id")]
    public required long GroupId { get; set; }
    
    [JsonProperty("attendance_id")]
    public required string AttendanceId { get; set; }
}

public class LessonDeletedEvent : EventMessageBase
{
    [JsonProperty("course_id")]
    public required long CourseId { get; set; }
}

public class AttendanceTakenEvent : EventMessageBase
{
    [JsonProperty("datetime")]
    public required string StartTime { get; set; }
    
    [JsonProperty("description")]
    public required string Description { get; set; }
}

public class ModuleCreatedEvent : EventMessageBase
{
    [JsonProperty("course_id")]
    public required long CourseId { get; set; }
    
    [JsonProperty("attendance_name")]
    public required string ModuleName { get; set; }
}

public class ModuleUpdatedEvent : EventMessageBase
{
    [JsonProperty("course_id")]
    public required long CourseId { get; set; }
    
    [JsonProperty("attendance_name")]
    public required string ModuleName { get; set; }
}

public class ModuleDeletedEvent : EventMessageBase
{
    [JsonProperty("course_id")]
    public required long CourseId { get; set; }
}