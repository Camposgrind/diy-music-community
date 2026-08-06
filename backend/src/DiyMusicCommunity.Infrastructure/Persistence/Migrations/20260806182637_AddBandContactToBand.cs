using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiyMusicCommunity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBandContactToBand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BandContact",
                table: "Bands",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Bands",
                keyColumn: "Id",
                keyValue: new Guid("ba4dc0de-beef-cafe-f00d-b00000000001"),
                column: "BandContact",
                value: null);

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: new Guid("c0de0000-cafe-beef-dead-000000000020"),
                column: "Title",
                value: "Vacío");

            migrationBuilder.CreateIndex(
                name: "IX_Bands_GenreId",
                table: "Bands",
                column: "GenreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bands_Genres_GenreId",
                table: "Bands",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bands_Genres_GenreId",
                table: "Bands");

            migrationBuilder.DropIndex(
                name: "IX_Bands_GenreId",
                table: "Bands");

            migrationBuilder.DropColumn(
                name: "BandContact",
                table: "Bands");

            migrationBuilder.UpdateData(
                table: "Tracks",
                keyColumn: "Id",
                keyValue: new Guid("c0de0000-cafe-beef-dead-000000000020"),
                column: "Title",
                value: "");
        }
    }
}
