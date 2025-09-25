using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RhyoliteOrbit.Migrations
{
    /// <inheritdoc />
    public partial class initial111 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "BillSetups");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "BillSetups");

            migrationBuilder.AlterColumn<string>(
                name: "BillTypeName",
                table: "FeesDescriptions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "FeesDescriptions",
                type: "integer",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "FeesDescriptions",
                type: "character varying(85)",
                maxLength: 85,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "FeesDescriptions");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "FeesDescriptions");

            migrationBuilder.AlterColumn<string>(
                name: "BillTypeName",
                table: "FeesDescriptions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

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
    }
}
