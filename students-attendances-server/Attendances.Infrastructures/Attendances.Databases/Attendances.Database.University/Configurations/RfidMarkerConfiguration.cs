using Attendances.Domain.University.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Attendances.Database.University.Configurations;

internal class RfidMarkerConfiguration : IEntityTypeConfiguration<RfidMarkerInfo>
{
    public void Configure(EntityTypeBuilder<RfidMarkerInfo> builder)
    {
        builder.ToTable(nameof(RfidMarkerInfo), "public");
        builder.HasIndex(item => item.Uuid).IsUnique();
        
        builder.Property(item => item.Uuid).IsRequired();
        builder.Property(item => item.RfidValue).HasMaxLength(100).IsRequired();
    }
}