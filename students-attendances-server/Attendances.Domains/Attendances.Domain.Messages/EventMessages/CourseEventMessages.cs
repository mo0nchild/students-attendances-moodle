using Attendances.Domain.Core.MessageBus;
using Newtonsoft.Json;

namespace Attendances.Domain.Messages.EventMessages;

public class CourseCreatedEvent : EventMessageBase
{
    [JsonProperty("course_id")]
    public required long CourseId { get; set; }
    
    [JsonProperty("fullname")]
    public required string Fullname { get; set; }
    
    [JsonProperty("shortname")]
    public required string Shortname { get; set; }
}

public class CourseUpdatedEvent : EventMessageBase
{
    [JsonProperty("course_id")]
    public required long CourseId { get; set; }
    
    [JsonProperty("fullname")]
    public required string Fullname { get; set; }
    
    [JsonProperty("shortname")]
    public required string Shortname { get; set; }
}

public class CourseDeletedEvent : EventMessageBase
{
    [JsonProperty("course_id")]
    public required long CourseId { get; set; }
}

public class UserRoleAssignedEvent : EventMessageBase
{
    [JsonProperty("user_id")]
    public required long UserId { get; set; }
    
    [JsonProperty("course_id")]
    public required long CourseId { get; set; }
    
    [JsonProperty("role")]
    public required string Archetype { get; set; }
    
    [JsonProperty("description")]
    public required string Description { get; set; }
    
    [JsonProperty("shortname")]
    public required string ShortName { get; set; }
}

public class UserRoleUnassignedEvent : EventMessageBase
{
    [JsonProperty("user_id")]
    public required long UserId { get; set; }
    
    [JsonProperty("course_id")]
    public required long CourseId { get; set; }
    
    [JsonProperty("role")]
    public required string Archetype { get; set; }
}