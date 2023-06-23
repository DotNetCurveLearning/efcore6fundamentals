using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PublisherData.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingCoverPK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtistCover_Covers_CoversCoverID",
                table: "ArtistCover");

            migrationBuilder.RenameColumn(
                name: "CoverID",
                table: "Covers",
                newName: "CoverId");

            migrationBuilder.RenameColumn(
                name: "CoversCoverID",
                table: "ArtistCover",
                newName: "CoversCoverId");

            migrationBuilder.RenameIndex(
                name: "IX_ArtistCover_CoversCoverID",
                table: "ArtistCover",
                newName: "IX_ArtistCover_CoversCoverId");

            migrationBuilder.InsertData(
                table: "Artists",
                columns: new[] { "ArtistId", "FirstName", "LastName" },
                values: new object[,]
                {
                    { 1, "Pablo", "Picasso" },
                    { 2, "Dee", "Bell" },
                    { 3, "Katharine", "Kuharic" }
                });

            migrationBuilder.InsertData(
                table: "Covers",
                columns: new[] { "CoverId", "DesignIdeas", "DigitalOnly" },
                values: new object[,]
                {
                    { 1, "How about a left hand in the dark?", false },
                    { 2, "Should we put a clock?", true },
                    { 3, "A big ear in the clouds?", false }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistCover_Covers_CoversCoverId",
                table: "ArtistCover",
                column: "CoversCoverId",
                principalTable: "Covers",
                principalColumn: "CoverId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtistCover_Covers_CoversCoverId",
                table: "ArtistCover");

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "ArtistId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "ArtistId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "ArtistId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Covers",
                keyColumn: "CoverId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Covers",
                keyColumn: "CoverId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Covers",
                keyColumn: "CoverId",
                keyValue: 3);

            migrationBuilder.RenameColumn(
                name: "CoverId",
                table: "Covers",
                newName: "CoverID");

            migrationBuilder.RenameColumn(
                name: "CoversCoverId",
                table: "ArtistCover",
                newName: "CoversCoverID");

            migrationBuilder.RenameIndex(
                name: "IX_ArtistCover_CoversCoverId",
                table: "ArtistCover",
                newName: "IX_ArtistCover_CoversCoverID");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistCover_Covers_CoversCoverID",
                table: "ArtistCover",
                column: "CoversCoverID",
                principalTable: "Covers",
                principalColumn: "CoverID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
