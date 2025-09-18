using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_phone_numbers_users_UserId1",
                table: "user_phone_numbers");

            migrationBuilder.DropIndex(
                name: "IX_user_phone_numbers_UserId1",
                table: "user_phone_numbers");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "user_phone_numbers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "user_phone_numbers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_phone_numbers_UserId1",
                table: "user_phone_numbers",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_user_phone_numbers_users_UserId1",
                table: "user_phone_numbers",
                column: "UserId1",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
