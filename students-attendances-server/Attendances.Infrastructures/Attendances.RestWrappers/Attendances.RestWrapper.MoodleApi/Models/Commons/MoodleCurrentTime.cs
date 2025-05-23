using Newtonsoft.Json;

namespace Attendances.RestWrapper.MoodleApi.Models.Commons;

internal class MoodleCurrentTime
{
    [JsonProperty("timestamp")]
    public required long Timestamp { get; set; }
    
    [JsonProperty("datetime")]
    public required string DateTime { get; set; }
    
    [JsonProperty("timezone")]
    public required string Timezone { get; set; }
}