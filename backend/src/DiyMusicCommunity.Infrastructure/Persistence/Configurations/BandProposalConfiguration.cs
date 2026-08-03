using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiyMusicCommunity.Infrastructure.Persistence.Configurations;

public sealed class BandProposalConfiguration : IEntityTypeConfiguration<BandProposal>
{
    public void Configure(EntityTypeBuilder<BandProposal> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Location)
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.SourceUrl)
            .HasMaxLength(500);

        builder.Property(p => p.RejectionReason)
            .HasMaxLength(1000);

        builder.Property(p => p.ReviewStatus)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasIndex(p => p.ReviewStatus);
        builder.HasIndex(p => p.SubmittedByUserId);
    }
}
