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
        builder.Property(r => r.Id).ValueGeneratedNever();

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

        builder.HasMany(r => r.Tracks)
            .WithOne()
            .HasForeignKey(t => t.ReleaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(r => r.Tracks).HasField("_tracks");

        builder.HasIndex(r => r.BandId);

        builder.HasData(
            new
            {
                Id = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"),
                BandId = new Guid("ba4dc0de-beef-cafe-f00d-b00000000001"),
                Title = "Grindcore Not War",
                LabelText = "Hecatombe Records, Regurgitated Semen Records",
                ReleaseType = Domain.Enums.ReleaseType.Album,
                ReleaseDate = new DateOnly(2023, 3, 10),
                Year = (int?)2023,
                CoverImageUrl = (string?)null
            });
    }
}
