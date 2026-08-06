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

        builder.HasData(
            new { Id = new Guid("f04f0000-cafe-beef-dead-000000000001"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Format = Domain.Enums.Format.Vinyl12 },
            new { Id = new Guid("f04f0000-cafe-beef-dead-000000000002"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Format = Domain.Enums.Format.CD },
            new { Id = new Guid("f04f0000-cafe-beef-dead-000000000003"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Format = Domain.Enums.Format.Cassette });
    }
}
