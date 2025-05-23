using Attendances.Domain.Core.MessageBus;
using Newtonsoft.Json;

namespace Attendances.Domain.Messages.EventMessages;

public class UserCreatedEvent : EventMessageBase
{
    [JsonProperty("username")]
    public required string Username { get; set; }
    
    [JsonProperty("firstname")]
    public required string Firstname { get; set; }
    
    [JsonProperty("lastname")]
    public required string Lastname { get; set; }
}

public class UserUpdatedEvent : EventMessageBase
{
    [JsonProperty("username")]
    public required string Username { get; set; }
    
    [JsonProperty("firstname")]
    public required string Firstname { get; set; }
    
    [JsonProperty("lastname")]
    public required string Lastname { get; set; }
}

public class UserDeletedEvent : EventMessageBase { }

public class UserPasswordUpdatedEvent : EventMessageBase { }