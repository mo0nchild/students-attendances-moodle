namespace Attendances.System.WebApi.Settings;

public class MoodleEventSettings
{
    public required string ExchangeName { get; set; } = "moodle_events";
    public required string ConsumerPath { get; set; } = "basic_consumer";
    
    public required List<string> IgnoringEvents { get; set; } = new();
}