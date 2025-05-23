using Attendances.Database.ExternalEvents.Contexts;
using Attendances.Database.Settings.Factories;

namespace Attendances.Database.ExternalEvents.Factories;

public class MigrationDbContextFactory : MigrationDbContextFactoryBase<ExternalEventDbContext>
{
    public MigrationDbContextFactory() : base("ExternalEventsDatabase")
    {
    }
    public override ExternalEventDbContext CreateDbContext(string[] args) => new(GetDbContextOptions());
}