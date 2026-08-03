using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiyMusicCommunity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFormatsAndOtherBands : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormatsText",
                table: "Releases");

            migrationBuilder.DropColumn(
                name: "AlsoInBandsText",
                table: "BandMembers");

            migrationBuilder.CreateTable(
                name: "BandMemberOtherBands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BandMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OtherBandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BandMemberOtherBands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BandMemberOtherBands_BandMembers_BandMemberId",
                        column: x => x.BandMemberId,
                        principalTable: "BandMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReleaseFormats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReleaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Format = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleaseFormats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReleaseFormats_Releases_ReleaseId",
                        column: x => x.ReleaseId,
                        principalTable: "Releases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BandMemberOtherBands_BandMemberId_OtherBandId",
                table: "BandMemberOtherBands",
                columns: new[] { "BandMemberId", "OtherBandId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BandMemberOtherBands_OtherBandId",
                table: "BandMemberOtherBands",
                column: "OtherBandId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseFormats_ReleaseId_Format",
                table: "ReleaseFormats",
                columns: new[] { "ReleaseId", "Format" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BandMemberOtherBands");

            migrationBuilder.DropTable(
                name: "ReleaseFormats");

            migrationBuilder.AddColumn<string>(
                name: "FormatsText",
                table: "Releases",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlsoInBandsText",
                table: "BandMembers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
