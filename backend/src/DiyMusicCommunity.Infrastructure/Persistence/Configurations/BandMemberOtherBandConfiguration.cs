using DiyMusicCommunity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiyMusicCommunity.Infrastructure.Persistence.Configurations;

public sealed class BandMemberOtherBandConfiguration : IEntityTypeConfiguration<BandMemberOtherBand>
{
    public void Configure(EntityTypeBuilder<BandMemberOtherBand> builder)
    {
        builder.HasKey(b => b.Id);

        builder.HasIndex(b => new { b.BandMemberId, b.OtherBandId }).IsUnique();
        builder.HasIndex(b => b.OtherBandId);
    }
}
