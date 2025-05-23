using Attendances.Database.University.Configurations;
using Attendances.Domain.University.Entities.Courses;
using Attendances.Domain.University.Entities.Lessons;
using Attendances.Domain.University.Entities.Users;
using Attendances.Domain.University.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Attendances.Database.University.Contexts;

public class UniversityDbContext(DbContextOptions<UniversityDbContext> options) : DbContext(options), IUniversityRepository
{
    public virtual DbSet<AccountInfo> Accounts { get; set; }
    public virtual DbSet<UserInfo> Users { get; set; }
    public virtual DbSet<CourseInfo> Courses { get; set; }
    public virtual DbSet<GroupInfo> Groups { get; set; }
    public virtual DbSet<LessonInfo> Lessons { get; set; }
    
    public virtual DbSet<RfidMarkerInfo> RfidMarkers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder.UseLazyLoadingProxies());
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UsersConfiguration());
        modelBuilder.ApplyConfiguration(new AccountsConfiguration());
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new GroupsConfiguration());
        modelBuilder.ApplyConfiguration(new LessonsConfigurations());
        modelBuilder.ApplyConfiguration(new RfidMarkerConfiguration());
    }
    public Task<IDbContextTransaction> BeginTransactionAsync() => Database.BeginTransactionAsync();
    public IDbContextTransaction BeginTransaction() => Database.BeginTransaction();
}