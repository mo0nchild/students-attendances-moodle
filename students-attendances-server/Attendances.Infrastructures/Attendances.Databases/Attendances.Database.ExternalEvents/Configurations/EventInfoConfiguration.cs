using Attendances.Domain.Messages.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Attendances.Database.ExternalEvents.Configurations;

internal class EventInfoConfiguration : IEntityTypeConfiguration<EventInfo>
{
    public void Configure(EntityTypeBuilder<EventInfo> builder)
    {
        builder.ToTable(nameof(EventInfo), "public");
        builder.HasIndex(item => item.Uuid).IsUnique();
        
        builder.Property(item => item.EventType).HasMaxLength(100).IsRequired();
        builder.Property(item => item.TimeStamp).IsRequired();
        builder.Property(item => item.IsHandled).IsRequired();
        
        builder.Property(item => item.Payload)
            .HasConversion(
                value => value.ToString(Formatting.None),
                value => JToken.Parse(value)
            )
            .IsRequired(true);
    }
}