using Attendances.Database.Settings.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Attendances.Database.Settings.Factories;

public abstract class MigrationDbContextFactoryBase<TContext>(string connectionSection) : IDesignTimeDbContextFactory<TContext>
    where TContext : DbContext
{
    private static readonly string ConfigurationFilename = "appsettings.json";
    protected virtual DbContextOptions<TContext> GetDbContextOptions()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), ConfigurationFilename), false)
            .Build();
        var connectionString = configuration.GetSection(connectionSection)
            .Get<DbContextSettingsBase>()!.ConnectionString;
        
        return DbContextOptionsFactory<TContext>.Create(connectionString);
    }
    public abstract TContext CreateDbContext(string[] args);
}