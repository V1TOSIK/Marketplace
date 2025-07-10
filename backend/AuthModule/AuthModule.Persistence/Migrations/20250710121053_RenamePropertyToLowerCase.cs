using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenamePropertyToLowerCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "auth_users",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "IsBaned",
                table: "auth_users",
                newName: "is_baned");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "auth_users",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "BanedAt",
                table: "auth_users",
                newName: "baned_at");

            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                table: "auth_users",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "is_baned",
                table: "auth_users",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "auth_users",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "is_baned",
                table: "auth_users",
                newName: "IsBaned");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "auth_users",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "baned_at",
                table: "auth_users",
                newName: "BanedAt");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "auth_users",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsBaned",
                table: "auth_users",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);
        }
    }
}
