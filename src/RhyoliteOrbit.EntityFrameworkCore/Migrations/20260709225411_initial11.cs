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
            migrationBuilder.RenameColumn(
                name: "AccountHeaderName",
                table: "CoaDetails",
                newName: "AccountControlName");

            migrationBuilder.RenameColumn(
                name: "AccountHeaderId",
                table: "CoaDetails",
                newName: "AccountControlId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccountControlName",
                table: "CoaDetails",
                newName: "AccountHeaderName");

            migrationBuilder.RenameColumn(
                name: "AccountControlId",
                table: "CoaDetails",
                newName: "AccountHeaderId");
        }
    }
}
