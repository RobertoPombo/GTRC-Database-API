using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GTRC_Database_API.Migrations
{
    /// <inheritdoc />
    public partial class TweaksUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnDiscordServer",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "DiscordName",
                table: "Users",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "AccessToken",
                table: "Users",
                newName: "RefreshToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Users",
                newName: "DiscordName");

            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "Users",
                newName: "AccessToken");

            migrationBuilder.AddColumn<bool>(
                name: "IsOnDiscordServer",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
