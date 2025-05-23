using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Attendances.Database.Settings.Factories;

public static class DbContextOptionsFactory<TContext> where TContext : DbContext
{
    public static DbContextOptions<TContext> Create(string connectionString, bool detailedLogging = false)
    {
        var builder = new DbContextOptionsBuilder<TContext>();
        Configure(connectionString).Invoke(builder);
        
        return builder.Options;
    }
    public static Action<DbContextOptionsBuilder> Configure(string connectionString, bool detailedLogging = false)
    {
        return (DbContextOptionsBuilder builder) =>
        {
            builder.UseNpgsql(connectionString, options =>
            {
                options.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds)
                    .MigrationsHistoryTable("_Migrations");
            }); 
            if (detailedLogging) builder.EnableSensitiveDataLogging();
            builder.UseLazyLoadingProxies(true);
        };
    }
}