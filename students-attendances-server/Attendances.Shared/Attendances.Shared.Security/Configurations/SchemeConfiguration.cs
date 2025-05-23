using Microsoft.AspNetCore.Authentication;
using Attendances.Shared.Security.Infrastructure;
using Attendances.Shared.Security.Settings;

namespace Attendances.Shared.Security.Configurations;

public static class SchemeConfiguration
{
    internal static AuthenticationBuilder AddUsersAuthentication(this AuthenticationBuilder builder, 
        Action<UsersAuthenticationOptions> configuration)
    {
        return builder.AddScheme<UsersAuthenticationOptions, UsersAuthenticationScheme>(
            UsersAuthenticationOptions.DefaultScheme, configuration);
    }
}