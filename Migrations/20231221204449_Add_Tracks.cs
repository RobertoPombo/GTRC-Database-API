using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GTRC_Database_API.Migrations
{
    /// <inheritdoc />
    public partial class Add_Tracks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccTrackID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PitBoxesCount = table.Column<int>(type: "int", nullable: false),
                    ServerSlotsCount = table.Column<int>(type: "int", nullable: false),
                    AccTimePenDT = table.Column<int>(type: "int", nullable: false),
                    Name_GTRC = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tracks");
        }
    }
}
