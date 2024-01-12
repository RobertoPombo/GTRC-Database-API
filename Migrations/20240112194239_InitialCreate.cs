using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GTRC_Database_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Alpha = table.Column<byte>(type: "tinyint", nullable: false),
                    Red = table.Column<byte>(type: "tinyint", nullable: false),
                    Green = table.Column<byte>(type: "tinyint", nullable: false),
                    Blue = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccTrackId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PitBoxesCount = table.Column<int>(type: "int", nullable: false),
                    ServerSlotsCount = table.Column<int>(type: "int", nullable: false),
                    AccTimePenDtS = table.Column<int>(type: "int", nullable: false),
                    NameGtrc = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SteamId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    DiscordId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BanDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name3Digits = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EloRating = table.Column<short>(type: "smallint", nullable: false),
                    SafetyRating = table.Column<short>(type: "smallint", nullable: false),
                    Warnings = table.Column<byte>(type: "tinyint", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SteamLoginToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscordLoginToken = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccCarId = table.Column<long>(type: "bigint", nullable: false),
                    ManufacturerId = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Class = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    ReleaseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    WidthMm = table.Column<int>(type: "int", nullable: false),
                    LengthMm = table.Column<int>(type: "int", nullable: false),
                    NameGtrc = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cars_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    DiscordRoleId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    DiscordChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
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
                name: "IX_Cars_ManufacturerId",
                table: "Cars",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_SeriesId",
                table: "Seasons",
                column: "SeriesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "Tracks");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Manufacturers");

            migrationBuilder.DropTable(
                name: "Series");
        }
    }
}
