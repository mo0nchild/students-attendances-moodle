using Newtonsoft.Json;

namespace Attendances.RestWrapper.MoodleApi.Models.University;


internal class MoodleModulesInfo
{
    [JsonProperty("id")]
    public required long ExternalId { get; set; }
    
    [JsonProperty("name")]
    public required string Name { get; set; }
    
    [JsonProperty("instance")]
    public required long Instance { get; set; }
    
    [JsonProperty("modname")]
    public required string ModuleName { get; set; }
    
    [JsonProperty("groupmode")]
    public required int GroupMode { get; set; }
}

internal class MoodleCourseModulesInfo
{
    [JsonProperty("modules")]
    public required List<MoodleModulesInfo> Modules { get; set; }
}
