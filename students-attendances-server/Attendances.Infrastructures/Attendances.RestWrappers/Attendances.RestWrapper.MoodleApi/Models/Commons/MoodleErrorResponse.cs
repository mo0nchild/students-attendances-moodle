using Newtonsoft.Json;

namespace Attendances.RestWrapper.MoodleApi.Models.Commons;

[JsonObject]
public class MoodleErrorResponse
{
    [JsonProperty("errorcode")]
    public required string ErrorCode { get; set; }
    
    [JsonProperty("exception")]
    public string Exception { get; set; } = String.Empty;
    
    [JsonProperty("message")]
    public string ErrorMessage { get; set; } = String.Empty;
}