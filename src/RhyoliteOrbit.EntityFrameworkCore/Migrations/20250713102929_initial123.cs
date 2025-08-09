using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RhyoliteOrbit.Migrations
{
    /// <inheritdoc />
    public partial class initial123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParentAccountHeaderId",
                table: "CoaControls",
                newName: "ParentSubGroupNameId");

            migrationBuilder.RenameColumn(
                name: "AccountHeaderName",
                table: "CoaControls",
                newName: "SubGroupName");

            migrationBuilder.RenameColumn(
                name: "AccountGroupId",
                table: "CoaControls",
                newName: "AccountHeadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubGroupName",
                table: "CoaControls",
                newName: "AccountHeaderName");

            migrationBuilder.RenameColumn(
                name: "ParentSubGroupNameId",
                table: "CoaControls",
                newName: "ParentAccountHeaderId");

            migrationBuilder.RenameColumn(
                name: "AccountHeadId",
                table: "CoaControls",
                newName: "AccountGroupId");
        }
    }
}
