using Attendances.Domain.Sync.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Attendances.Database.Sync.Configurations;

internal class LessonSyncItemConfiguration : IEntityTypeConfiguration<LessonSyncItem>
{
    public void Configure(EntityTypeBuilder<LessonSyncItem> builder)
    {
        builder.ToTable(nameof(LessonSyncItem), "public");
        builder.HasIndex(item => item.Uuid).IsUnique();

        builder.Property(item => item.LessonInfo)
            .HasConversion(
                value => JsonConvert.SerializeObject(value),
                value => JsonConvert.DeserializeObject<LessonSyncInfo>(value)!
            );
        
        builder.Property(item => item.Action)
            .HasConversion(
                value => value.ToString(),
                value => (SyncAction)Enum.Parse(typeof(SyncAction), value)
            );
        builder.Property(item => item.Status)
            .HasConversion(
                value => value.ToString(),
                value => (SyncStatus)Enum.Parse(typeof(SyncStatus), value)
            );
        builder.Property(item => item.Source)
            .HasConversion(
                value => value.ToString(),
                value => (SyncSource)Enum.Parse(typeof(SyncSource), value)
            );
    }
}