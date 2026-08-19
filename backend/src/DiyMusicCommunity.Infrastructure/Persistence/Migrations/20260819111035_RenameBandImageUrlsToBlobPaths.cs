using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiyMusicCommunity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameBandImageUrlsToBlobPaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LogoImageUrl",
                table: "Bands",
                newName: "LogoImageBlobPath");

            migrationBuilder.RenameColumn(
                name: "BandImageUrl",
                table: "Bands",
                newName: "BandPhotoBlobPath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LogoImageBlobPath",
                table: "Bands",
                newName: "LogoImageUrl");

            migrationBuilder.RenameColumn(
                name: "BandPhotoBlobPath",
                table: "Bands",
                newName: "BandImageUrl");
        }
    }
}
