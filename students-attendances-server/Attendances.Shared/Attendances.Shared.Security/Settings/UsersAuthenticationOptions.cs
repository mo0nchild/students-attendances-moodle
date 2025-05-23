using Microsoft.AspNetCore.Authentication;

namespace Attendances.Shared.Security.Settings;

public class UsersAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "MyAuthenticationScheme";
}