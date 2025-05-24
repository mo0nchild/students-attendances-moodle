using Attendances.Domain.University.Entities.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Attendances.Database.University.Configurations;

internal class LessonsConfigurations : IEntityTypeConfiguration<LessonInfo>
{
    public void Configure(EntityTypeBuilder<LessonInfo> builder)
    {
        builder.ToTable(nameof(LessonInfo), "public");
        builder.HasIndex(item => item.Uuid).IsUnique();

        builder.Property(item => item.AttendanceId).IsRequired();
        builder.Property(item => item.Description).HasMaxLength(255).IsRequired();

        builder.Property(item => item.Attendances)
            .HasConversion(
                value => JsonConvert.SerializeObject(value),
                value => JsonConvert.DeserializeObject<List<AttendanceInfo>>(value)!
            );

        builder.HasOne(item => item.Course).WithMany(item => item.Lessons)
            .HasPrincipalKey(item => item.Uuid)
            .HasForeignKey("CourseUuid")
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(item => item.Group).WithMany()
            .HasPrincipalKey(item => item.Uuid)
            .HasForeignKey("GroupUuid")
            .OnDelete(DeleteBehavior.Cascade);
    }
}