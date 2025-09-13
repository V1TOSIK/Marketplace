using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProductModule.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIdToCharacteristicValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_characteristic_values",
                table: "characteristic_values");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "characteristic_values",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_characteristic_values",
                table: "characteristic_values",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_characteristic_values",
                table: "characteristic_values");

            migrationBuilder.DropColumn(
                name: "id",
                table: "characteristic_values");

            migrationBuilder.AddPrimaryKey(
                name: "PK_characteristic_values",
                table: "characteristic_values",
                columns: new[] { "value", "characteristic_template_id", "group_id" });
        }
    }
}
