using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DiyMusicCommunity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedGenres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), "Grindcore" },
                    { new Guid("a7b8c9d0-e1f2-3456-0123-567890123456"), "Goregrind" },
                    { new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"), "Crust" },
                    { new Guid("b8c9d0e1-f2a3-4567-1234-678901234567"), "Gorenoise" },
                    { new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), "D-Beat" },
                    { new Guid("c9d0e1f2-a3b4-5678-2345-789012345678"), "Death Metal" },
                    { new Guid("d0e1f2a3-b4c5-6789-3456-890123456789"), "Death-Grind" },
                    { new Guid("d4e5f6a7-b8c9-0123-def0-234567890123"), "Powerviolence" },
                    { new Guid("e5f6a7b8-c9d0-1234-ef01-345678901234"), "Punk" },
                    { new Guid("f6a7b8c9-d0e1-2345-f012-456789012345"), "Noise" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("a7b8c9d0-e1f2-3456-0123-567890123456"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("b8c9d0e1-f2a3-4567-1234-678901234567"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("c9d0e1f2-a3b4-5678-2345-789012345678"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("d0e1f2a3-b4c5-6789-3456-890123456789"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("d4e5f6a7-b8c9-0123-def0-234567890123"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("e5f6a7b8-c9d0-1234-ef01-345678901234"));

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: new Guid("f6a7b8c9-d0e1-2345-f012-456789012345"));
        }
    }
}
