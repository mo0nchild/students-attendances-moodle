using Attendances.Domain.Core.MessageBus;
using Newtonsoft.Json;

namespace Attendances.Domain.Messages.EventMessages;

public class GroupCreatedEvent : EventMessageBase
{
    [JsonProperty("course_id")]
    public required long CourseId { get; set; }
    
    [JsonProperty("name")]
    public required string GroupName { get; set; }
    
    [JsonProperty("description")]
    public required string Description { get; set; }
}

public class GroupUpdatedEvent : EventMessageBase
{
    [JsonProperty("course_id")]
    public required long CourseId { get; set; }
    
    [JsonProperty("name")]
    public required string GroupName { get; set; }
    
    [JsonProperty("description")]
    public required string Description { get; set; }
}

public class GroupDeletedEvent : EventMessageBase
{
    [JsonProperty("course_id")]
    public required long CourseId { get; set; }
}

public class GroupMemberAddedEvent : EventMessageBase
{
    [JsonProperty("course_id")]
    public required long CourseId { get; set; }
    
    [JsonProperty("user_id")]
    public required long UserId { get; set; }
}

public class GroupMemberRemovedEvent : EventMessageBase
{
    [JsonProperty("course_id")]
    public required long CourseId { get; set; }
    
    [JsonProperty("user_id")]
    public required long UserId { get; set; }
}