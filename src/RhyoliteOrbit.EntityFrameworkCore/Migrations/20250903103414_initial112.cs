using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RhyoliteOrbit.Migrations
{
    /// <inheritdoc />
    public partial class initial112 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Pin",
                table: "Students",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pin",
                table: "Students");
        }
    }
}
