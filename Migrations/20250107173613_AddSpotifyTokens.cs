using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APISocMed.Migrations
{
    /// <inheritdoc />
    public partial class AddSpotifyTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpotifyId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpotifyRefreshToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpotifyId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SpotifyRefreshToken",
                table: "Users");
        }
    }
}
