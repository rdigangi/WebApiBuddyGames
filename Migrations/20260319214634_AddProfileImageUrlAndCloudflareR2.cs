using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiBuddyGames.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileImageUrlAndCloudflareR2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "utenti",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "utenti");
        }
    }
}
