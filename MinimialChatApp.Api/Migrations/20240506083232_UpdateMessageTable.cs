using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimialChatApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMessageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageHistory",
                table: "UserGroups");

            migrationBuilder.AddColumn<int>(
                name: "ShowOptions",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowOptions",
                table: "Messages");

            migrationBuilder.AddColumn<int>(
                name: "MessageHistory",
                table: "UserGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
