using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiyMusicCommunity.Infrastructure.Persistence.Configurations;

public sealed class ReleaseConfiguration : IEntityTypeConfiguration<Release>
{
    public void Configure(EntityTypeBuilder<Release> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(r => r.LabelText)
            .HasMaxLength(300);

        builder.Property(r => r.CoverImageUrl)
            .HasMaxLength(500);

        builder.Property(r => r.ReleaseType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasMany(r => r.Formats)
            .WithOne()
            .HasForeignKey(rf => rf.ReleaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.BandId);
    }
}
