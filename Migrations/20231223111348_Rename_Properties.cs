using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GTRC_Database_API.Migrations
{
    /// <inheritdoc />
    public partial class Rename_Properties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccTrackID",
                table: "Tracks",
                newName: "AccTrackId");

            migrationBuilder.RenameColumn(
                name: "Name_GTRC",
                table: "Tracks",
                newName: "NameGtrc");

            migrationBuilder.RenameColumn(
                name: "AccCarID",
                table: "Cars",
                newName: "AccCarId");

            migrationBuilder.RenameColumn(
                name: "Name_GTRC",
                table: "Cars",
                newName: "NameGtrc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccTrackId",
                table: "Tracks",
                newName: "AccTrackID");

            migrationBuilder.RenameColumn(
                name: "NameGtrc",
                table: "Tracks",
                newName: "Name_GTRC");

            migrationBuilder.RenameColumn(
                name: "AccCarId",
                table: "Cars",
                newName: "AccCarID");

            migrationBuilder.RenameColumn(
                name: "NameGtrc",
                table: "Cars",
                newName: "Name_GTRC");
        }
    }
}
