using Attendances.Database.Settings.Factories;
using Attendances.Database.Sync.Contexts;

namespace Attendances.Database.Sync.Factories;

public class MigrationDbContextFactory : MigrationDbContextFactoryBase<SyncDbContext>
{
    public MigrationDbContextFactory() : base("SyncDatabase")
    {
    }
    public override SyncDbContext CreateDbContext(string[] args) => new(GetDbContextOptions());
}