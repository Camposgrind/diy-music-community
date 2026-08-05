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
    }
}
