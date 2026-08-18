using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiyMusicCommunity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLastKnownLineupToBandMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLastKnownLineup",
                table: "BandMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "BandMembers",
                keyColumn: "Id",
                keyValue: new Guid("de4db33f-cafe-f00d-a1b2-c3d4e5f00001"),
                column: "IsLastKnownLineup",
                value: false);

            migrationBuilder.UpdateData(
                table: "BandMembers",
                keyColumn: "Id",
                keyValue: new Guid("de4db33f-cafe-f00d-a1b2-c3d4e5f00002"),
                column: "IsLastKnownLineup",
                value: false);

            migrationBuilder.UpdateData(
                table: "BandMembers",
                keyColumn: "Id",
                keyValue: new Guid("de4db33f-cafe-f00d-a1b2-c3d4e5f00003"),
                column: "IsLastKnownLineup",
                value: false);

            migrationBuilder.UpdateData(
                table: "BandMembers",
                keyColumn: "Id",
                keyValue: new Guid("de4db33f-cafe-f00d-a1b2-c3d4e5f00004"),
                column: "IsLastKnownLineup",
                value: false);

            migrationBuilder.Sql("""
                UPDATE members
                SET IsLastKnownLineup = 1
                FROM BandMembers AS members
                INNER JOIN Bands AS bands ON bands.Id = members.BandId
                WHERE bands.Status = 'SplitUp' AND members.IsCurrent = 1;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLastKnownLineup",
                table: "BandMembers");
        }
    }
}
