// ReSharper disable MemberCanBePrivate.Global
namespace Attendances.Domain.University.Settings;

public class TokenOptions
{
    public const int AccessExpiresDefault = 1000, RefreshExpiresDefault = 20000;
    
    public string AccessSecret { get; set; } = Guid.Empty.ToString();
    public string RefreshSecret { get; set; } = Guid.Empty.ToString();

    public long AccessExpires { get; set; } = AccessExpiresDefault;
    public long RefreshExpires { get; set; } = RefreshExpiresDefault;
}