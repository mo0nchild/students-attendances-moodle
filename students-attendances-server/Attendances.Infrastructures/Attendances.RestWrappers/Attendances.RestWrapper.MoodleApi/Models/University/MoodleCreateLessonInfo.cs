using Newtonsoft.Json;

namespace Attendances.RestWrapper.MoodleApi.Models.University;

internal class MoodleCreateLessonInfo
{
    [JsonProperty("sessionid")]
    public required long ExternalId { get; set; }
}