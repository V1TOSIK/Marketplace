using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumnsInRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_auth_users_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "ReplacedByToken",
                table: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "refresh_tokens");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "refresh_tokens",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "refresh_tokens",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "RevokedAt",
                table: "refresh_tokens",
                newName: "revoked_at");

            migrationBuilder.RenameColumn(
                name: "IsRevoked",
                table: "refresh_tokens",
                newName: "is_revoked");

            migrationBuilder.RenameColumn(
                name: "ExpirationDate",
                table: "refresh_tokens",
                newName: "expiration_date");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "refresh_tokens",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserId",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_UserId");

            migrationBuilder.AddColumn<Guid>(
                name: "replaced_by_token_id",
                table: "refresh_tokens",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_refresh_tokens",
                table: "refresh_tokens",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_tokens_auth_users_UserId",
                table: "refresh_tokens",
                column: "UserId",
                principalTable: "auth_users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refresh_tokens_auth_users_UserId",
                table: "refresh_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_refresh_tokens",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "replaced_by_token_id",
                table: "refresh_tokens");

            migrationBuilder.RenameTable(
                name: "refresh_tokens",
                newName: "RefreshTokens");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "RefreshTokens",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "RefreshTokens",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "revoked_at",
                table: "RefreshTokens",
                newName: "RevokedAt");

            migrationBuilder.RenameColumn(
                name: "is_revoked",
                table: "RefreshTokens",
                newName: "IsRevoked");

            migrationBuilder.RenameColumn(
                name: "expiration_date",
                table: "RefreshTokens",
                newName: "ExpirationDate");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "RefreshTokens",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_UserId",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserId");

            migrationBuilder.AddColumn<string>(
                name: "ReplacedByToken",
                table: "RefreshTokens",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_auth_users_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "auth_users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
