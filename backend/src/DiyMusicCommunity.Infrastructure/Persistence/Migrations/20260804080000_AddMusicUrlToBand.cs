using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiyMusicCommunity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMusicUrlToBand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MusicUrlPortal",
                table: "Bands",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MusicUrlPortal",
                table: "Bands");
        }
    }
}
