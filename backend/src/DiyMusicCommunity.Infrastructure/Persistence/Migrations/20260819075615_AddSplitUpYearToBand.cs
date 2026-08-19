using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiyMusicCommunity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSplitUpYearToBand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SplitUpYear",
                table: "Bands",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Bands",
                keyColumn: "Id",
                keyValue: new Guid("ba4dc0de-beef-cafe-f00d-b00000000001"),
                column: "SplitUpYear",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SplitUpYear",
                table: "Bands");
        }
    }
}
