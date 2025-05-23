using Attendances.Database.Settings.Factories;
using Attendances.Database.Settings.Helpers;
using Attendances.Database.Sync.Contexts;
using Attendances.Domain.Sync.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Attendances.Database.Sync;

public static class Bootstrapper
{
    private static readonly string DbSettingsSection = "SyncDatabase";
    
    public static async Task<IServiceCollection> AddSyncDatabase(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var settings = serviceCollection.Configure<SyncDbContextSettings>(configuration.GetSection(DbSettingsSection))
            .BuildServiceProvider()
            .GetRequiredService<IOptions<SyncDbContextSettings>>();
        serviceCollection.AddDbContextFactory<SyncDbContext>(options =>
        {
            DbContextOptionsFactory<SyncDbContext>.Configure(settings.Value.ConnectionString, true).Invoke(options);
            options.UseLoggerFactory(LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
            }));
        });
        await serviceCollection.AddDbContextFactoryWrapper<ISyncRepository, SyncDbContext>();
        
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var dbContextFactory = serviceProvider.GetService<IDbContextFactory<SyncDbContext>>()!;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.Database.MigrateAsync();
        return serviceCollection;
    }
}