using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GTRC_Database_API.Migrations
{
    /// <inheritdoc />
    public partial class AddTableSeason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeriesId = table.Column<int>(type: "int", nullable: false),
                    MinDriversPerEntry = table.Column<byte>(type: "tinyint", nullable: false),
                    MaxDriversPerEntry = table.Column<byte>(type: "tinyint", nullable: false),
                    GridSlotsLimit = table.Column<byte>(type: "tinyint", nullable: false),
                    CarLimitBallast = table.Column<byte>(type: "tinyint", nullable: false),
                    GainBallast = table.Column<byte>(type: "tinyint", nullable: false),
                    CarLimitRestrictor = table.Column<byte>(type: "tinyint", nullable: false),
                    GainRestrictor = table.Column<byte>(type: "tinyint", nullable: false),
                    CarLimitRegisterLimit = table.Column<byte>(type: "tinyint", nullable: false),
                    DateRegisterLimit = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateBoPFreeze = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoShowLimit = table.Column<byte>(type: "tinyint", nullable: false),
                    SignOutLimit = table.Column<byte>(type: "tinyint", nullable: false),
                    CarChangeLimit = table.Column<byte>(type: "tinyint", nullable: false),
                    DateCarChangeLimit = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GroupCarLimits = table.Column<bool>(type: "bit", nullable: false),
                    BopLatestModelOnly = table.Column<bool>(type: "bit", nullable: false),
                    DaysIgnoreCarLimits = table.Column<int>(type: "int", nullable: false),
                    FormationLapType = table.Column<int>(type: "int", nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seasons_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_SeriesId",
                table: "Seasons",
                column: "SeriesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Seasons");
        }
    }
}
