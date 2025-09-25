using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RhyoliteOrbit.Migrations
{
    /// <inheritdoc />
    public partial class initial11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "BillSetups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "BillSetups",
                type: "character varying(85)",
                maxLength: 85,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "BillSetups");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "BillSetups");
        }
    }
}
