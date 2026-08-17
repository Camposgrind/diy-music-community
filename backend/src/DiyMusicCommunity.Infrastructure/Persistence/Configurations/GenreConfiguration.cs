using DiyMusicCommunity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiyMusicCommunity.Infrastructure.Persistence.Configurations;

public sealed class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id).ValueGeneratedNever();

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(g => g.Name).IsUnique();

        builder.HasData(
            new { Id = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), Name = "Grindcore" },
            new { Id = new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"), Name = "Crust" },
            new { Id = new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), Name = "D-Beat" },
            new { Id = new Guid("d4e5f6a7-b8c9-0123-def0-234567890123"), Name = "Powerviolence" },
            new { Id = new Guid("e5f6a7b8-c9d0-1234-ef01-345678901234"), Name = "Punk" },
            new { Id = new Guid("f6a7b8c9-d0e1-2345-f012-456789012345"), Name = "Noise" },
            new { Id = new Guid("a7b8c9d0-e1f2-3456-0123-567890123456"), Name = "Goregrind" },
            new { Id = new Guid("b8c9d0e1-f2a3-4567-1234-678901234567"), Name = "Gorenoise" },
            new { Id = new Guid("c9d0e1f2-a3b4-5678-2345-789012345678"), Name = "Death Metal" },
            new { Id = new Guid("d0e1f2a3-b4c5-6789-3456-890123456789"), Name = "Death-Grind" }
        );
    }
}
