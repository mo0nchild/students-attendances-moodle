using Attendances.Application.Authorization;
using Attendances.Application.Tokens;
using Attendances.Domain.University.Settings;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Attendances.Shared.Security.Configurations;

public static class SecretsConfigurations
{
    private static readonly string TokenSection = "Tokens";
    
    internal static async Task<IServiceCollection> AddSecretsConfiguration(this IServiceCollection servicesCollection, 
            IConfiguration configuration)
    {
        await servicesCollection.AddTokensServices(configuration);
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        
        var tokenSecrets = await GetTokenSecrets(factory.CreateLogger("Program"));
        servicesCollection.AddSingleton<TokenSecretsSettings>(_ => new TokenSecretsSettings()
        {
            Secrets = tokenSecrets
        });
        return servicesCollection;
    }

    private static Task<TokenOptions> GetTokenSecrets(ILogger logger)
    {
        var tokenConfiguration = new ConfigurationBuilder()
            .AddJsonFile("tokenSecrets.json")
            .Build();
        var defaultSecrets = tokenConfiguration.GetSection(TokenSection).Get<TokenOptions>();
        if (defaultSecrets == null) throw new Exception($"Token Secret configuration file is missing or invalid");
        return Task.FromResult(defaultSecrets);
    }
}