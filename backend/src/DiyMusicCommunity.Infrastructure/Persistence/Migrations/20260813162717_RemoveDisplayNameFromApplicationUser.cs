using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiyMusicCommunity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDisplayNameFromApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DisplayName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DisplayName",
                table: "AspNetUsers",
                column: "DisplayName");
        }
    }
}
