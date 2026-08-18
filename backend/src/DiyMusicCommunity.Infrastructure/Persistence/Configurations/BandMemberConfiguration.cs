using DiyMusicCommunity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiyMusicCommunity.Infrastructure.Persistence.Configurations;

public sealed class BandMemberConfiguration : IEntityTypeConfiguration<BandMember>
{
    public void Configure(EntityTypeBuilder<BandMember> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedNever();

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

        builder.HasData(
            new { Id = new Guid("de4db33f-cafe-f00d-a1b2-c3d4e5f00001"), BandId = new Guid("ba4dc0de-beef-cafe-f00d-b00000000001"), Name = "Sergio Campos",   Instrument = "Vocals", IsCurrent = true, IsLastKnownLineup = false, StartYear = (int?)2016, EndYear = (int?)null },
            new { Id = new Guid("de4db33f-cafe-f00d-a1b2-c3d4e5f00002"), BandId = new Guid("ba4dc0de-beef-cafe-f00d-b00000000001"), Name = "Hector Gonzalez", Instrument = "Guitar", IsCurrent = true, IsLastKnownLineup = false, StartYear = (int?)2016, EndYear = (int?)null },
            new { Id = new Guid("de4db33f-cafe-f00d-a1b2-c3d4e5f00003"), BandId = new Guid("ba4dc0de-beef-cafe-f00d-b00000000001"), Name = "Binky",           Instrument = "Bass",  IsCurrent = true, IsLastKnownLineup = false, StartYear = (int?)2016, EndYear = (int?)null },
            new { Id = new Guid("de4db33f-cafe-f00d-a1b2-c3d4e5f00004"), BandId = new Guid("ba4dc0de-beef-cafe-f00d-b00000000001"), Name = "Samuel Fernandez",Instrument = "Drums", IsCurrent = true, IsLastKnownLineup = false, StartYear = (int?)2016, EndYear = (int?)null });
    }
}
