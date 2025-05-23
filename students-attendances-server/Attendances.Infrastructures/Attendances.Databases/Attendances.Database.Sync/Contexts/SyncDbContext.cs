using Attendances.Database.Sync.Configurations;
using Attendances.Domain.Sync.Entities;
using Attendances.Domain.Sync.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Attendances.Database.Sync.Contexts;

public class SyncDbContext(DbContextOptions<SyncDbContext> options) : DbContext(options), ISyncRepository
{
    public DbSet<LessonSyncItem> LessonSyncItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder.UseLazyLoadingProxies());
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new LessonSyncItemConfiguration());
    }
    public Task<IDbContextTransaction> BeginTransactionAsync() => Database.BeginTransactionAsync();
    public IDbContextTransaction BeginTransaction() => Database.BeginTransaction();
}