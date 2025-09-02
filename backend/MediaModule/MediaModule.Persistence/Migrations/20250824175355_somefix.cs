using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SomeFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "deleted_at",
                table: "medias",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "IX_Media_EntityId",
                table: "medias",
                column: "entity_id");

            migrationBuilder.CreateIndex(
                name: "IX_Media_IsMain",
                table: "medias",
                column: "is_main");

            migrationBuilder.CreateIndex(
                name: "IX_medias_url",
                table: "medias",
                column: "url",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Media_EntityId",
                table: "medias");

            migrationBuilder.DropIndex(
                name: "IX_Media_IsMain",
                table: "medias");

            migrationBuilder.DropIndex(
                name: "IX_medias_url",
                table: "medias");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deleted_at",
                table: "medias",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
