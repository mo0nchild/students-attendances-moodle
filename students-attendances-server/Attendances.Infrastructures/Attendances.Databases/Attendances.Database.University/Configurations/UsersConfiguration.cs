using Attendances.Domain.University.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Attendances.Database.University.Configurations;

internal class UsersConfiguration : IEntityTypeConfiguration<UserInfo>
{
    public void Configure(EntityTypeBuilder<UserInfo> builder)
    {
        builder.ToTable(nameof(UserInfo), "public");
        builder.HasIndex(item => item.Uuid).IsUnique();
        builder.HasIndex(item => item.ExternalId).IsUnique();
        builder.HasIndex(item => item.Username).IsUnique();
        
        builder.Property(item => item.Email).HasMaxLength(100).IsRequired();
        builder.Property(item => item.Username).HasMaxLength(100).IsRequired();
        
        builder.Property(item => item.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(item => item.LastName).HasMaxLength(100).IsRequired();
        
        builder.Property(item => item.City).HasMaxLength(100).IsRequired(false);
        builder.Property(item => item.Country).HasMaxLength(100).IsRequired(false);
        builder.Property(item => item.Description).IsRequired(false);
        builder.Property(item => item.ImageUrl).IsRequired(false);
        
        builder.Property(item => item.Roles)
            .HasConversion(
                value => JsonConvert.SerializeObject(value),
                value => JsonConvert.DeserializeObject<List<RoleInfo>>(value)!
            )
            .IsRequired(false);
    }
}