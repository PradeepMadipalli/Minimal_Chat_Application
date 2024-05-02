using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimialChatApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOnlineStatusColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OnlineStatus",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnlineStatus",
                table: "AspNetUsers");
        }
    }
}
