using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiyMusicCommunity.Infrastructure.Persistence.Configurations;

public sealed class BandClaimConfiguration : IEntityTypeConfiguration<BandClaim>
{
    public void Configure(EntityTypeBuilder<BandClaim> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.Message)
            .HasMaxLength(1000);

        builder.Property(c => c.EvidenceUrl)
            .HasMaxLength(500);

        builder.Property(c => c.RejectionReason)
            .HasMaxLength(1000);

        builder.Property(c => c.ClaimType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => new { c.UserId, c.BandId });
    }
}
