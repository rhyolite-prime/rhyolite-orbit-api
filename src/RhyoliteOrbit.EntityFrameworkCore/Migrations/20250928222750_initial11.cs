using System;
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
            migrationBuilder.DropTable(
                name: "Relationships");

            migrationBuilder.DropTable(
                name: "Religions");

            migrationBuilder.DropColumn(
                name: "ReligionId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "FirstGuardianRelationshipId",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "SecondGuardianRelationshipId",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "ReligionId",
                table: "EmployeeBioDatas");

            migrationBuilder.RenameColumn(
                name: "ReligionName",
                table: "Students",
                newName: "Religion");

            migrationBuilder.RenameColumn(
                name: "SecondGuardianRelationshipName",
                table: "Parents",
                newName: "SecondGuardianRelationship");

            migrationBuilder.RenameColumn(
                name: "FirstGuardianRelationshipName",
                table: "Parents",
                newName: "FirstGuardianRelationship");

            migrationBuilder.RenameColumn(
                name: "ReligionName",
                table: "EmployeeBioDatas",
                newName: "Religion");

            migrationBuilder.AddColumn<string>(
                name: "Religion",
                table: "Staffs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Religion",
                table: "Staffs");

            migrationBuilder.RenameColumn(
                name: "Religion",
                table: "Students",
                newName: "ReligionName");

            migrationBuilder.RenameColumn(
                name: "SecondGuardianRelationship",
                table: "Parents",
                newName: "SecondGuardianRelationshipName");

            migrationBuilder.RenameColumn(
                name: "FirstGuardianRelationship",
                table: "Parents",
                newName: "FirstGuardianRelationshipName");

            migrationBuilder.RenameColumn(
                name: "Religion",
                table: "EmployeeBioDatas",
                newName: "ReligionName");

            migrationBuilder.AddColumn<Guid>(
                name: "ReligionId",
                table: "Students",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "FirstGuardianRelationshipId",
                table: "Parents",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SecondGuardianRelationshipId",
                table: "Parents",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ReligionId",
                table: "EmployeeBioDatas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Relationships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relationships", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Religions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Religions", x => x.Id);
                });
        }
    }
}
