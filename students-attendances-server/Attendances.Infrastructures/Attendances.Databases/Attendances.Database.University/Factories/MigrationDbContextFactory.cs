using Attendances.Database.Settings.Factories;
using Attendances.Database.University.Contexts;

namespace Attendances.Database.University.Factories;

public class MigrationDbContextFactory : MigrationDbContextFactoryBase<UniversityDbContext>
{
    public MigrationDbContextFactory() : base("UniversityDatabase")
    {
    }
    public override UniversityDbContext CreateDbContext(string[] args) => new(GetDbContextOptions());
}