using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using RhyoliteOrbit.Models.ProductionPlanning;

#nullable disable

namespace RhyoliteOrbit.Migrations
{
    /// <inheritdoc />
    public partial class initial114 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillOfMaterials");

            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "OperatorAssignments");

            migrationBuilder.DropTable(
                name: "QualityControls");

            migrationBuilder.DropTable(
                name: "Routings");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "SystemNumbers");

            migrationBuilder.DropColumn(
                name: "StaffStatusId",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "StaffStatusName",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "EmployeeBioDatas");

            migrationBuilder.DropColumn(
                name: "StatusName",
                table: "EmployeeBioDatas");

            migrationBuilder.AddColumn<string>(
                name: "SupervisorEmail",
                table: "WorkCenters",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SupervisorId",
                table: "WorkCenters",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "SupervisorName",
                table: "WorkCenters",
                type: "character varying(130)",
                maxLength: 130,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupervisorPhoneNo",
                table: "WorkCenters",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormatPattern",
                table: "SystemNumbers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Module",
                table: "SystemNumbers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPosted",
                table: "StudentAttendances",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Pin",
                table: "Staffs",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Staffs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "UnitOfMeasure",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ActualStartDate",
                table: "ProductionOrders",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ActualQuantity",
                table: "ProductionOrders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ActualEndDate",
                table: "ProductionOrders",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerDepartment",
                table: "ProductionOrders",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerEmail",
                table: "ProductionOrders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ManagerId",
                table: "ProductionOrders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ManagerName",
                table: "ProductionOrders",
                type: "character varying(130)",
                maxLength: 130,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerPhone",
                table: "ProductionOrders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductionOrderIdentifier",
                table: "ProductionOrders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductionScheduleId",
                table: "ProductionOrders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ProductionScheduleIdentifier",
                table: "ProductionOrders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "EmployeeBioDatas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BillOfMaterialItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    QuantityPerUnit = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_BillOfMaterialItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinalProductTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionOrderIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    TestDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    InspectorId = table.Column<Guid>(type: "uuid", nullable: false),
                    InspectorName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    QcChecklistId = table.Column<Guid>(type: "uuid", nullable: false),
                    QcChecklistName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Results = table.Column<List<InspectionResult>>(type: "jsonb", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_FinalProductTests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinishedGoods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FinishedGoodIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ProductionOrderIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ProductionOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    QuantityProduced = table.Column<decimal>(type: "numeric", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsQaApproved = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_FinishedGoods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InProcessInspections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionOrderIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    JobTicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobTicketIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    InspectionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    InspectorId = table.Column<Guid>(type: "uuid", nullable: false),
                    InspectorName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    QcChecklistId = table.Column<Guid>(type: "uuid", nullable: false),
                    QcChecklistName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Results = table.Column<List<InspectionResult>>(type: "jsonb", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_InProcessInspections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobTickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionOrderIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: false),
                    JobTicketIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    WorkCenterId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkCenterName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EstimatedDurationInHours = table.Column<decimal>(type: "numeric", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsMandatory = table.Column<bool>(type: "boolean", nullable: false),
                    SupervisorId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupervisorName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    SupervisorEmail = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    SupervisorDepartment = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    SupervisorPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    AssignedOperatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedOperatorName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
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
                    table.PrimaryKey("PK_JobTickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaterialConsumptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionOrderIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ConsumptionIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    WorkCenterId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkCenterName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Details = table.Column<List<MaterialConsumptionDetail>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_MaterialConsumptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NonConformance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InProcessInspectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    FinalProductTestingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    RootCause = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CorrectiveAction = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PreventiveAction = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ReportedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ReportedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportedByName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_NonConformance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionOrderIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ProductionScheduleIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    WorkCenterId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkCenterName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ScheduledQuantity = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
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
                    table.PrimaryKey("PK_ProductionSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QcChecklists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Items = table.Column<List<QcChecklistItem>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_QcChecklists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RawMaterials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Sku = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CurrentStock = table.Column<decimal>(type: "numeric", nullable: false),
                    CostingMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StandardCost = table.Column<decimal>(type: "numeric", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_RawMaterials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScrapAndWastes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionOrderIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ProductionOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScrapType = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "text", nullable: true),
                    Reason = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
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
                    table.PrimaryKey("PK_ScrapAndWastes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillOfMaterialItems");

            migrationBuilder.DropTable(
                name: "FinalProductTests");

            migrationBuilder.DropTable(
                name: "FinishedGoods");

            migrationBuilder.DropTable(
                name: "InProcessInspections");

            migrationBuilder.DropTable(
                name: "JobTickets");

            migrationBuilder.DropTable(
                name: "MaterialConsumptions");

            migrationBuilder.DropTable(
                name: "NonConformance");

            migrationBuilder.DropTable(
                name: "ProductionSchedules");

            migrationBuilder.DropTable(
                name: "QcChecklists");

            migrationBuilder.DropTable(
                name: "RawMaterials");

            migrationBuilder.DropTable(
                name: "ScrapAndWastes");

            migrationBuilder.DropColumn(
                name: "SupervisorEmail",
                table: "WorkCenters");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "WorkCenters");

            migrationBuilder.DropColumn(
                name: "SupervisorName",
                table: "WorkCenters");

            migrationBuilder.DropColumn(
                name: "SupervisorPhoneNo",
                table: "WorkCenters");

            migrationBuilder.DropColumn(
                name: "FormatPattern",
                table: "SystemNumbers");

            migrationBuilder.DropColumn(
                name: "Module",
                table: "SystemNumbers");

            migrationBuilder.DropColumn(
                name: "IsPosted",
                table: "StudentAttendances");

            migrationBuilder.DropColumn(
                name: "Pin",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "ManagerDepartment",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "ManagerEmail",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "ManagerName",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "ManagerPhone",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "ProductionOrderIdentifier",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "ProductionScheduleId",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "ProductionScheduleIdentifier",
                table: "ProductionOrders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EmployeeBioDatas");

            migrationBuilder.AddColumn<string>(
                name: "ModuleId",
                table: "SystemNumbers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StaffStatusId",
                table: "Staffs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "StaffStatusName",
                table: "Staffs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UnitOfMeasure",
                table: "Products",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Products",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ActualStartDate",
                table: "ProductionOrders",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<decimal>(
                name: "ActualQuantity",
                table: "ProductionOrders",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ActualEndDate",
                table: "ProductionOrders",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<Guid>(
                name: "StatusId",
                table: "EmployeeBioDatas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "StatusName",
                table: "EmployeeBioDatas",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BillOfMaterials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    CurrentStock = table.Column<decimal>(type: "numeric", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ReorderLevel = table.Column<decimal>(type: "numeric", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillOfMaterials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchNumber = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    ProductOrMaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductOrMaterialName = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperatorAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedTask = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EndTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    OperatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    WorkCenterId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkCenterName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatorAssignments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QualityControls",
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
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductName = table.Column<string>(type: "text", nullable: true),
                    ProductionOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    TestDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TestResult = table.Column<int>(type: "integer", nullable: false),
                    TestType = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityControls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Routings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    OperationNumber = table.Column<int>(type: "integer", nullable: false),
                    PreviousOperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductName = table.Column<string>(type: "text", nullable: true),
                    StandardTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    WorkCenterId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkCenterName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routings", x => x.Id);
                });
        }
    }
}
