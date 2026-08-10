using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiyMusicCommunity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedConvulsionsBandContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Bands",
                keyColumn: "Id",
                keyValue: new Guid("ba4dc0de-beef-cafe-f00d-b00000000001"),
                column: "BandContact",
                value: "convulsionsgrindcore@gmail.com");

            migrationBuilder.AddForeignKey(
                name: "FK_BandMemberOtherBands_Bands_OtherBandId",
                table: "BandMemberOtherBands",
                column: "OtherBandId",
                principalTable: "Bands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BandMembers_Bands_BandId",
                table: "BandMembers",
                column: "BandId",
                principalTable: "Bands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Releases_Bands_BandId",
                table: "Releases",
                column: "BandId",
                principalTable: "Bands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BandMemberOtherBands_Bands_OtherBandId",
                table: "BandMemberOtherBands");

            migrationBuilder.DropForeignKey(
                name: "FK_BandMembers_Bands_BandId",
                table: "BandMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Releases_Bands_BandId",
                table: "Releases");

            migrationBuilder.UpdateData(
                table: "Bands",
                keyColumn: "Id",
                keyValue: new Guid("ba4dc0de-beef-cafe-f00d-b00000000001"),
                column: "BandContact",
                value: null);
        }
    }
}
