using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Attendances.Shared.Commons.Configurations;

public record CorsSettings(string AllowedOrigins);

public static class CorsConfiguration
{
    internal static Task<IServiceCollection> AddAppCors(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddCors();
        return Task.FromResult(serviceCollection);
    }
    internal static void UseAppCors(this WebApplication application)
    {
        var corsSettings = application.Configuration.GetSection(nameof(CorsSettings)).Get<CorsSettings>();
        
        var origins = corsSettings?.AllowedOrigins.Split(',', ';').Select(x => x.Trim())
            .Where(x => !string.IsNullOrEmpty(x)).ToArray();

        application.UseCors(policy =>
        {
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.AllowCredentials();
            if (origins is { Length: > 0 }) policy.WithOrigins(origins);
            else policy.SetIsOriginAllowed(origin => true);

            policy.WithExposedHeaders("Content-Disposition");
        });
    }
}