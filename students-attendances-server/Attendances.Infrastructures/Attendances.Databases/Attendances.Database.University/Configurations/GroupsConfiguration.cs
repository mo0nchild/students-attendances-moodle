using Attendances.Domain.University.Entities.Courses;
using Attendances.Domain.University.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Attendances.Database.University.Configurations;

internal class GroupsConfiguration : IEntityTypeConfiguration<GroupInfo>
{
    public void Configure(EntityTypeBuilder<GroupInfo> builder)
    {
        builder.ToTable(nameof(GroupInfo), "public");
        builder.HasIndex(item => item.Uuid).IsUnique();
        builder.HasIndex(item => item.ExternalId).IsUnique();
        
        builder.Property(item => item.GroupName).HasMaxLength(100).IsRequired();
        builder.Property(item => item.Description).IsRequired(false);
        
        builder.HasOne(item => item.Course).WithMany(item => item.Groups)
            .HasPrincipalKey(item => item.Uuid)
            .HasForeignKey("CourseUuid")
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(item => item.Students).WithMany(item => item.Groups)
            .UsingEntity(opt => opt
                    .HasOne(typeof(UserInfo))
                    .WithMany()
                    .HasForeignKey("StudentUuid")
                    .OnDelete(DeleteBehavior.Cascade), 
                opt => opt
                    .HasOne(typeof(GroupInfo))
                    .WithMany()
                    .HasForeignKey("GroupUuid")
                    .OnDelete(DeleteBehavior.Cascade),
                opt =>
                {
                    opt.HasKey("StudentUuid", "GroupUuid");
                    opt.ToTable("GroupStudents", "public");
                });
    }
}