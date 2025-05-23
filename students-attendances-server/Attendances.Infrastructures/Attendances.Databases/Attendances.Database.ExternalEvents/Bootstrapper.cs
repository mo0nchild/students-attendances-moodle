using Attendances.Database.ExternalEvents.Contexts;
using Attendances.Database.Settings.Factories;
using Attendances.Database.Settings.Helpers;
using Attendances.Domain.Messages.Repositories;
using Attendances.Domain.University.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Attendances.Database.ExternalEvents;

public static class Bootstrapper
{
    private static readonly string DbSettingsSection = "ExternalEventsDatabase";
    
    public static async Task<IServiceCollection> AddExternalEventsDatabase(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var settings = serviceCollection.Configure<ExternalEventDbContextSettings>(configuration.GetSection(DbSettingsSection))
            .BuildServiceProvider()
            .GetRequiredService<IOptions<ExternalEventDbContextSettings>>();
        serviceCollection.AddDbContextFactory<ExternalEventDbContext>(options =>
        {
            DbContextOptionsFactory<ExternalEventDbContext>.Configure(settings.Value.ConnectionString, true).Invoke(options);
            options.UseLoggerFactory(LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
            }));
        });
        await serviceCollection.AddDbContextFactoryWrapper<IEventRepository, ExternalEventDbContext>();
        
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var dbContextFactory = serviceProvider.GetService<IDbContextFactory<ExternalEventDbContext>>()!;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.Database.MigrateAsync();
        return serviceCollection;
    }
}