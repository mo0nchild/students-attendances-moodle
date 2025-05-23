using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Attendances.Domain.Core.MessageBus;

public class MessageBase
{
    [JsonProperty("event_type")]
    public string EventType { get; set; } = string.Empty;
    
    [JsonProperty("timestamp")]
    public long TimeStamp { get; set; } = default;
    
    [JsonProperty("payload")]
    public JToken Payload { get; set; } = default!;
    
    [JsonIgnore]
    public Guid EventUuid { get; set; }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class EventHandlerAttribute : Attribute
{
    public EventHandlerAttribute(string eventType) => EventType = eventType;
    public virtual string EventType { get; }
}

public abstract class EventMessageBase
{
    [JsonProperty("record_id")]
    public required long RecordId { get; set; }
}