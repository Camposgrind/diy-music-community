using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DiyMusicCommunity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedConvulsionsBand : Migration
    {
        // ── Fixed GUIDs ────────────────────────────────────────────────────────
        // Seeded by 20260804072301_SeedGenres
        private static readonly Guid GenreGrindcoreId  = new("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

        private static readonly Guid BandId            = new("ba4dc0de-beef-cafe-f00d-b00000000001");

        private static readonly Guid MemberSergioId    = new("de4db33f-cafe-f00d-a1b2-c3d4e5f00001");
        private static readonly Guid MemberHectorId    = new("de4db33f-cafe-f00d-a1b2-c3d4e5f00002");
        private static readonly Guid MemberBinkyId     = new("de4db33f-cafe-f00d-a1b2-c3d4e5f00003");
        private static readonly Guid MemberSamuelId    = new("de4db33f-cafe-f00d-a1b2-c3d4e5f00004");

        private static readonly Guid ReleaseId         = new("a1b2cafe-dead-beef-f00d-c0de00000001");

        private static readonly Guid FormatVinyl12Id   = new("f04f0000-cafe-beef-dead-000000000001");
        private static readonly Guid FormatCDId        = new("f04f0000-cafe-beef-dead-000000000002");
        private static readonly Guid FormatCassetteId  = new("f04f0000-cafe-beef-dead-000000000003");

        private static readonly Guid Track01Id         = new("c0de0000-cafe-beef-dead-000000000001");
        private static readonly Guid Track02Id         = new("c0de0000-cafe-beef-dead-000000000002");
        private static readonly Guid Track03Id         = new("c0de0000-cafe-beef-dead-000000000003");
        private static readonly Guid Track04Id         = new("c0de0000-cafe-beef-dead-000000000004");
        private static readonly Guid Track05Id         = new("c0de0000-cafe-beef-dead-000000000005");
        private static readonly Guid Track06Id         = new("c0de0000-cafe-beef-dead-000000000006");
        private static readonly Guid Track07Id         = new("c0de0000-cafe-beef-dead-000000000007");
        private static readonly Guid Track08Id         = new("c0de0000-cafe-beef-dead-000000000008");
        private static readonly Guid Track09Id         = new("c0de0000-cafe-beef-dead-000000000009");
        private static readonly Guid Track10Id         = new("c0de0000-cafe-beef-dead-000000000010");
        private static readonly Guid Track11Id         = new("c0de0000-cafe-beef-dead-000000000011");
        private static readonly Guid Track12Id         = new("c0de0000-cafe-beef-dead-000000000012");
        private static readonly Guid Track13Id         = new("c0de0000-cafe-beef-dead-000000000013");
        private static readonly Guid Track14Id         = new("c0de0000-cafe-beef-dead-000000000014");
        private static readonly Guid Track15Id         = new("c0de0000-cafe-beef-dead-000000000015");
        private static readonly Guid Track16Id         = new("c0de0000-cafe-beef-dead-000000000016");
        private static readonly Guid Track17Id         = new("c0de0000-cafe-beef-dead-000000000017");
        private static readonly Guid Track18Id         = new("c0de0000-cafe-beef-dead-000000000018");
        private static readonly Guid Track19Id         = new("c0de0000-cafe-beef-dead-000000000019");
        private static readonly Guid Track20Id         = new("c0de0000-cafe-beef-dead-000000000020");
        private static readonly Guid Track21Id         = new("c0de0000-cafe-beef-dead-000000000021");
        private static readonly Guid Track22Id         = new("c0de0000-cafe-beef-dead-000000000022");
        private static readonly Guid Track23Id         = new("c0de0000-cafe-beef-dead-000000000023");

        private static readonly DateTime SeedDate = new(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc);
        // ───────────────────────────────────────────────────────────────────────

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── Band ──────────────────────────────────────────────────────────
            migrationBuilder.InsertData(
                table: "Bands",
                columns: new[]
                {
                    "Id", "Name", "Country", "Location", "GenreId",
                    "Status", "FormationYear", "Description",
                    "LogoImageUrl", "BandImageUrl", "MusicUrlPortal",
                    "TrustStatus", "IsClaimed", "CreatedAt", "UpdatedAt"
                },
                values: new object[]
                {
                    BandId, "Convulsions", "Spain", "El Ejido", GenreGrindcoreId,
                    "Active", 2016, null,
                    null, null, "https://convulsionsgrindcore.bandcamp.com",
                    "Claimed", true, SeedDate, SeedDate
                });

            // ── BandMembers ───────────────────────────────────────────────────
            migrationBuilder.InsertData(
                table: "BandMembers",
                columns: new[] { "Id", "BandId", "Name", "Instrument", "StartYear", "EndYear", "IsCurrent" },
                values: new object[,]
                {
                    { MemberSergioId, BandId, "Sergio Campos",    "Vocals",  2016, null, true },
                    { MemberHectorId, BandId, "Hector Gonzalez",  "Guitar",  2016, null, true },
                    { MemberBinkyId,  BandId, "Binky",            "Bass",    2016, null, true },
                    { MemberSamuelId, BandId, "Samuel Fernandez", "Drums",   2016, null, true }
                });

            // ── Release ───────────────────────────────────────────────────────
            migrationBuilder.InsertData(
                table: "Releases",
                columns: new[] { "Id", "BandId", "Title", "ReleaseType", "ReleaseDate", "Year", "LabelText", "CoverImageUrl" },
                values: new object[]
                {
                    ReleaseId, BandId, "Grindcore Not War", "Album",
                    new DateOnly(2023, 3, 10), 2023,
                    "Hecatombe Records, Regurgitated Semen Records", null
                });

            // ── ReleaseFormats ────────────────────────────────────────────────
            migrationBuilder.InsertData(
                table: "ReleaseFormats",
                columns: new[] { "Id", "ReleaseId", "Format" },
                values: new object[,]
                {
                    { FormatVinyl12Id,  ReleaseId, "Vinyl12"  },
                    { FormatCDId,       ReleaseId, "CD"       },
                    { FormatCassetteId, ReleaseId, "Cassette" }
                });

            // ── Tracks ────────────────────────────────────────────────────────
            migrationBuilder.InsertData(
                table: "Tracks",
                columns: new[] { "Id", "ReleaseId", "Title", "TrackNumber" },
                values: new object[,]
                {
                    { Track01Id, ReleaseId, "Crear Y Creer",              1  },
                    { Track02Id, ReleaseId, "Te Pudres",                  2  },
                    { Track03Id, ReleaseId, "Sangre Por Dinero",          3  },
                    { Track04Id, ReleaseId, "Noche Agónica",              4  },
                    { Track05Id, ReleaseId, "La Ley Del Padrón",          5  },
                    { Track06Id, ReleaseId, "Penitencia",                 6  },
                    { Track07Id, ReleaseId, "La Molécula",                7  },
                    { Track08Id, ReleaseId, "Crisis De Identidad",        8  },
                    { Track09Id, ReleaseId, "Todo Habrá Acabado",         9  },
                    { Track10Id, ReleaseId, "Muerte Al Mainstream",       10 },
                    { Track11Id, ReleaseId, "La Espiral",                 11 },
                    { Track12Id, ReleaseId, "No Soy Como Tú",             12 },
                    { Track13Id, ReleaseId, "Bestia De Carne",            13 },
                    { Track14Id, ReleaseId, "La Mosca",                   14 },
                    { Track15Id, ReleaseId, "Toubkal",                    15 },
                    { Track16Id, ReleaseId, "Puto Poser",                 16 },
                    { Track17Id, ReleaseId, "Viejos Riders Nunca Mueren", 17 },
                    { Track18Id, ReleaseId, "Sombra Eterna",              18 },
                    { Track19Id, ReleaseId, "Nunca Ganas",                19 },
                    { Track20Id, ReleaseId, "",                           20 },
                    { Track21Id, ReleaseId, "Sin Libertad",               21 },
                    { Track22Id, ReleaseId, "Fronteras",                  22 },
                    { Track23Id, ReleaseId, "Freya",                      23 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ── Tracks ────────────────────────────────────────────────────────
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track01Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track02Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track03Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track04Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track05Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track06Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track07Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track08Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track09Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track10Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track11Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track12Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track13Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track14Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track15Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track16Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track17Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track18Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track19Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track20Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track21Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track22Id);
            migrationBuilder.DeleteData(table: "Tracks", keyColumn: "Id", keyValue: Track23Id);

            // ── ReleaseFormats ────────────────────────────────────────────────
            migrationBuilder.DeleteData(table: "ReleaseFormats", keyColumn: "Id", keyValue: FormatVinyl12Id);
            migrationBuilder.DeleteData(table: "ReleaseFormats", keyColumn: "Id", keyValue: FormatCDId);
            migrationBuilder.DeleteData(table: "ReleaseFormats", keyColumn: "Id", keyValue: FormatCassetteId);

            // ── Release ───────────────────────────────────────────────────────
            migrationBuilder.DeleteData(table: "Releases", keyColumn: "Id", keyValue: ReleaseId);

            // ── BandMembers ───────────────────────────────────────────────────
            migrationBuilder.DeleteData(table: "BandMembers", keyColumn: "Id", keyValue: MemberSergioId);
            migrationBuilder.DeleteData(table: "BandMembers", keyColumn: "Id", keyValue: MemberHectorId);
            migrationBuilder.DeleteData(table: "BandMembers", keyColumn: "Id", keyValue: MemberBinkyId);
            migrationBuilder.DeleteData(table: "BandMembers", keyColumn: "Id", keyValue: MemberSamuelId);

            // ── Band ──────────────────────────────────────────────────────────
            migrationBuilder.DeleteData(table: "Bands", keyColumn: "Id", keyValue: BandId);
        }
    }
}
