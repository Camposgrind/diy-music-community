using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiyMusicCommunity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueTrackNumberPerRelease : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tracks_ReleaseId",
                table: "Tracks");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_ReleaseId_TrackNumber",
                table: "Tracks",
                columns: new[] { "ReleaseId", "TrackNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tracks_ReleaseId_TrackNumber",
                table: "Tracks");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_ReleaseId",
                table: "Tracks",
                column: "ReleaseId");
        }
    }
}
