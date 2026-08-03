using DiyMusicCommunity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiyMusicCommunity.Infrastructure.Persistence.Configurations;

public sealed class ModerationActionConfiguration : IEntityTypeConfiguration<ModerationAction>
{
    public void Configure(EntityTypeBuilder<ModerationAction> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.ActionType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Reason)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasIndex(m => m.ModeratorId);
        builder.HasIndex(m => m.TargetId);
    }
}
