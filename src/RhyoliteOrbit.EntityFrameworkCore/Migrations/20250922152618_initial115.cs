using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RhyoliteOrbit.Migrations
{
    /// <inheritdoc />
    public partial class initial115 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "RawMaterials");

            migrationBuilder.AlterColumn<string>(
                name: "UnitOfMeasure",
                table: "ScrapAndWastes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnitOfMeasureId",
                table: "ScrapAndWastes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "UnitOfMeasure",
                table: "RawMaterials",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InventoryCategory",
                table: "RawMaterials",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "UnitOfMeasureId",
                table: "RawMaterials",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "InventoryCategory",
                table: "FinishedGoods",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UnitOfMeasure",
                table: "FinishedGoods",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnitOfMeasureId",
                table: "FinishedGoods",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "UnitOfMeasurements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitOfMeasurements", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnitOfMeasurements");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasureId",
                table: "ScrapAndWastes");

            migrationBuilder.DropColumn(
                name: "InventoryCategory",
                table: "RawMaterials");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasureId",
                table: "RawMaterials");

            migrationBuilder.DropColumn(
                name: "InventoryCategory",
                table: "FinishedGoods");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasure",
                table: "FinishedGoods");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasureId",
                table: "FinishedGoods");

            migrationBuilder.AlterColumn<string>(
                name: "UnitOfMeasure",
                table: "ScrapAndWastes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UnitOfMeasure",
                table: "RawMaterials",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "RawMaterials",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
