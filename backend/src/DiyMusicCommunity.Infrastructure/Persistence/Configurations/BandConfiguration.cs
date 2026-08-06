using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiyMusicCommunity.Infrastructure.Persistence.Configurations;

public sealed class BandConfiguration : IEntityTypeConfiguration<Band>
{
    public void Configure(EntityTypeBuilder<Band> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Location)
            .HasMaxLength(200);

        builder.Property(b => b.Description)
            .HasMaxLength(2000);

        builder.Property(b => b.LogoImageUrl)
            .HasMaxLength(500);

        builder.Property(b => b.BandImageUrl)
            .HasMaxLength(500);

        builder.Property(b => b.MusicUrlPortal)
            .HasColumnType("nvarchar(max)");

        builder.Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(b => b.TrustStatus)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasIndex(b => b.Name);
        builder.HasIndex(b => b.Status);

        builder.HasData(
            new
            {
                Id = new Guid("ba4dc0de-beef-cafe-f00d-b00000000001"),
                Name = "Convulsions",
                Country = "Spain",
                Location = "El Ejido",
                GenreId = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                Status = Domain.Enums.BandStatus.Active,
                FormationYear = 2016,
                IsClaimed = true,
                MusicUrlPortal = "https://convulsionsgrindcore.bandcamp.com",
                TrustStatus = Domain.Enums.TrustStatus.Claimed,
                CreatedAt = new DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc)
            });
    }
}
