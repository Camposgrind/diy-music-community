using DiyMusicCommunity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiyMusicCommunity.Infrastructure.Persistence.Configurations;

public sealed class TrackConfiguration : IEntityTypeConfiguration<Track>
{
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.HasIndex(t => new { t.ReleaseId, t.TrackNumber }).IsUnique();

        builder.HasData(
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000001"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "Crear Y Creer",              TrackNumber = 1  },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000002"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "Te Pudres",                 TrackNumber = 2  },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000003"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "Sangre Por Dinero",         TrackNumber = 3  },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000004"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "Noche Agónica",             TrackNumber = 4  },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000005"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "La Ley Del Padrón",         TrackNumber = 5  },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000006"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "Penitencia",                TrackNumber = 6  },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000007"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "La Molécula",               TrackNumber = 7  },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000008"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "Crisis De Identidad",       TrackNumber = 8  },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000009"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "Todo Habrá Acabado",        TrackNumber = 9  },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000010"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "Muerte Al Mainstream",      TrackNumber = 10 },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000011"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "La Espiral",                TrackNumber = 11 },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000012"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "No Soy Como Tú",            TrackNumber = 12 },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000013"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "Bestia De Carne",           TrackNumber = 13 },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000014"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "La Mosca",                  TrackNumber = 14 },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000015"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "Toubkal",                   TrackNumber = 15 },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000016"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "Puto Poser",                TrackNumber = 16 },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000017"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "Viejos Riders Nunca Mueren",TrackNumber = 17 },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000018"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "Sombra Eterna",             TrackNumber = 18 },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000019"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "Nunca Ganas",               TrackNumber = 19 },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000020"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "Vacío",                    TrackNumber = 20 },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000021"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "Sin Libertad",              TrackNumber = 21 },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000022"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "Fronteras",                 TrackNumber = 22 },
            new { Id = new Guid("c0de0000-cafe-beef-dead-000000000023"), ReleaseId = new Guid("a1b2cafe-dead-beef-f00d-c0de00000001"), Title = "Freya",                    TrackNumber = 23 });
    }
}
