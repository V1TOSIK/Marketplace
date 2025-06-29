using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class renamecolumnuseridinrefreshtoken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refresh_tokens_auth_users_UserId",
                table: "refresh_tokens");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "refresh_tokens",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_UserId",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_tokens_auth_users_user_id",
                table: "refresh_tokens",
                column: "user_id",
                principalTable: "auth_users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refresh_tokens_auth_users_user_id",
                table: "refresh_tokens");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "refresh_tokens",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_user_id",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_tokens_auth_users_UserId",
                table: "refresh_tokens",
                column: "UserId",
                principalTable: "auth_users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
