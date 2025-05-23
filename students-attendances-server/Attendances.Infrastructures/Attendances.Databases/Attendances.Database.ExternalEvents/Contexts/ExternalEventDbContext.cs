using Attendances.Database.ExternalEvents.Configurations;
using Attendances.Domain.Messages.Entities;
using Attendances.Domain.Messages.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Attendances.Database.ExternalEvents.Contexts;

public class ExternalEventDbContext(DbContextOptions<ExternalEventDbContext> options) : DbContext(options), 
    IEventRepository
{
    public DbSet<EventInfo> EventInfos { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder.UseLazyLoadingProxies());
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new EventInfoConfiguration());
    }
    public Task<IDbContextTransaction> BeginTransactionAsync() => Database.BeginTransactionAsync();
    public IDbContextTransaction BeginTransaction() => Database.BeginTransaction();
}