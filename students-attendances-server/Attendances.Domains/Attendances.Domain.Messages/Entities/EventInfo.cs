using Attendances.Domain.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Attendances.Domain.Messages.Entities;

public class EventInfo : BaseEntity
{
    public string EventType { get; set; } = string.Empty;
    public long TimeStamp { get; set; } = default;
    
    public bool IsHandled { get; set; } = false;
    
    public required JToken Payload { get; set; }
}