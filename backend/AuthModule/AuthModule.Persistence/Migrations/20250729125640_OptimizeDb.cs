using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_refresh_tokens_is_revoked_revoked_at",
                table: "refresh_tokens");

            migrationBuilder.RenameColumn(
                name: "is_baned",
                table: "auth_users",
                newName: "is_banned");

            migrationBuilder.RenameColumn(
                name: "baned_at",
                table: "auth_users",
                newName: "banned_at");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "refresh_tokens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "IX_auth_users_email",
                table: "auth_users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_auth_users_phone_number",
                table: "auth_users",
                column: "phone_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_auth_users_email",
                table: "auth_users");

            migrationBuilder.DropIndex(
                name: "IX_auth_users_phone_number",
                table: "auth_users");

            migrationBuilder.RenameColumn(
                name: "is_banned",
                table: "auth_users",
                newName: "is_baned");

            migrationBuilder.RenameColumn(
                name: "banned_at",
                table: "auth_users",
                newName: "baned_at");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "refresh_tokens",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_is_revoked_revoked_at",
                table: "refresh_tokens",
                columns: new[] { "is_revoked", "revoked_at" });
        }
    }
}
