using Attendances.Application.Commons.Infrastructures.Models;
using Newtonsoft.Json;

namespace Attendances.RestWrapper.MoodleApi.Models.University;

internal class MoodleCourseSearchingInfo
{
    [JsonProperty("courses")]
    public required List<CourseExternalInfo> Courses { get; set; }
}

internal class MoodleUserSearchingInfo
{
    [JsonProperty("users")]
    public required List<UserExternalInfo> Users { get; set; }
}