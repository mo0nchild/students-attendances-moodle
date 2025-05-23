using Attendances.Database.Settings.Factories;
using Attendances.Database.Settings.Helpers;
using Attendances.Database.University.Contexts;
using Attendances.Domain.University.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Attendances.Database.University;

public static class Bootstrapper
{
    private static readonly string DbSettingsSection = "UniversityDatabase";
    
    public static async Task<IServiceCollection> AddUniversityDatabase(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var settings = serviceCollection.Configure<UniversityDbContextSettings>(configuration.GetSection(DbSettingsSection))
            .BuildServiceProvider()
            .GetRequiredService<IOptions<UniversityDbContextSettings>>();
        serviceCollection.AddDbContextFactory<UniversityDbContext>(options =>
        {
            DbContextOptionsFactory<UniversityDbContext>.Configure(settings.Value.ConnectionString, true).Invoke(options);
            options.UseLoggerFactory(LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
            }));
        });
        await serviceCollection.AddDbContextFactoryWrapper<IUniversityRepository, UniversityDbContext>();
        
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var dbContextFactory = serviceProvider.GetService<IDbContextFactory<UniversityDbContext>>()!;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.Database.MigrateAsync();
        return serviceCollection;
    }
}