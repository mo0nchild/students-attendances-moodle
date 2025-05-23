namespace Attendances.Application.Tokens.Models;

public class TokensModel
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}