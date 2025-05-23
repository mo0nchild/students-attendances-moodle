using Attendances.Shared.Security.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Attendances.Shared.Security;

public static class Bootstrapper
{
    public static async Task<IServiceCollection> AddSecurityServices(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        await serviceCollection.AddIdentityServices(configuration);
        return serviceCollection;
    }
    
    public static async Task<IServiceCollection> AddSecretService(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        return await serviceCollection.AddSecretsConfiguration(configuration);
    }
    
    public static WebApplication UseSecurity(this WebApplication application)
    {
        application.UseAuthentication();
        application.UseAuthorization();
        return application;
    }
}