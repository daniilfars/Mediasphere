using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTwoIndexToLikeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Likes_TargetType_ContentId",
                table: "Likes",
                columns: new[] { "TargetType", "ContentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId_TargetType_ContentId",
                table: "Likes",
                columns: new[] { "UserId", "TargetType", "ContentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Likes_TargetType_ContentId",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Likes_UserId_TargetType_ContentId",
                table: "Likes");
        }
    }
}
