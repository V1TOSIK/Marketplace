using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class renameblocktoban : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsBlocked",
                table: "auth_users",
                newName: "IsBaned");

            migrationBuilder.RenameColumn(
                name: "BlockedAt",
                table: "auth_users",
                newName: "BanedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsBaned",
                table: "auth_users",
                newName: "IsBlocked");

            migrationBuilder.RenameColumn(
                name: "BanedAt",
                table: "auth_users",
                newName: "BlockedAt");
        }
    }
}
