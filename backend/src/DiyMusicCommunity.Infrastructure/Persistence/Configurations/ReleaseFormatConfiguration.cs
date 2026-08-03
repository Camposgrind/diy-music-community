using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiyMusicCommunity.Infrastructure.Persistence.Configurations;

public sealed class ReleaseFormatConfiguration : IEntityTypeConfiguration<ReleaseFormat>
{
    public void Configure(EntityTypeBuilder<ReleaseFormat> builder)
    {
        builder.HasKey(rf => rf.Id);

        builder.Property(rf => rf.Format)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasIndex(rf => new { rf.ReleaseId, rf.Format }).IsUnique();
    }
}
