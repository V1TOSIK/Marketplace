using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTemplateIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_characteristic_templates_name_category_id",
                table: "characteristic_templates");

            migrationBuilder.CreateIndex(
                name: "IX_characteristic_templates_name_unit_category_id",
                table: "characteristic_templates",
                columns: new[] { "name", "unit", "category_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_characteristic_templates_name_unit_category_id",
                table: "characteristic_templates");

            migrationBuilder.CreateIndex(
                name: "IX_characteristic_templates_name_category_id",
                table: "characteristic_templates",
                columns: new[] { "name", "category_id" },
                unique: true);
        }
    }
}
