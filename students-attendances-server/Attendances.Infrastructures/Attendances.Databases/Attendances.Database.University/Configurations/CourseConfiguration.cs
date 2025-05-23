using Attendances.Domain.University.Entities.Courses;
using Attendances.Domain.University.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Attendances.Database.University.Configurations;

internal class CourseConfiguration : IEntityTypeConfiguration<CourseInfo>
{
    public void Configure(EntityTypeBuilder<CourseInfo> builder)
    {
        builder.ToTable(nameof(CourseInfo), "public");
        builder.HasIndex(item => item.Uuid).IsUnique();
        builder.HasIndex(item => item.ExternalId).IsUnique();
        
        builder.Property(item => item.ShortName).HasMaxLength(50).IsRequired();
        builder.Property(item => item.FullName).HasMaxLength(255).IsRequired();
        builder.Property(item => item.Format).HasMaxLength(50).IsRequired(false);

        builder.Property(item => item.AttendanceModules)
            .HasConversion(
                value => JsonConvert.SerializeObject(value),
                value => JsonConvert.DeserializeObject<List<AttendanceModuleInfo>>(value)!
            );
        
        builder.HasMany(item => item.Students).WithMany(item => item.CoursesAsStudent)
            .UsingEntity(opt => opt
                    .HasOne(typeof(UserInfo))
                    .WithMany()
                    .HasForeignKey("StudentUuid")
                    .OnDelete(DeleteBehavior.Cascade), 
                opt => opt
                    .HasOne(typeof(CourseInfo))
                    .WithMany()
                    .HasForeignKey("CourseUuid")
                    .OnDelete(DeleteBehavior.Cascade),
                opt =>
                {
                    opt.HasKey("StudentUuid", "CourseUuid");
                    opt.ToTable("CourseStudents", "public");
                });
        builder.HasMany(item => item.Teachers).WithMany(item => item.CoursesAsTeacher)
            .UsingEntity(opt => opt
                    .HasOne(typeof(UserInfo))
                    .WithMany()
                    .HasForeignKey("TeacherUuid")
                    .OnDelete(DeleteBehavior.Cascade), 
                opt => opt
                    .HasOne(typeof(CourseInfo))
                    .WithMany()
                    .HasForeignKey("CourseUuid")
                    .OnDelete(DeleteBehavior.Cascade),
                opt =>
                {
                    opt.HasKey("TeacherUuid", "CourseUuid");
                    opt.ToTable("CourseTeachers", "public");
                });
    }
}