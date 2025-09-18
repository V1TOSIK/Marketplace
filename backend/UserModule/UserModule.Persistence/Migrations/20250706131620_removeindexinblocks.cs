using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIndexInBlocks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserBlocks_UserId_BlockedUserId",
                table: "user_blocks");

            migrationBuilder.CreateIndex(
                name: "IX_user_blocks_user_id",
                table: "user_blocks",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_blocks_user_id",
                table: "user_blocks");

            migrationBuilder.CreateIndex(
                name: "IX_UserBlocks_UserId_BlockedUserId",
                table: "user_blocks",
                columns: new[] { "user_id", "blocked_user_id" },
                unique: true);
        }
    }
}
