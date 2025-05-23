using Attendances.Domain.University.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Attendances.Database.University.Configurations;

internal class AccountsConfiguration : IEntityTypeConfiguration<AccountInfo>
{
    public void Configure(EntityTypeBuilder<AccountInfo> builder)
    {
        builder.ToTable(nameof(AccountInfo), "public");
        builder.HasIndex(item => item.Uuid).IsUnique();
        
        builder.Property(item => item.Username).HasMaxLength(100).IsRequired();
        builder.Property(item => item.RefreshToken).IsRequired(false);
        
        builder.Property(item => item.Role).HasConversion(
            value => value.ToString(),
            value => (AccountRole)Enum.Parse(typeof(AccountRole), value));
        
        builder.HasOne(item => item.User).WithOne()
            .HasPrincipalKey<UserInfo>(item => item.Uuid)
            .HasForeignKey<AccountInfo>("UserUuid")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}