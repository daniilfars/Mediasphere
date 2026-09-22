using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameToComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Comments",
                newName: "AuthorId");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Comments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Comments",
                newName: "UserId");
        }
    }
}
