namespace Attendances.RestWrapper.MoodleApi.Settings;

public class MoodleRestSettings
{
    public required string BaseUrl { get; set; }
    public required string Token { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
}