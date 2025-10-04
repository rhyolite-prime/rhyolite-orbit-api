using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RhyoliteOrbit.Migrations
{
    /// <inheritdoc />
    public partial class initial12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductionDate",
                table: "FinishedGoods");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalDate",
                table: "ProductionOrde`rs",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "ProductionOrders",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "ProductionOrders",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalDate",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "ProductionOrders");

            migrationBuilder.AddColumn<DateTime>(
                name: "ProductionDate",
                table: "FinishedGoods",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
