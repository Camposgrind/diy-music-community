using DiyMusicCommunity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiyMusicCommunity.Infrastructure.Persistence.Configurations;

public sealed class BandMemberConfiguration : IEntityTypeConfiguration<BandMember>
{
    public void Configure(EntityTypeBuilder<BandMember> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Instrument)
            .HasMaxLength(200);

        builder.HasMany(m => m.OtherBands)
            .WithOne()
            .HasForeignKey(ob => ob.BandMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(m => m.BandId);
    }
}
