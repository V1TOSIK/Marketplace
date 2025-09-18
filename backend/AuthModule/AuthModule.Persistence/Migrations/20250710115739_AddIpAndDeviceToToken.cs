using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIpAndDeviceToToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "auth_users",
                newName: "id");

            migrationBuilder.AddColumn<string>(
                name: "device",
                table: "refresh_tokens",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ip_address",
                table: "refresh_tokens",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_is_revoked_revoked_at",
                table: "refresh_tokens",
                columns: new[] { "is_revoked", "revoked_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_refresh_tokens_is_revoked_revoked_at",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "device",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "ip_address",
                table: "refresh_tokens");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "auth_users",
                newName: "user_id");
        }
    }
}
