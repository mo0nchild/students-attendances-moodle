using Newtonsoft.Json;

namespace Attendances.Application.Authorization.Infrastructures.Models;

public class TeacherExternalInfo
{
    [JsonProperty("id")]
    public required long ExternalId { get; set; }
    
    [JsonProperty("username")]
    public required string Username { get; set; }
    
    [JsonProperty("email")]
    public required string Email { get; set; }
    
    [JsonProperty("firstname")]
    public required string FirstName { get; set; }
    
    [JsonProperty("lastname")]
    public required string LastName { get; set; }
}