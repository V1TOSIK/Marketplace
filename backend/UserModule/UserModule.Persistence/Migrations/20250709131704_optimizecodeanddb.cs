using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeCodeAndDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_blocks_users_UserId1",
                table: "user_blocks");

            migrationBuilder.DropIndex(
                name: "IX_user_blocks_user_id",
                table: "user_blocks");

            migrationBuilder.DropIndex(
                name: "IX_user_blocks_UserId1",
                table: "user_blocks");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "user_blocks");

            migrationBuilder.CreateIndex(
                name: "IX_user_blocks_user_id_blocked_user_id",
                table: "user_blocks",
                columns: new[] { "user_id", "blocked_user_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_blocks_user_id_blocked_user_id",
                table: "user_blocks");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "user_blocks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_blocks_user_id",
                table: "user_blocks",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_blocks_UserId1",
                table: "user_blocks",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_user_blocks_users_UserId1",
                table: "user_blocks",
                column: "UserId1",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
