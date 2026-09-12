using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RhyoliteOrbit.Models.Dms;
using RhyoliteOrbit.Models.GeneralLedger;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Models.Pos;
using RhyoliteOrbit.Models.ProductionPlanning;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Models.Shared;
using RhyoliteOrbit.Models.Wssd;

#nullable disable

namespace RhyoliteOrbit.Migrations
{
    /// <inheritdoc />
    public partial class initial1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbpAuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    ServiceName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    MethodName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Parameters = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    ReturnValue = table.Column<string>(type: "text", nullable: true),
                    ExecutionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ExecutionDuration = table.Column<int>(type: "integer", nullable: false),
                    ClientIpAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ClientName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    BrowserInfo = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ExceptionMessage = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Exception = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ImpersonatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    ImpersonatorTenantId = table.Column<int>(type: "integer", nullable: true),
                    CustomData = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpBackgroundJobs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobType = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    JobArgs = table.Column<string>(type: "character varying(1048576)", maxLength: 1048576, nullable: false),
                    TryCount = table.Column<short>(type: "smallint", nullable: false),
                    NextTryTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastTryTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsAbandoned = table.Column<bool>(type: "boolean", nullable: false),
                    Priority = table.Column<byte>(type: "smallint", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpBackgroundJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpDynamicProperties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropertyName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    InputType = table.Column<string>(type: "text", nullable: true),
                    Permission = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpDynamicProperties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpEditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
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
                    table.PrimaryKey("PK_AbpEditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpEntityChangeSets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BrowserInfo = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ClientIpAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ClientName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ExtensionData = table.Column<string>(type: "text", nullable: true),
                    ImpersonatorTenantId = table.Column<int>(type: "integer", nullable: true),
                    ImpersonatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    Reason = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpEntityChangeSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpLanguages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Icon = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_AbpLanguages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpLanguageTexts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    LanguageName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Source = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "text", maxLength: 67108864, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpLanguageTexts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationName = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: false),
                    Data = table.Column<string>(type: "character varying(1048576)", maxLength: 1048576, nullable: true),
                    DataTypeName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    EntityTypeName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    EntityTypeAssemblyQualifiedName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    EntityId = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    Severity = table.Column<byte>(type: "smallint", nullable: false),
                    UserIds = table.Column<string>(type: "character varying(131072)", maxLength: 131072, nullable: true),
                    ExcludedUserIds = table.Column<string>(type: "character varying(131072)", maxLength: 131072, nullable: true),
                    TenantIds = table.Column<string>(type: "character varying(131072)", maxLength: 131072, nullable: true),
                    TargetNotifiers = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpNotificationSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    NotificationName = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    EntityTypeName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    EntityTypeAssemblyQualifiedName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    EntityId = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    TargetNotifiers = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpNotificationSubscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpOrganizationUnitRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpOrganizationUnitRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpOrganizationUnits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<string>(type: "character varying(95)", maxLength: 95, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
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
                    table.PrimaryKey("PK_AbpOrganizationUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpOrganizationUnits_AbpOrganizationUnits_ParentId",
                        column: x => x.ParentId,
                        principalTable: "AbpOrganizationUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AbpTenantNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    NotificationName = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: false),
                    Data = table.Column<string>(type: "character varying(1048576)", maxLength: 1048576, nullable: true),
                    DataTypeName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    EntityTypeName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    EntityTypeAssemblyQualifiedName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    EntityId = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    Severity = table.Column<byte>(type: "smallint", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpTenantNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserAccounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    UserLinkId = table.Column<long>(type: "bigint", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
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
                    table.PrimaryKey("PK_AbpUserAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserLoginAttempts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    TenancyName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    UserNameOrEmailAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ClientIpAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ClientName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    BrowserInfo = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Result = table.Column<byte>(type: "smallint", nullable: false),
                    FailReason = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserLoginAttempts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TenantNotificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TargetNotifiers = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserOrganizationUnits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserOrganizationUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpUsers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AuthenticationSource = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    EmailAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Surname = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Password = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    EmailConfirmationCode = table.Column<string>(type: "character varying(328)", maxLength: 328, nullable: true),
                    PasswordResetCode = table.Column<string>(type: "character varying(328)", maxLength: 328, nullable: true),
                    LockoutEndDateUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false),
                    IsLockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    IsPhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    SecurityStamp = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    IsTwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsEmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedEmailAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpUsers_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AbpUsers_AbpUsers_DeleterUserId",
                        column: x => x.DeleterUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AbpUsers_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AbpWebhookEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WebhookName = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpWebhookEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpWebhookSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    WebhookUri = table.Column<string>(type: "text", nullable: false),
                    Secret = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Webhooks = table.Column<string>(type: "text", nullable: true),
                    Headers = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpWebhookSubscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AcademicYears",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BeginDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    Terms = table.Column<List<Term>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_AcademicYears", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountBalances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CreditBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    DebitBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    CreditForeign = table.Column<decimal>(type: "numeric", nullable: false),
                    DebitForeign = table.Column<decimal>(type: "numeric", nullable: false),
                    OuId = table.Column<Guid>(type: "uuid", nullable: false),
                    OuName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CurrencyCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    BuyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    SellRate = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_AccountBalances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountHeads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AccountHeadType = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_AccountHeads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountingPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LastClosedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AccountingPeriodStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AccountingPeriodEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                    Period = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_AccountingPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AllowanceTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AllowanceDays = table.Column<int>(type: "integer", nullable: false),
                    ExpenseAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpenseAccountName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    IsTaxable = table.Column<bool>(type: "boolean", nullable: false),
                    AllowanceRates = table.Column<List<AllowanceRate>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_AllowanceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: true),
                    Audience = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_Announcements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_AssessmentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssignedClasses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TermId = table.Column<Guid>(type: "uuid", nullable: false),
                    TermName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StaffId = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    Details = table.Column<List<AssignedClassDetail>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_AssignedClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BankId = table.Column<Guid>(type: "uuid", nullable: false),
                    BankName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BankBranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    BankBranchName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Address = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Location = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    AccountNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AccountType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CurrencyCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ContactPerson = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    ContactNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ChequeBookStart = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ChequeBookEnd = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DateOpened = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
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
                    table.PrimaryKey("PK_BankAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ContactPerson = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ContactNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BankCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    BankBranches = table.Column<List<BankBranch>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_Banks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BikTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ExpenseAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpenseAccountName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    IsTaxable = table.Column<bool>(type: "boolean", nullable: false),
                    BikRates = table.Column<List<BikRate>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_BikTypes", x => x.Id);
                });

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
                name: "BillPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TermId = table.Column<Guid>(type: "uuid", nullable: false),
                    TermName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    StudentName = table.Column<string>(type: "character varying(1200)", maxLength: 1200, nullable: true),
                    AmountPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    ModeOfPayment = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CurrencyCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CurrencyBuyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrencySellRate = table.Column<decimal>(type: "numeric", nullable: false),
                    ChequeNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BillId = table.Column<Guid>(type: "uuid", nullable: false),
                    BillNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ReceiptNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PaymentDescription = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsCreditMemo = table.Column<bool>(type: "boolean", nullable: false),
                    IsPosted = table.Column<bool>(type: "boolean", nullable: false),
                    DocumentUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
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
                    table.PrimaryKey("PK_BillPayments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BillSetups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TermId = table.Column<Guid>(type: "uuid", nullable: false),
                    TermName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BillTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    BillTypeName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TotalBillAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Details = table.Column<List<BillSetupDetail>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_BillSetups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BillTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_BillTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BonusAndOnetimeAllowances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    AllowanceTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllowanceTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsFixedAmount = table.Column<bool>(type: "boolean", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Percentage = table.Column<decimal>(type: "numeric", nullable: false),
                    IsTaxable = table.Column<bool>(type: "boolean", nullable: false),
                    IsSsf = table.Column<bool>(type: "boolean", nullable: false),
                    IsPf = table.Column<bool>(type: "boolean", nullable: false),
                    AllowanceDays = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_BonusAndOnetimeAllowances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookLendings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookTitle = table.Column<string>(type: "text", nullable: true),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentName = table.Column<string>(type: "text", nullable: true),
                    StudentClass = table.Column<string>(type: "text", nullable: true),
                    LendingDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ReturnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Remarks = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_BookLendings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OuId = table.Column<Guid>(type: "uuid", nullable: false),
                    OuName = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    BeginDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    BudgetDetails = table.Column<List<BudgetDetail>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_Budgets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessDates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ActualDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
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
                    table.PrimaryKey("PK_BusinessDates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CityName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PhoneNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    RegionOrState = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CompanyTin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    FirstMonthOfFiscalYear = table.Column<int>(type: "integer", nullable: false),
                    VatNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployerSocialSecurityNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PaySocialSecurityFund = table.Column<bool>(type: "boolean", nullable: false),
                    SocialSecurityFundEmployeeRate = table.Column<decimal>(type: "numeric", nullable: false),
                    SocialSecurityFundEmployerRate = table.Column<decimal>(type: "numeric", nullable: false),
                    PayProvidentFund = table.Column<bool>(type: "boolean", nullable: false),
                    ProvidentFundEmployeeRate = table.Column<decimal>(type: "numeric", nullable: false),
                    ProvidentFundEmployerRate = table.Column<decimal>(type: "numeric", nullable: false),
                    ProvidentFundTreatAsSocialSecurityFund = table.Column<bool>(type: "boolean", nullable: false),
                    ProvidentFundTaxEmployerPortion = table.Column<bool>(type: "boolean", nullable: false),
                    PaySecondProvidentOrOthers = table.Column<bool>(type: "boolean", nullable: false),
                    SecondProvidentEmployeeRate = table.Column<decimal>(type: "numeric", nullable: false),
                    SecondProvidentEmployerRate = table.Column<decimal>(type: "numeric", nullable: false),
                    TreatSecondProvidentAsSsf = table.Column<bool>(type: "boolean", nullable: false),
                    SecondProvidentTaxEmployerPortion = table.Column<bool>(type: "boolean", nullable: false),
                    OvertimePercentageOnBasicSalary = table.Column<bool>(type: "boolean", nullable: false),
                    OvertimePercentageOnDailyWage = table.Column<bool>(type: "boolean", nullable: false),
                    DefaultCurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_BusinessProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CancelledBills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TermId = table.Column<Guid>(type: "uuid", nullable: false),
                    TermName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BillNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BillDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CurrencyName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CurrencyCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    MinorName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BuyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    SellRate = table.Column<decimal>(type: "numeric", nullable: false),
                    BillAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    BillBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    StudentName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    BillStatus = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BillTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    BillTypeName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BillSetupId = table.Column<Guid>(type: "uuid", nullable: false),
                    BillSetupInfo = table.Column<BillSetupInfo>(type: "jsonb", nullable: true),
                    Details = table.Column<List<BillDetail>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_CancelledBills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CancelledPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TermId = table.Column<Guid>(type: "uuid", nullable: false),
                    TermName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    StudentName = table.Column<string>(type: "text", nullable: true),
                    AmountPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    ModeOfPayment = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CurrencyCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CurrencyBuyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrencySellRate = table.Column<decimal>(type: "numeric", nullable: false),
                    ChequeNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BillId = table.Column<Guid>(type: "uuid", nullable: false),
                    BillNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ReceiptNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PaymentDescription = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsCreditMemo = table.Column<bool>(type: "boolean", nullable: false),
                    IsPosted = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_CancelledPayments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClassStreams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_ClassStreams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clubs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    FoundedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
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
                    table.PrimaryKey("PK_Clubs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoaControls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    MinAccount = table.Column<int>(type: "integer", nullable: false),
                    MaxAccount = table.Column<int>(type: "integer", nullable: false),
                    AccountHeadId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountHeadType = table.Column<int>(type: "integer", nullable: false),
                    ParentControlId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_CoaControls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoaDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    AccountNo = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AccountControlId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountControlName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CurrencyCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CurrentBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrentForeignBalance = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_CoaDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_Collections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CostCenters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_CostCenters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    NumericIsoCode = table.Column<int>(type: "integer", nullable: false),
                    Nationality = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CouponValue = table.Column<decimal>(type: "numeric", nullable: false),
                    MinimumOrderLimit = table.Column<decimal>(type: "numeric", nullable: false),
                    MaximumOrderLimit = table.Column<decimal>(type: "numeric", nullable: false),
                    CouponIdentifier = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ValidTill = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UsageQuota = table.Column<int>(type: "integer", nullable: false),
                    UsageCount = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UsePercentage = table.Column<bool>(type: "boolean", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Assignments = table.Column<List<CouponAssignment>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreditAdjustments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditSaleId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdjustmentDate = table.Column<decimal>(type: "numeric", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Reason = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    AdjustmentType = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_CreditAdjustments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreditSaleRepaymentSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditSaleId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonthlyPayment = table.Column<decimal>(type: "numeric", nullable: false),
                    PrincipalPayment = table.Column<decimal>(type: "numeric", nullable: false),
                    InterestPayment = table.Column<decimal>(type: "numeric", nullable: false),
                    PrincipalBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    InterestPlusPrincipalBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Period = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_CreditSaleRepaymentSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreditSales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MerchantId = table.Column<Guid>(type: "uuid", nullable: false),
                    MerchantName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    PrincipalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    OutstandingAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    CreditTermId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditTermDays = table.Column<int>(type: "integer", nullable: false),
                    CreditStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    InterestRate = table.Column<decimal>(type: "numeric", nullable: false),
                    PenaltyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    NumberOfInstallments = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CommentsOrNotes = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ClientReference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_CreditSales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreditTerms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TermDays = table.Column<int>(type: "integer", nullable: false),
                    DefaultInterestRate = table.Column<decimal>(type: "numeric", nullable: false),
                    DefaultPenaltyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    GracePeriodDays = table.Column<int>(type: "integer", nullable: false),
                    DefaultNumberOfInstallments = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    MinAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    MinCreditScore = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_CreditTerms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MinorName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BuyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    SellRate = table.Column<decimal>(type: "numeric", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    Rates = table.Column<List<CurrencyRate>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Curricula",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: true),
                    GradeLevel = table.Column<int>(type: "integer", nullable: false),
                    EffectiveStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EffectiveEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentName = table.Column<string>(type: "text", nullable: true),
                    AcademicStandards = table.Column<string>(type: "text", nullable: true),
                    LearningObjectives = table.Column<string>(type: "text", nullable: true),
                    AssessmentMethods = table.Column<string>(type: "text", nullable: true),
                    RequiredResources = table.Column<string>(type: "text", nullable: true),
                    Syllabi = table.Column<List<Syllabus>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_Curricula", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerRanks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(205)", maxLength: 205, nullable: true),
                    MinimumTotalSpend = table.Column<decimal>(type: "numeric", nullable: false),
                    InStoreDiscount = table.Column<decimal>(type: "numeric", nullable: false),
                    OnlineDiscount = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
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
                    table.PrimaryKey("PK_CustomerRanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerIdentifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CustomerGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerGroupName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Balance = table.Column<decimal>(type: "numeric", nullable: false),
                    VatAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    VatAccountName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreditAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditAccountName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DebitAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    DebitAccountName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TinNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PhoneNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CurrencyCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreditLimit = table.Column<decimal>(type: "numeric", nullable: false),
                    IrsAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    IrsAccountName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RemittanceAccounts = table.Column<List<RemittanceAccount>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeductionTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LiabilityAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    LiabilityAccountName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Rates = table.Column<List<DeductionRate>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_DeductionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultGlNumbers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountPayablesBankAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    PayRollBankAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    SalesBankAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountsReceivableBankAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountsReceivableAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountsPayableAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    WithholdingTaxAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImprestCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditPurchasesAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdvanceFromCustomerAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    CovidTaxReceivedAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    CommunicationsServiceTaxReceivedAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    SalesRevenueAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    StockPurchasesAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentEarningsAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    RetainedEarningsAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    VatReceivedAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    VatRate = table.Column<decimal>(type: "numeric", nullable: false),
                    ImprestGlAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchasesBankAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdvanceToSupplierAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    CovidTaxPaidAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    CommunicationsServiceTaxPaidAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    GoodsInTransitAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    UnrealizedFxLossAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    UnrealizedFxGainAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    RealizedFxLossAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    RealizedFxGainAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    VatPaidAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    GetflReceivedAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    GetflPaidAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    NhilReceivedAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    NhilPaidAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    SocialSecurityEmployerPortionDefaultExpenseGl = table.Column<Guid>(type: "uuid", nullable: false),
                    SocialSecurityEmployeePortionDefaultExpenseGl = table.Column<Guid>(type: "uuid", nullable: false),
                    SocialSecurityEmployerPortionDefaultLiabilityGl = table.Column<Guid>(type: "uuid", nullable: false),
                    SocialSecurityEmployeePortionDefaultLiabilityGl = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_DefaultGlNumbers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrackingNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiptNo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    RecipientName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RecipientPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SecondaryContactNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DestinationAddress = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    RegionOrState = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DeliveryChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeliveryChannelName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    DeliveryZoneId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeliveryZoneName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    PickupPointId = table.Column<Guid>(type: "uuid", nullable: true),
                    PickupPointName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RiderId = table.Column<Guid>(type: "uuid", nullable: true),
                    RiderName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RiderPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EstimatedDeliveryTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DispatchedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeliveredTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeliveryConfirmationCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    DeliveryFee = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ExtraNotes = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
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
                    table.PrimaryKey("PK_Deliveries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryChannels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Description = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    IsLocal = table.Column<bool>(type: "boolean", nullable: false),
                    ZoneRates = table.Column<List<DeliveryZoneRate>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_DeliveryChannels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
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
                    table.PrimaryKey("PK_Department", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DisciplinaryActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentName = table.Column<string>(type: "text", nullable: true),
                    StudentClass = table.Column<string>(type: "text", nullable: true),
                    IncidentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Offense = table.Column<string>(type: "text", nullable: true),
                    ActionTaken = table.Column<string>(type: "text", nullable: true),
                    Remarks = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_DisciplinaryActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DmsComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentCommentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CommentText = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    IsEdited = table.Column<bool>(type: "boolean", nullable: false),
                    CommentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_DmsComments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DmsCustomAttributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AttributeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DataType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PossibleValues = table.Column<List<DropdownOption>>(type: "jsonb", nullable: true),
                    DefaultValue = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_DmsCustomAttributes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DmsCustomIdentifiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Prefix = table.Column<string>(type: "text", nullable: true),
                    Suffix = table.Column<string>(type: "text", nullable: true),
                    IncludeYear = table.Column<bool>(type: "boolean", nullable: false),
                    IncludeMonth = table.Column<bool>(type: "boolean", nullable: false),
                    PaddingLength = table.Column<int>(type: "integer", nullable: false),
                    CurrentSequence = table.Column<int>(type: "integer", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateName = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_DmsCustomIdentifiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DmsDocumentMetadataValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_DmsDocumentMetadataValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DmsDocumentTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_DmsDocumentTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DmsDocumentTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
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
                    table.PrimaryKey("PK_DmsDocumentTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DmsTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ColorCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
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
                    table.PrimaryKey("PK_DmsTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DmsWorkflowInstances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentStepIndex = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
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
                    table.PrimaryKey("PK_DmsWorkflowInstances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DmsWorkflowTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkflowInstanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedTo = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Comments = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
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
                    table.PrimaryKey("PK_DmsWorkflowTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DmsWorkflowTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    WorkflowTemplateSteps = table.Column<List<WorkflowTemplateStep>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_DmsWorkflowTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FolderId = table.Column<Guid>(type: "uuid", nullable: false),
                    FolderName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FileSize = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsStarred = table.Column<bool>(type: "boolean", nullable: false),
                    MimeType = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Checksum = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    VersionId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dormitories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_Dormitories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DormitoryRooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DormitoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomNumber = table.Column<string>(type: "text", nullable: true),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_DormitoryRooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ElectiveSubjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectName = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Level = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_ElectiveSubjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeAllowances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    AllowanceTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllowanceTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    IsTaxable = table.Column<bool>(type: "boolean", nullable: false),
                    IsMonthly = table.Column<bool>(type: "boolean", nullable: false),
                    Ssf = table.Column<bool>(type: "boolean", nullable: false),
                    ProvidentFund = table.Column<bool>(type: "boolean", nullable: false),
                    AllowanceDays = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeAllowances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeBenefitInKinds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    BenefitInKindTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    BenefitInKindTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeBenefitInKinds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeBioDatasx",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OtherName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeePhoto = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Gender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CityOrLocation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ResidenceAddress = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    DateAppointed = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SalaryGradeId = table.Column<Guid>(type: "uuid", nullable: false),
                    SalaryGradeName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    SalaryNotchId = table.Column<Guid>(type: "uuid", nullable: false),
                    SalaryNotchName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    RankId = table.Column<Guid>(type: "uuid", nullable: false),
                    RankName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    TaxIdentificationNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PersonalEmail = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    MaritalStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Religion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Nationality = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Hometown = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PrimaryPhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SecondaryPhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CompanyEmail = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NationalIdentificationNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    NationalHealthInsuranceNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DriverLicenseNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DriverLicenseIssueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DriverLicenseExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IdentityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IdentityNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Height = table.Column<decimal>(type: "numeric", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric", nullable: false),
                    Languages = table.Column<List<string>>(type: "jsonb", nullable: true),
                    Interests = table.Column<List<string>>(type: "jsonb", nullable: true),
                    HealthIssues = table.Column<List<string>>(type: "jsonb", nullable: true),
                    LeaveDaysEntitled = table.Column<int>(type: "integer", nullable: false),
                    MedicalExpensesLimit = table.Column<decimal>(type: "numeric", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeBioDatasx", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_EmployeeCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeDaysWorked",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    Days = table.Column<int>(type: "integer", nullable: false),
                    Hours = table.Column<decimal>(type: "numeric", nullable: false),
                    Minutes = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeDaysWorked", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeDeductions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    DeductionTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeductionTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    EmployerAmount = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeDeductions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeLoanRepaymentSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeLoanId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeLoanName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ScheduleDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    MonthlyPayment = table.Column<decimal>(type: "numeric", nullable: false),
                    PrincipalPayment = table.Column<decimal>(type: "numeric", nullable: false),
                    InterestPayment = table.Column<decimal>(type: "numeric", nullable: false),
                    PrincipalBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    InterestPlusPrincipalBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    Period = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeLoanRepaymentSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeLoans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LoanDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeductionStarts = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LoanTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NextDeduction = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    MonthlyDeduction = table.Column<decimal>(type: "numeric", nullable: false),
                    ChargeInterest = table.Column<bool>(type: "boolean", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    AnnualInterestRate = table.Column<decimal>(type: "numeric", nullable: false),
                    InterestType = table.Column<string>(type: "text", nullable: true),
                    InterestCharges = table.Column<decimal>(type: "numeric", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_EmployeeLoans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeOnetimeDeductions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    DeductionTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeductionTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsFixedAmount = table.Column<bool>(type: "boolean", nullable: false),
                    Percentage = table.Column<decimal>(type: "numeric", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeOnetimeDeductions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeRanks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_EmployeeRanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeReliefs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    ReliefTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReliefTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeReliefs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSalaryAdvances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    LoanDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LoanTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeSalaryAdvances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSalaryData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeSalaryGradeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeSalaryNotchId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeePhoto = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    SalaryType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PayType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BankId = table.Column<Guid>(type: "uuid", nullable: false),
                    BankName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    BankBranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    BankBranchName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    DailyHours = table.Column<decimal>(type: "numeric", nullable: false),
                    PreviousSalary = table.Column<decimal>(type: "numeric", nullable: false),
                    MonthlySalary = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CurrentHourlyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    AccountNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PaysGraduatedTax = table.Column<bool>(type: "boolean", nullable: false),
                    PaysWithHoldingTax = table.Column<bool>(type: "boolean", nullable: false),
                    WithHoldingTaxRate = table.Column<decimal>(type: "numeric", nullable: false),
                    VacationDaysPaid = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeSalaryData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "text", nullable: true),
                    EmployeeName = table.Column<string>(type: "text", nullable: true),
                    SocialSecurityFundEmployerContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    SocialSecurityNo = table.Column<string>(type: "text", nullable: true),
                    SocialSecurityFundEmployeeContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    SuperAnnuationEmployerContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    SuperAnnuationEmployeeContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    ProvidentFundEmployeeContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    ProvidentFundEmployerContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    SecondProvidentFundEmployeeContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    SecondProvidentFundEmployerContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    PaySocialSecurityFund = table.Column<bool>(type: "boolean", nullable: false),
                    PayProvidentFund = table.Column<bool>(type: "boolean", nullable: false),
                    PaySecondProvidentFund = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeSnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    EventDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: true),
                    Organizer = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExamSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Subject = table.Column<string>(type: "text", nullable: true),
                    GradeLevel = table.Column<string>(type: "text", nullable: true),
                    ExamDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Venue = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_ExamSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeesDescriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BillTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceivableAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceivableAccountName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Quantity = table.Column<int>(type: "integer", maxLength: 50, nullable: false),
                    Unit = table.Column<string>(type: "character varying(85)", maxLength: 85, nullable: true),
                    BillTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_FeesDescriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldTrips",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TripDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Venue = table.Column<string>(type: "text", nullable: true),
                    Organizer = table.Column<string>(type: "text", nullable: true),
                    NumberOfParticipants = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LeadTeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeadTeacherName = table.Column<string>(type: "text", nullable: true),
                    LeadTeacherDesignation = table.Column<string>(type: "text", nullable: true),
                    AdditionalSupervisors = table.Column<List<FieldTripSupervisor>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_FieldTrips", x => x.Id);
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
                name: "FinishedGoodAllocation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FinishedGoodId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerName = table.Column<string>(type: "text", nullable: true),
                    QuantityAllocated = table.Column<decimal>(type: "numeric", nullable: false),
                    ArInvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ArInvoiceNumber = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_FinishedGoodAllocation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinishedGoods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ProductionOrderIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ProductionOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UnitOfMeasureId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    QuantityProduced = table.Column<decimal>(type: "numeric", nullable: false),
                    InventoryCategory = table.Column<int>(type: "integer", nullable: false),
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
                name: "Folders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Path = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Folders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GiftCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Balance = table.Column<decimal>(type: "numeric", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RedeemedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
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
                    table.PrimaryKey("PK_GiftCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GradingSchemes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TermId = table.Column<Guid>(type: "uuid", nullable: false),
                    TermName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LevelId = table.Column<Guid>(type: "uuid", nullable: false),
                    LevelName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ExamPercentage = table.Column<decimal>(type: "numeric", nullable: false),
                    ClassPercentage = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_GradingSchemes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gratuities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MinYears = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxYears = table.Column<decimal>(type: "numeric", nullable: false),
                    Factor = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_Gratuities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImprestCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_ImprestCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Imprests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    ImprestCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImprestCategoryName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric", nullable: false),
                    DisbursementDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ExpectedRetirementDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreditAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditAccountName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ChequeNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ImprestNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DebitAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    DebitAccountName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ReferenceNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    OuId = table.Column<Guid>(type: "uuid", nullable: false),
                    OuName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CurrencyCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    BuyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    SellRate = table.Column<decimal>(type: "numeric", nullable: false),
                    PostedToGl = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_Imprests", x => x.Id);
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
                name: "InstalledPlugins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PluginId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Author = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DefaultConfig = table.Column<Dictionary<string, object>>(type: "jsonb", nullable: true),
                    Spec = table.Column<Dictionary<string, object>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_InstalledPlugins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Slug = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsService = table.Column<bool>(type: "boolean", nullable: false),
                    ParentCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductCount = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_InventoryCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    NumberOfTaggedProducts = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_ItemTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JournalDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JournalId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    OuId = table.Column<Guid>(type: "uuid", nullable: false),
                    OuName = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ChequeNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    LocalDebit = table.Column<decimal>(type: "numeric", nullable: false),
                    LocalCredit = table.Column<decimal>(type: "numeric", nullable: false),
                    ForeignDebit = table.Column<decimal>(type: "numeric", nullable: false),
                    ForeignCredit = table.Column<decimal>(type: "numeric", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TransactionSource = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsLocal = table.Column<bool>(type: "boolean", nullable: false),
                    IsReconciled = table.Column<bool>(type: "boolean", nullable: false),
                    IsOpeningBalance = table.Column<bool>(type: "boolean", nullable: false),
                    IsConsolidated = table.Column<bool>(type: "boolean", nullable: false),
                    DateConsolidated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CurrencyCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    BuyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    SellRate = table.Column<decimal>(type: "numeric", nullable: false),
                    ClosedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PettyCashRecipientId = table.Column<Guid>(type: "uuid", nullable: false),
                    PettyCashRecipientName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
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
                    table.PrimaryKey("PK_JournalDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Journals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchNumber = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    BatchDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TotalCredit = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalDebit = table.Column<decimal>(type: "numeric", nullable: false),
                    IsPosted = table.Column<bool>(type: "boolean", nullable: false),
                    IsMultiCurrency = table.Column<bool>(type: "boolean", nullable: false),
                    OuId = table.Column<Guid>(type: "uuid", nullable: false),
                    OuName = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    TransactionSource = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
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
                    table.PrimaryKey("PK_Journals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Laboratories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    EquipmentList = table.Column<List<LaboratoryEquipment>>(type: "jsonb", nullable: true),
                    ManagerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ManagerName = table.Column<string>(type: "text", nullable: true),
                    ContactPhone = table.Column<string>(type: "text", nullable: true),
                    ContactEmail = table.Column<string>(type: "text", nullable: true),
                    ManagerDepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ManagerDepartmentName = table.Column<string>(type: "text", nullable: true),
                    RequiredCertifications = table.Column<List<string>>(type: "jsonb", nullable: true),
                    LastSafetyInspection = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SafetyCertificationExpiry = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SafetyLevel = table.Column<int>(type: "integer", nullable: false),
                    BuildingName = table.Column<string>(type: "text", nullable: true),
                    Floor = table.Column<string>(type: "text", nullable: true),
                    RoomNumber = table.Column<string>(type: "text", nullable: true),
                    OperatingHours = table.Column<List<OperatingHour>>(type: "jsonb", nullable: true),
                    AccessRequirements = table.Column<List<string>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_Laboratories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryEquipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    SerialNumber = table.Column<string>(type: "text", nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Condition = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryEquipments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LessonPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Subject = table.Column<string>(type: "text", nullable: true),
                    GradeLevel = table.Column<string>(type: "text", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    PlanDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
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
                    table.PrimaryKey("PK_LessonPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Levels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_Levels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Libraries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_Libraries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LibraryBooks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Author = table.Column<string>(type: "text", nullable: true),
                    Isbn = table.Column<string>(type: "text", nullable: true),
                    PublishedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: true),
                    LibraryId = table.Column<Guid>(type: "uuid", nullable: false),
                    LibraryName = table.Column<Guid>(type: "uuid", nullable: false),
                    Keywords = table.Column<List<string>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_LibraryBooks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanAmortizationHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LoanDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DeductionStarts = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LoanTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LoanAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    MonthlyDeduction = table.Column<decimal>(type: "numeric", nullable: false),
                    ChargeInterest = table.Column<bool>(type: "boolean", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    AnnualInterestRate = table.Column<decimal>(type: "numeric", nullable: false),
                    InterestType = table.Column<string>(type: "text", nullable: true),
                    InterestCharges = table.Column<decimal>(type: "numeric", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_LoanAmortizationHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ApplyDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LoanAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    LoanTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LoanNotes = table.Column<string>(type: "text", nullable: true),
                    LoanStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LoanDuration = table.Column<int>(type: "integer", nullable: false),
                    DeductionStarts = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
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
                    table.PrimaryKey("PK_LoanApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ChargeInterest = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_LoanTypes", x => x.Id);
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
                name: "Merchants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    MerchantIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    BusinessDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BusinessClassification = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    ContactNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ContactEmail = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ContactPerson = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    TaxIdentificationNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BusinessRegistrationNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IdentityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IdentityCardNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ComplianceDocuments = table.Column<List<ComplianceDocument>>(type: "jsonb", nullable: true),
                    RemittanceAccounts = table.Column<List<RemittanceAccount>>(type: "jsonb", nullable: true),
                    BusinessSector = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CountryName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    RegionOrState = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    City = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Address = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    IsFeeChargeOnCustomer = table.Column<bool>(type: "boolean", nullable: false),
                    RhyoliteCbsClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    RevenueAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    RevenueAccountNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PayoutAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreferredPaymentChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreferredPaymentChannelName = table.Column<Guid>(type: "uuid", nullable: false),
                    PayoutAccountNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AverageRating = table.Column<int>(type: "integer", nullable: false),
                    Pin = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    WebsiteUrl = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    DateEstablished = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    HasSubAccountEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsDirectDebitEnabled = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_Merchants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageContent = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    MessageSubject = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Alias = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_MessageTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MiscellaneousPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    PaymentTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentTypeName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsPosted = table.Column<bool>(type: "boolean", nullable: false),
                    RevenueLedgerAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    OuId = table.Column<Guid>(type: "uuid", nullable: false),
                    OuName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ReceiptNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CurrencyCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
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
                    table.PrimaryKey("PK_MiscellaneousPayments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyAllowanceHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AllowanceTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllowanceTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Taxable = table.Column<bool>(type: "boolean", nullable: false),
                    Ssf = table.Column<bool>(type: "boolean", nullable: false),
                    ProvidentFund = table.Column<bool>(type: "boolean", nullable: false),
                    AllowanceDays = table.Column<int>(type: "integer", nullable: false),
                    IsMonthly = table.Column<bool>(type: "boolean", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BuyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    SellRate = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyAllowanceHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyAllowances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AllowanceTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllowanceTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    IsTaxable = table.Column<bool>(type: "boolean", nullable: false),
                    Ssf = table.Column<bool>(type: "boolean", nullable: false),
                    ProvidentFund = table.Column<bool>(type: "boolean", nullable: false),
                    AllowanceDays = table.Column<int>(type: "integer", nullable: false),
                    IsMonthly = table.Column<bool>(type: "boolean", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BuyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    SellRate = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyAllowances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyBenefitsInKindHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BenefitInKindTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    BenefitInKindTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyBenefitsInKindHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyBenefitsInKinds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BenefitInKindTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    BenefitInKindTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyBenefitsInKinds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyBonuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EarningTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EarningTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsSpecialEarning = table.Column<bool>(type: "boolean", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyBonuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyBonusHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EarningTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EarningTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsSpecialEarning = table.Column<bool>(type: "boolean", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyBonusHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyCumulativeDeductionHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyCumulativeDeductionHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyCumulativeDeductions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyCumulativeDeductions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyDeductionHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DeductionTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeductionTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    EmployerAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyDeductionHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyDeductions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DeductionTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeductionTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    EmployerAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyDeductions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyIrsTaxes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyIrsTaxes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyIrsTaxHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyIrsTaxHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyLoanDeductionHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeLoanId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeLoanName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    LoanTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LoanPeriod = table.Column<int>(type: "integer", nullable: false),
                    LoanAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    RepayAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    ClosingBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    OpeningBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    ScheduleDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    MonthlyPayment = table.Column<decimal>(type: "numeric", nullable: false),
                    PrincipalPayment = table.Column<decimal>(type: "numeric", nullable: false),
                    InterestPayment = table.Column<decimal>(type: "numeric", nullable: false),
                    PrincipalBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    InterestPlusPrincipalBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    EmployeeLoanRepaymentScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyLoanDeductionHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyLoanDeductions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeLoanId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeLoanName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    LoanTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LoanPeriod = table.Column<int>(type: "integer", nullable: false),
                    LoanAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    RepayAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    ClosingBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    OpeningBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    ScheduleDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    MonthlyPayment = table.Column<decimal>(type: "numeric", nullable: false),
                    PrincipalPayment = table.Column<decimal>(type: "numeric", nullable: false),
                    InterestPayment = table.Column<decimal>(type: "numeric", nullable: false),
                    PrincipalBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    InterestPlusPrincipalBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    EmployeeLoanRepaymentScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyLoanDeductions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyOnetimeDeductionHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyOnetimeDeductionHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyOnetimeDeductions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyOnetimeDeductions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyOvertimeHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OvertimeTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    OvertimeTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    OvertimeHours = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyOvertimeHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyOvertimes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OvertimeTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    OvertimeTypeName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    OvertimeHours = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyOvertimes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyPfDeductionHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyPfDeductionHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyPfDeductions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyPfDeductions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyReliefHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReliefTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReliefTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyReliefHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyReliefs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReliefTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReliefTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_MonthlyReliefs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlySalaryAdvanceHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    LoanTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LoanDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
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
                    table.PrimaryKey("PK_MonthlySalaryAdvanceHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlySalaryAdvances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    LoanTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LoanDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
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
                    table.PrimaryKey("PK_MonthlySalaryAdvances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlySecPfDeductionHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_MonthlySecPfDeductionHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlySecPfDeductions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_MonthlySecPfDeductions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlySsnitDeductionHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_MonthlySsnitDeductionHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlySsnitDeductions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_MonthlySsnitDeductions", x => x.Id);
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
                name: "NotificationAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Alias = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SmtpCredentials = table.Column<SmtpCredential>(type: "jsonb", nullable: true),
                    TelegramApiCredentials = table.Column<TelegramApiCredential>(type: "jsonb", nullable: true),
                    SmsApiCredentials = table.Column<SmsApiCredential>(type: "jsonb", nullable: true),
                    WhatsAppApiCredentials = table.Column<WhatsAppApiCredential>(type: "jsonb", nullable: true),
                    IsEmailAccountActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsTelegramAccountActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsWhatsAppAccountActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsSmsAccountActive = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_NotificationAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Operations",
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
                    PriorityLevel = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Tag = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_Operations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiptNo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CustomOrderNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OrderDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Source = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SoldBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CustomerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomerPhoneNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CustomerEmail = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OrderSubtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    OrderTax = table.Column<decimal>(type: "numeric", nullable: false),
                    OrderDiscount = table.Column<decimal>(type: "numeric", nullable: false),
                    ShippingCost = table.Column<decimal>(type: "numeric", nullable: false),
                    OrderTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentStatus = table.Column<int>(type: "integer", nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PaymentReference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OrderStatus = table.Column<int>(type: "integer", nullable: false),
                    HasDelivery = table.Column<bool>(type: "boolean", nullable: false),
                    DeliveryAddress = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DeliveryChannelId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeliveryChannelName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OrderItems = table.Column<List<OrderItem>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OvertimeTimeSheets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    JobDescription = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    OvertimeTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    OvertimeTypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OvertimeHours = table.Column<decimal>(type: "numeric", nullable: false),
                    OvertimeMinutes = table.Column<decimal>(type: "numeric", nullable: false),
                    IsTaxable = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_OvertimeTimeSheets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OvertimeTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsTaxable = table.Column<bool>(type: "boolean", nullable: false),
                    Rates = table.Column<List<OvertimeRate>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_OvertimeTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParentTeacherMeetings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MeetingDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Venue = table.Column<string>(type: "text", nullable: true),
                    Agenda = table.Column<string>(type: "text", nullable: true),
                    Minutes = table.Column<string>(type: "text", nullable: true),
                    Attendance = table.Column<List<PtaAttendance>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_ParentTeacherMeetings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartnerCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PartnerType = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_PartnerCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PastQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TermId = table.Column<Guid>(type: "uuid", nullable: false),
                    TermName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Caption = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FileUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
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
                    table.PrimaryKey("PK_PastQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayCalendars",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Details = table.Column<List<PayCalendarDetail>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_PayCalendars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymasterHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeSsfDeduction = table.Column<decimal>(type: "numeric", nullable: false),
                    EmployerSsfDeduction = table.Column<decimal>(type: "numeric", nullable: false),
                    BasicSalary = table.Column<decimal>(type: "numeric", nullable: false),
                    IrsTax = table.Column<decimal>(type: "numeric", nullable: false),
                    NetSalary = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxableAllowance = table.Column<decimal>(type: "numeric", nullable: false),
                    NonTaxableAllowance = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxableOverTime = table.Column<decimal>(type: "numeric", nullable: false),
                    OverTimeTax = table.Column<decimal>(type: "numeric", nullable: false),
                    NonTaxableOverTime = table.Column<decimal>(type: "numeric", nullable: false),
                    VoluntaryDeduction = table.Column<decimal>(type: "numeric", nullable: false),
                    DaysWorked = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxRelief = table.Column<decimal>(type: "numeric", nullable: false),
                    BenefitsInKind = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    EmployeeProvidentFundContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    EmployerProvidentFundContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    EmployeeSecProvidentFundContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    EmployerSecProvidentFundContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxableSpecialEarning = table.Column<decimal>(type: "numeric", nullable: false),
                    NonTaxableSpecialEarning = table.Column<decimal>(type: "numeric", nullable: false),
                    OneTimeDeduction = table.Column<decimal>(type: "numeric", nullable: false),
                    SpecialEarning = table.Column<decimal>(type: "numeric", nullable: false),
                    BonuxTax = table.Column<decimal>(type: "numeric", nullable: false),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BuyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    SellRate = table.Column<decimal>(type: "numeric", nullable: false),
                    HoursWorked = table.Column<decimal>(type: "numeric", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    WithHeld = table.Column<bool>(type: "boolean", nullable: false),
                    WithHeldRate = table.Column<decimal>(type: "numeric", nullable: false),
                    OvertimeExcessTax = table.Column<decimal>(type: "numeric", nullable: false),
                    ContributionType = table.Column<string>(type: "text", nullable: true),
                    EmployeeContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    EmployerContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    LoanDeduction = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxableIncome = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalSalaryAdvance = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_PaymasterHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Paymasters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    EmployeeDepartment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmployeeSsfDeduction = table.Column<decimal>(type: "numeric", nullable: false),
                    EmployerSsfDeduction = table.Column<decimal>(type: "numeric", nullable: false),
                    BasicSalary = table.Column<decimal>(type: "numeric", nullable: false),
                    IrsTax = table.Column<decimal>(type: "numeric", nullable: false),
                    NetSalary = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxableAllowance = table.Column<decimal>(type: "numeric", nullable: false),
                    NonTaxableAllowance = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxableOverTime = table.Column<decimal>(type: "numeric", nullable: false),
                    OverTimeTax = table.Column<decimal>(type: "numeric", nullable: false),
                    NonTaxableOverTime = table.Column<decimal>(type: "numeric", nullable: false),
                    VoluntaryDeduction = table.Column<decimal>(type: "numeric", nullable: false),
                    DaysWorked = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxRelief = table.Column<decimal>(type: "numeric", nullable: false),
                    BenefitsInKind = table.Column<decimal>(type: "numeric", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    EmployeeProvidentFundContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    EmployerProvidentFundContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    EmployeeSecProvidentFundContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    EmployerSecProvidentFundContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxableSpecialEarning = table.Column<decimal>(type: "numeric", nullable: false),
                    NonTaxableSpecialEarning = table.Column<decimal>(type: "numeric", nullable: false),
                    OneTimeDeduction = table.Column<decimal>(type: "numeric", nullable: false),
                    SpecialEarning = table.Column<decimal>(type: "numeric", nullable: false),
                    BonuxTax = table.Column<decimal>(type: "numeric", nullable: false),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyName = table.Column<string>(type: "text", nullable: true),
                    BuyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    SellRate = table.Column<decimal>(type: "numeric", nullable: false),
                    HoursWorked = table.Column<decimal>(type: "numeric", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    WithHeld = table.Column<bool>(type: "boolean", nullable: false),
                    WithHeldRate = table.Column<decimal>(type: "numeric", nullable: false),
                    OvertimeExcessTax = table.Column<decimal>(type: "numeric", nullable: false),
                    ContributionType = table.Column<string>(type: "text", nullable: true),
                    EmployeeContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    EmployerContribution = table.Column<decimal>(type: "numeric", nullable: false),
                    LoanDeduction = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxableIncome = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalSalaryAdvance = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_Paymasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditSaleId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsCreditSale = table.Column<bool>(type: "boolean", nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomerPhoneNo = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    Status = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    InvoiceAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    InterestComponent = table.Column<decimal>(type: "numeric", nullable: false),
                    PenaltyComponent = table.Column<decimal>(type: "numeric", nullable: false),
                    PrincipalComponent = table.Column<decimal>(type: "numeric", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric", nullable: false),
                    ReceiptNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PaymentType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PaymentChannel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
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
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TargetAudience = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_PaymentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayrollPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_PayrollPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PettyCashRecipients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    PhoneNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_PettyCashRecipients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PickupPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FriendlyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SystemName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    GhanaPostGps = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GpsCoordinates = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_PickupPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PluginActivityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PluginId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActivityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Details = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Source = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    StackTrace = table.Column<string>(type: "character varying(650)", maxLength: 650, nullable: true),
                    ExecutionTimeMs = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_PluginActivityLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PosBranches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CountryName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CountryCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    RegionOrState = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    PhoneNo = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    BranchManager = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RevenueAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    RevenueAccountNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GeoCoordinates = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_PosBranches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PosCustomers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RankId = table.Column<Guid>(type: "uuid", nullable: false),
                    RankName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TotalAmountSpent = table.Column<decimal>(type: "numeric", nullable: false),
                    NumberOfVisits = table.Column<int>(type: "integer", nullable: false),
                    LoyaltyPoints = table.Column<int>(type: "integer", nullable: false),
                    DefaultRegion = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    CreditLimit = table.Column<decimal>(type: "numeric", nullable: false),
                    WalletBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    DefaultCreditTermInDays = table.Column<int>(type: "integer", nullable: false),
                    IsCreditAllowed = table.Column<bool>(type: "boolean", nullable: false),
                    BlockedFromCredit = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_PosCustomers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PosInventoryItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ShortDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FullDescription = table.Column<List<string>>(type: "jsonb", nullable: true),
                    AdminComment = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    MerchantId = table.Column<Guid>(type: "uuid", nullable: false),
                    MerchantName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Sku = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Images = table.Column<List<string>>(type: "jsonb", nullable: true),
                    StockQuantity = table.Column<int>(type: "integer", nullable: false),
                    OldSellingPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    CostPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric", nullable: false),
                    IsFreeShipping = table.Column<bool>(type: "boolean", nullable: false),
                    ShipSeparately = table.Column<bool>(type: "boolean", nullable: false),
                    UomCodeId = table.Column<Guid>(type: "uuid", nullable: false),
                    UomName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CategorySlug = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Tags = table.Column<List<string>>(type: "jsonb", nullable: true),
                    Variants = table.Column<List<PosInventoryItemVariant>>(type: "jsonb", nullable: true),
                    MinimumCartQuantity = table.Column<int>(type: "integer", nullable: false),
                    MaximumCartQuantity = table.Column<int>(type: "integer", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ShelfLocation = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    AllowCustomerReviews = table.Column<bool>(type: "boolean", nullable: false),
                    AvailableStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AvailableEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    MarkAsNew = table.Column<bool>(type: "boolean", nullable: false),
                    DisableBuyButton = table.Column<bool>(type: "boolean", nullable: false),
                    DisableWishlistButton = table.Column<bool>(type: "boolean", nullable: false),
                    AvailableForPreOrder = table.Column<bool>(type: "boolean", nullable: false),
                    IsTaxExempt = table.Column<bool>(type: "boolean", nullable: false),
                    IsNotReturnable = table.Column<bool>(type: "boolean", nullable: false),
                    IsRental = table.Column<bool>(type: "boolean", nullable: false),
                    IsService = table.Column<bool>(type: "boolean", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    ReviewsCount = table.Column<int>(type: "integer", nullable: false),
                    ProductAttributes = table.Column<List<ProductAttribute>>(type: "jsonb", nullable: true),
                    Specifications = table.Column<List<PosInventoryItemSpecification>>(type: "jsonb", nullable: true),
                    BulkPricing = table.Column<List<PosInventoryItemBulkPricing>>(type: "jsonb", nullable: true),
                    MetaTitle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    MetaKeywords = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    MetaDescription = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    LeadTime = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_PosInventoryItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Tag = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ProductionOrderIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PlannedQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    ActualQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    PlannedStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ActualStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PlannedEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ActualEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EstimatedBudget = table.Column<decimal>(type: "numeric", nullable: false),
                    PriorityLevel = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ManagerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ManagerName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    ManagerEmail = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ManagerDepartment = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ManagerPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Comments = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
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
                    table.PrimaryKey("PK_ProductionOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionWorkflows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Steps = table.Column<List<ProcessStep>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_ProductionWorkflows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomerPhoneNo = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    CustomerEmail = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReviewText = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    ReplyText = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_ProductReviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UnitOfMeasureId = table.Column<Guid>(type: "uuid", nullable: false),
                    MinimumOrderQuantity = table.Column<int>(type: "integer", nullable: false),
                    StandardCost = table.Column<decimal>(type: "numeric", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromotionHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PromotedFrom = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentClassName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PromotedTo = table.Column<Guid>(type: "uuid", nullable: false),
                    DestinationClassName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DatePromoted = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    StudentName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
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
                    table.PrimaryKey("PK_PromotionHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PvRetentions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    VoucherHeaderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditAccountName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    IsFixedAmount = table.Column<bool>(type: "boolean", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_PvRetentions", x => x.Id);
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
                    UnitOfMeasure = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UnitOfMeasureId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentStock = table.Column<decimal>(type: "numeric", nullable: false),
                    CostingMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StandardCost = table.Column<decimal>(type: "numeric", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    InventoryCategory = table.Column<int>(type: "integer", nullable: false),
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
                name: "RefundAndReturns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestNo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiptNo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CustomerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomerPhoneNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    RestockItems = table.Column<bool>(type: "boolean", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    ApprovedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ProcessedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AdminNotes = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Items = table.Column<List<RefundItemDetail>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_RefundAndReturns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportDownloads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReportKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReportOptions = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RequestedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AccountSource = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReportFilePath = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PublicAccessUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    ReportServiceBaseUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
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
                    table.PrimaryKey("PK_ReportDownloads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResultsUploads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SubjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TermId = table.Column<Guid>(type: "uuid", nullable: false),
                    TermName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ResultTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResultTypeName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TotalMarks = table.Column<decimal>(type: "numeric", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    StudentName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    MarksObtained = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_ResultsUploads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResultTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Percentage = table.Column<decimal>(type: "numeric", nullable: false),
                    LevelId = table.Column<Guid>(type: "uuid", nullable: false),
                    LevelName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_ResultTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalaryAdvanceApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    ApplyDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LoanAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    LoanTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanTypeName = table.Column<Guid>(type: "uuid", maxLength: 100, nullable: false),
                    Reasons = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    LoanStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_SalaryAdvanceApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalaryGrades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NumericalOrder = table.Column<int>(type: "integer", nullable: false),
                    SalaryNotches = table.Column<List<SalaryNotch>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_SalaryGrades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalaryIncrementHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    CurrentSalary = table.Column<decimal>(type: "numeric", nullable: false),
                    PreviousSalary = table.Column<decimal>(type: "numeric", nullable: false),
                    IncrementAmount = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_SalaryIncrementHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesAgents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MerchantId = table.Column<Guid>(type: "uuid", maxLength: 15, nullable: false),
                    MerchantName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_SalesAgents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchClasses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LevelId = table.Column<Guid>(type: "uuid", nullable: false),
                    LevelName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ClassName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StreamId = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_SchClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scholarships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentName = table.Column<string>(type: "text", nullable: true),
                    StudentClass = table.Column<string>(type: "text", nullable: true),
                    ScholarshipType = table.Column<string>(type: "text", nullable: true),
                    AwardDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Sponsor = table.Column<string>(type: "text", nullable: true),
                    Remarks = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_Scholarships", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CurrentAcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentAcademicYearName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CurrentTermId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentTermName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SecondaryEmailAddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RegionOrState = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PrimaryPhoneNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SchoolHead = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SchoolLogoUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    SecondaryPhoneNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    AssistantSchoolHead = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Website = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PostalAddress = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    District = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_SchoolProfiles", x => x.Id);
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
                    UnitOfMeasure = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UnitOfMeasureId = table.Column<Guid>(type: "uuid", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "ShopCarts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemImageUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    ItemName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Sku = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    MinOrderQuantity = table.Column<int>(type: "integer", nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_ShopCarts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Siblings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SiblingClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    SiblingClass = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SiblingId = table.Column<Guid>(type: "uuid", nullable: false),
                    SiblingIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SiblingFullName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    StudentName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
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
                    table.PrimaryKey("PK_Siblings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpecialNeeds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentName = table.Column<string>(type: "text", nullable: true),
                    StudentClass = table.Column<string>(type: "text", nullable: true),
                    NeedType = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IdentifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
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
                    table.PrimaryKey("PK_SpecialNeeds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SportsCompetitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Venue = table.Column<string>(type: "text", nullable: true),
                    Organizer = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: true),
                    RegistrationFee = table.Column<decimal>(type: "numeric", nullable: false),
                    PrizeMoney = table.Column<decimal>(type: "numeric", nullable: false),
                    DrugTestingRequired = table.Column<bool>(type: "boolean", nullable: false),
                    ProhibitedItems = table.Column<List<string>>(type: "jsonb", nullable: true),
                    RulesAndRegulations = table.Column<List<string>>(type: "jsonb", nullable: true),
                    SafetyRequirements = table.Column<List<string>>(type: "jsonb", nullable: true),
                    Schedule = table.Column<List<SportsCompetitionSchedule>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_SportsCompetitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SportsTeams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CoachOrManager = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: true),
                    FoundedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Nickname = table.Column<string>(type: "text", nullable: true),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    AssistantCoaches = table.Column<List<string>>(type: "jsonb", nullable: true),
                    CaptainId = table.Column<Guid>(type: "uuid", nullable: false),
                    CaptainName = table.Column<string>(type: "text", nullable: true),
                    CaptainClass = table.Column<string>(type: "text", nullable: true),
                    AgeGroup = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    GamesPlayed = table.Column<int>(type: "integer", nullable: false),
                    Wins = table.Column<int>(type: "integer", nullable: false),
                    Losses = table.Column<int>(type: "integer", nullable: false),
                    Draws = table.Column<int>(type: "integer", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    GoalsFor = table.Column<int>(type: "integer", nullable: false),
                    GoalsAgainst = table.Column<int>(type: "integer", nullable: false),
                    CurrentRanking = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_SportsTeams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StaffDesignations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_StaffDesignations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StaffMeetings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Agenda = table.Column<string>(type: "text", nullable: true),
                    MeetingDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Venue = table.Column<string>(type: "text", nullable: true),
                    Organizer = table.Column<string>(type: "text", nullable: true),
                    Minutes = table.Column<string>(type: "text", nullable: true),
                    Attendance = table.Column<List<StaffAttendance>>(type: "jsonb", nullable: true),
                    IsMandatory = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_StaffMeetings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Staffs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SocialSecurityNumber = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    DateEmployed = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsTeachingStaff = table.Column<bool>(type: "boolean", nullable: false),
                    LastName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    OtherName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    HomeAddress = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CityOrLocation = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    MaritalStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PrimaryPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SecondaryPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmailAddress = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    EmergencyContactPerson = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EmergencyContactPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Nationality = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Religion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Pin = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
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
                    table.PrimaryKey("PK_Staffs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockReceipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiptNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    StockTransferId = table.Column<Guid>(type: "uuid", nullable: true),
                    TransferNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DateReceived = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ReceivingBranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceivingBranchName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DispatchingBranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    DispatchingBranchName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AutoUpdateInventory = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ReceivedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    Remarks = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TotalValue = table.Column<decimal>(type: "numeric", nullable: false),
                    ReceiptDetails = table.Column<List<StockReceiptDetail>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_StockReceipts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockTransfers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransferNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FromBranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromBranchName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ToBranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToBranchName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DispatchedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ReceivedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    Notes = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TotalValue = table.Column<decimal>(type: "numeric", nullable: false),
                    Details = table.Column<List<StockTransferDetail>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_StockTransfers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentAttendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AttendanceDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    NoPresent = table.Column<int>(type: "integer", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TermId = table.Column<Guid>(type: "uuid", nullable: false),
                    TermName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    IsPosted = table.Column<bool>(type: "boolean", nullable: false),
                    Details = table.Column<List<StudentAttendanceDetail>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_StudentAttendances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentAwards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentName = table.Column<string>(type: "text", nullable: true),
                    StudentClass = table.Column<string>(type: "text", nullable: true),
                    AwardName = table.Column<string>(type: "text", nullable: true),
                    AwardDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Issuer = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_StudentAwards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentBehaviorRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TermId = table.Column<Guid>(type: "uuid", nullable: false),
                    TermName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    StudentName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GeneralRemarks = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ConductOrAttitudeRemarks = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
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
                    table.PrimaryKey("PK_StudentBehaviorRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentBillReceipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TermId = table.Column<Guid>(type: "uuid", nullable: false),
                    TermName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ReceivedFrom = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    AmountPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    AmountInWords = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Reason = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BalanceDue = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ReceiptNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    StudentName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_StudentBillReceipts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentBillReportHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BillSetupId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TermId = table.Column<Guid>(type: "uuid", nullable: false),
                    TermName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ClassName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    BillTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    BillDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    BillNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DocumentUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
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
                    table.PrimaryKey("PK_StudentBillReportHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentBills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TermId = table.Column<Guid>(type: "uuid", nullable: false),
                    TermName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BillNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BillDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CurrencyName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CurrencyCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    MinorName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    BuyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    SellRate = table.Column<decimal>(type: "numeric", nullable: false),
                    BillAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    BillBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    StudentName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    BillStatus = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BillTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    BillTypeName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BillSetupId = table.Column<Guid>(type: "uuid", nullable: false),
                    BillSetupInfo = table.Column<BillSetupInfo>(type: "jsonb", nullable: true),
                    Details = table.Column<List<BillDetail>>(type: "jsonb", nullable: true),
                    DocumentUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
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
                    table.PrimaryKey("PK_StudentBills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentClubMemberships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentName = table.Column<string>(type: "text", nullable: true),
                    StudentClass = table.Column<string>(type: "text", nullable: true),
                    ClubId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClubName = table.Column<Guid>(type: "uuid", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_StudentClubMemberships", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentCounselings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentName = table.Column<string>(type: "text", nullable: true),
                    StudentClass = table.Column<string>(type: "text", nullable: true),
                    CounselingDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CounselorName = table.Column<string>(type: "text", nullable: true),
                    Topic = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_StudentCounselings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentDormitoryAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentName = table.Column<string>(type: "text", nullable: true),
                    StudentClass = table.Column<string>(type: "text", nullable: true),
                    DormitoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    DormitoryName = table.Column<string>(type: "text", nullable: true),
                    DormitoryRoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    DormitoryRoomNumber = table.Column<string>(type: "text", nullable: true),
                    AssignmentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Remarks = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_StudentDormitoryAssignments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentHealthRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentName = table.Column<string>(type: "text", nullable: true),
                    StudentClass = table.Column<string>(type: "text", nullable: true),
                    RecordDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    HealthIssue = table.Column<string>(type: "text", nullable: true),
                    Treatment = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_StudentHealthRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    LastName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    MiddleName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    NationalityId = table.Column<Guid>(type: "uuid", nullable: false),
                    NationalityName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EnrollmentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Gender = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Religion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    HomeAddress = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CityOrLocation = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    EnrollmentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BiometricIdentifier = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Pin = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    GuardianList = table.Column<List<Guardian>>(type: "jsonb", nullable: true),
                    IsAlumni = table.Column<bool>(type: "boolean", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
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
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentStatements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    StudentIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Statement = table.Column<List<Statement>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_StudentStatements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubjectCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_SubjectCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubjectRemarks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MinimumMarks = table.Column<decimal>(type: "numeric", nullable: false),
                    MaximumMarks = table.Column<decimal>(type: "numeric", nullable: false),
                    Remarks = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
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
                    table.PrimaryKey("PK_SubjectRemarks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SubjectCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectCategoryName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cycle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Reference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PaymentMethod = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Channel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsDirectDebitPayment = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Network = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
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
                    table.PrimaryKey("PK_SubscriptionPayments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionRenewalHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionPaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Combo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Country = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Cycle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Reference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CustomerMsisdn = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IsDirectDebitRenewal = table.Column<bool>(type: "boolean", nullable: false),
                    Channel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Network = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PaymentMethod = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RenewalStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RenewalEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    SelectedModules = table.Column<List<int>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_SubscriptionRenewalHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierIdentifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SupplierGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierGroupName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Balance = table.Column<decimal>(type: "numeric", nullable: false),
                    VatAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    VatAccountName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreditAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditAccountName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DebitAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    DebitAccountName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TinNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PhoneNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CurrencyCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreditLimit = table.Column<decimal>(type: "numeric", nullable: false),
                    IrsAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    IrsAccountName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SupplierContacts = table.Column<List<SupplierContact>>(type: "jsonb", nullable: true),
                    SupplierAddresses = table.Column<List<SupplierAddress>>(type: "jsonb", nullable: true),
                    RemittanceAccounts = table.Column<List<RemittanceAccount>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Syllabi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CurriculumId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectName = table.Column<string>(type: "text", nullable: true),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherName = table.Column<string>(type: "text", nullable: true),
                    CourseObjectives = table.Column<string>(type: "text", nullable: true),
                    GradingPolicy = table.Column<string>(type: "text", nullable: true),
                    RequiredMaterials = table.Column<string>(type: "text", nullable: true),
                    ClassPolicies = table.Column<string>(type: "text", nullable: true),
                    WeeklySchedule = table.Column<string>(type: "text", nullable: true),
                    AcademicTerm = table.Column<string>(type: "text", nullable: true),
                    AcademicYear = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Syllabi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemNumbers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    ItemName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Prefix = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Suffix = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FormatPattern = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Padding = table.Column<int>(type: "integer", nullable: false),
                    LastNo = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_SystemNumbers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSetting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MailHostName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsSslEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    PortNumber = table.Column<int>(type: "integer", nullable: false),
                    PrimaryEmailAddress = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    EmailPassword = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DefaultCurrencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    SendBillPaymentEmailNotification = table.Column<bool>(type: "boolean", nullable: false),
                    SendBillPaymentSmsNotification = table.Column<bool>(type: "boolean", nullable: false),
                    BillPaymentEmailTemplateHtmlUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    BillPaymentSmsTemplateHtmlUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    TerminalReportTemplateHtmlUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
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
                    table.PrimaryKey("PK_SystemSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ErpModule = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxReliefs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_TaxReliefs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxTableHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TaxTableList = table.Column<List<TaxTable>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_TaxTableHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxTables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric", nullable: false),
                    UpperLimitOfAmount = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_TaxTables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeacherSubjectAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherName = table.Column<string>(type: "text", nullable: true),
                    SubjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectName = table.Column<string>(type: "text", nullable: true),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearName = table.Column<string>(type: "text", nullable: true),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassName = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_TeacherSubjectAssignments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TerminalReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    TermId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    StudentIdentifier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ClassName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AcademicYearName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PromotedTo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TermAverage = table.Column<decimal>(type: "numeric", nullable: false),
                    NumberOnRoll = table.Column<int>(type: "integer", nullable: false),
                    TermName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NextTermBegins = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    Attendance = table.Column<int>(type: "integer", nullable: false),
                    TotalAttendanceDays = table.Column<int>(type: "integer", nullable: false),
                    ClassTeacher = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    ClassTeacherRemarks = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    HeadTeacherSignatureImageUrl = table.Column<string>(type: "text", nullable: true),
                    SubjectResults = table.Column<List<SubjectResult>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_TerminalReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnitOfMeasure",
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
                    table.PrimaryKey("PK_UnitOfMeasure", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UomCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    MinimumCartQuantity = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_UomCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebHookRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    EndpointUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
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
                    table.PrimaryKey("PK_WebHookRegistry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkCenters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CapacityPerHour = table.Column<int>(type: "integer", nullable: false),
                    SupervisorId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupervisorName = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    SupervisorPhoneNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SupervisorEmail = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OperatingHours = table.Column<List<OperatingHour>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_WorkCenters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WssdDataBuckets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DataSetType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ParentBucketId = table.Column<Guid>(type: "uuid", nullable: false),
                    Dataset = table.Column<List<DefaultDatasetModel>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_WssdDataBuckets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WssdServiceInteractionConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WssdServiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    WssdServiceName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ParentNodes = table.Column<List<WssdServiceInteractionConfigurationParentNode>>(type: "jsonb", nullable: true),
                    Options = table.Column<List<WssdServiceInteractionConfigurationOption>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_WssdServiceInteractionConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WssdServiceReleases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WssdServiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    WssdServiceName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Environment = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    WhitelistedPhoneNumbers = table.Column<List<string>>(type: "jsonb", nullable: true),
                    WhitelistedEmails = table.Column<List<string>>(type: "jsonb", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    ApprovedBy = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: true),
                    TimeApproved = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ReleaseNotes = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ArtefactVersion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RollBackTo = table.Column<Guid>(type: "uuid", nullable: true),
                    RollBackDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ErrorRateThreshold = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrentErrorRate = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_WssdServiceReleases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WssdServiceRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WssdServiceName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UssdCode = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    IsUssdCodePorted = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayTitle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PurposeDescription = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ServiceDescription = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    MerchantId = table.Column<Guid>(type: "uuid", nullable: false),
                    MerchantName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    CountryCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SubscriptionPlan = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BillingCycle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Duration = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Fee = table.Column<decimal>(type: "numeric", nullable: false),
                    SessionRate = table.Column<decimal>(type: "numeric", nullable: false),
                    CustomSubdomain = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    WhiteLabelPrimaryThemeColor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_WssdServiceRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WssdServices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UssdCode = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    DisplayTitle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ActivatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LogoUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    IsWssdActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsUssdActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsWhiteLabelled = table.Column<bool>(type: "boolean", nullable: false),
                    WhiteLabelPrimaryThemeColor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    WhiteLabelSecondaryThemeColor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CustomSubdomain = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ClientId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ClientSecret = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AllowedOrigins = table.Column<List<string>>(type: "jsonb", nullable: true),
                    UseAsSecondaryService = table.Column<bool>(type: "boolean", nullable: false),
                    MerchantId = table.Column<Guid>(type: "uuid", nullable: false),
                    MerchantName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    CountryCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SubscriptionPlan = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BillingCycle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Duration = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Fee = table.Column<decimal>(type: "numeric", nullable: false),
                    UssdSessionRate = table.Column<decimal>(type: "numeric", nullable: false),
                    WssdSessionRate = table.Column<decimal>(type: "numeric", nullable: false),
                    SubscriptionDuration = table.Column<int>(type: "integer", nullable: false),
                    LastRenewalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    WebhookUrl = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    UssdSessionBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    WssdSessionBalance = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_WssdServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WssdSessionInteractions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    WssdServiceId = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    WssdServiceName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    InteractionSource = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    MobileNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Operator = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Details = table.Column<List<InteractionDetail>>(type: "jsonb", nullable: true),
                    Score = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_WssdSessionInteractions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpDynamicEntityProperties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityFullName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DynamicPropertyId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpDynamicEntityProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpDynamicEntityProperties_AbpDynamicProperties_DynamicProp~",
                        column: x => x.DynamicPropertyId,
                        principalTable: "AbpDynamicProperties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpDynamicPropertyValues",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    DynamicPropertyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpDynamicPropertyValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpDynamicPropertyValues_AbpDynamicProperties_DynamicProper~",
                        column: x => x.DynamicPropertyId,
                        principalTable: "AbpDynamicProperties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpFeatures",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    EditionId = table.Column<int>(type: "integer", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpFeatures_AbpEditions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "AbpEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpEntityChanges",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChangeTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ChangeType = table.Column<byte>(type: "smallint", nullable: false),
                    EntityChangeSetId = table.Column<long>(type: "bigint", nullable: false),
                    EntityId = table.Column<string>(type: "character varying(48)", maxLength: 48, nullable: true),
                    EntityTypeFullName = table.Column<string>(type: "character varying(192)", maxLength: 192, nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpEntityChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpEntityChanges_AbpEntityChangeSets_EntityChangeSetId",
                        column: x => x.EntityChangeSetId,
                        principalTable: "AbpEntityChangeSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IsStatic = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpRoles_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AbpRoles_AbpUsers_DeleterUserId",
                        column: x => x.DeleterUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AbpRoles_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AbpSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpSettings_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AbpTenants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BusinessId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BusinessLogo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    RequireTwoFactorAuth = table.Column<bool>(type: "boolean", nullable: false),
                    DataSupportBusinessAccountId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    HasSubscribedToGl = table.Column<bool>(type: "boolean", nullable: false),
                    GlSubscriptionStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    GlSubscriptionEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    GlSubscriptionFee = table.Column<decimal>(type: "numeric", nullable: false),
                    GlBillingCycle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GlNextRenewalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HasSubscribedToPayroll = table.Column<bool>(type: "boolean", nullable: false),
                    PayrollSubscriptionStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PayrollSubscriptionEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PayrollSubscriptionFee = table.Column<decimal>(type: "numeric", nullable: false),
                    PayrollBillingCycle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PayrollNextRenewalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HasSubscribedToSchoolManager = table.Column<bool>(type: "boolean", nullable: false),
                    SchoolManagerSubscriptionStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SchoolManagerSubscriptionEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SchoolManagerSubscriptionFee = table.Column<decimal>(type: "numeric", nullable: false),
                    SchoolManagerBillingCycle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SchoolManagerNextRenewalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HasSubscribedToAp = table.Column<bool>(type: "boolean", nullable: false),
                    ApSubscriptionStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ApSubscriptionEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ApSubscriptionFee = table.Column<decimal>(type: "numeric", nullable: false),
                    ApBillingCycle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ApNextRenewalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HasSubscribedToAr = table.Column<bool>(type: "boolean", nullable: false),
                    ArSubscriptionStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ArSubscriptionEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ArSubscriptionFee = table.Column<decimal>(type: "numeric", nullable: false),
                    ArBillingCycle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ArNextRenewalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HasSubscribedToFixedAssets = table.Column<bool>(type: "boolean", nullable: false),
                    FixedAssetsSubscriptionStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FixedAssetsSubscriptionEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FixedAssetsSubscriptionFee = table.Column<decimal>(type: "numeric", nullable: false),
                    FixedAssetsBillingCycle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FixedAssetsNextRenewalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HasSubscribedToHumanResource = table.Column<bool>(type: "boolean", nullable: false),
                    HumanResourceSubscriptionStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HumanResourceSubscriptionEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HumanResourceSubscriptionFee = table.Column<decimal>(type: "numeric", nullable: false),
                    HumanResourceBillingCycle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    HumanResourceNextRenewalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HasSubscribedToInventory = table.Column<bool>(type: "boolean", nullable: false),
                    InventorySubscriptionStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    InventorySubscriptionEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    InventorySubscriptionFee = table.Column<decimal>(type: "numeric", nullable: false),
                    InventoryBillingCycle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    InventoryNextRenewalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HasSubscribedToManufacturing = table.Column<bool>(type: "boolean", nullable: false),
                    ManufacturingSubscriptionStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ManufacturingSubscriptionEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ManufacturingSubscriptionFee = table.Column<decimal>(type: "numeric", nullable: false),
                    ManufacturingBillingCycle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ManufacturingNextRenewalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HasSubscribedToPointOfSale = table.Column<bool>(type: "boolean", nullable: false),
                    PointOfSaleSubscriptionStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PointOfSaleSubscriptionEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PointOfSaleSubscriptionFee = table.Column<decimal>(type: "numeric", nullable: false),
                    PointOfSaleBillingCycle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PointOfSaleNextRenewalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HasSubscribedToCoreBanking = table.Column<bool>(type: "boolean", nullable: false),
                    CoreBankingSubscriptionStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CoreBankingSubscriptionEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CoreBankingSubscriptionFee = table.Column<decimal>(type: "numeric", nullable: false),
                    CoreBankingBillingCycle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CoreBankingNextRenewalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HasSubscribedToSupplyChain = table.Column<bool>(type: "boolean", nullable: false),
                    SupplyChainSubscriptionStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SupplyChainSubscriptionEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SupplyChainSubscriptionFee = table.Column<decimal>(type: "numeric", nullable: false),
                    SupplyChainBillingCycle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SupplyChainNextRenewalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HasSubscribedToPropertyRentals = table.Column<bool>(type: "boolean", nullable: false),
                    PropertyRentalsSubscriptionStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PropertyRentalsSubscriptionEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PropertyRentalsSubscriptionFee = table.Column<decimal>(type: "numeric", nullable: false),
                    PropertyRentalsBillingCycle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PropertyRentalsNextRenewalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HasSubscribedToDocumentManagement = table.Column<bool>(type: "boolean", nullable: false),
                    DocumentManagementSubscriptionStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DocumentManagementSubscriptionEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DocumentManagementSubscriptionFee = table.Column<decimal>(type: "numeric", nullable: false),
                    DocumentManagementBillingCycle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DocumentManagementNextRenewalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDirectDebitEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PaymentMethod = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    MobileMoneyChannel = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Network = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    AccountId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AccountSecret = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TenancyName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ConnectionString = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    EditionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpTenants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpTenants_AbpEditions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "AbpEditions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AbpTenants_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AbpTenants_AbpUsers_DeleterUserId",
                        column: x => x.DeleterUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AbpTenants_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AbpUserClaims",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpUserClaims_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserLogins",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserLogins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpUserLogins_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpUserRoles_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserTokens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Value = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ExpireDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpUserTokens_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpWebhookSendAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WebhookEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    WebhookSubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Response = table.Column<string>(type: "text", nullable: true),
                    ResponseStatusCode = table.Column<int>(type: "integer", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpWebhookSendAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpWebhookSendAttempts_AbpWebhookEvents_WebhookEventId",
                        column: x => x.WebhookEventId,
                        principalTable: "AbpWebhookEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpDynamicEntityPropertyValues",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<string>(type: "text", nullable: true),
                    DynamicEntityPropertyId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpDynamicEntityPropertyValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpDynamicEntityPropertyValues_AbpDynamicEntityProperties_D~",
                        column: x => x.DynamicEntityPropertyId,
                        principalTable: "AbpDynamicEntityProperties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpEntityPropertyChanges",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityChangeId = table.Column<long>(type: "bigint", nullable: false),
                    NewValue = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    OriginalValue = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    PropertyName = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    PropertyTypeFullName = table.Column<string>(type: "character varying(192)", maxLength: 192, nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    NewValueHash = table.Column<string>(type: "text", nullable: true),
                    OriginalValueHash = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpEntityPropertyChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpEntityPropertyChanges_AbpEntityChanges_EntityChangeId",
                        column: x => x.EntityChangeId,
                        principalTable: "AbpEntityChanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpPermissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    IsGranted = table.Column<bool>(type: "boolean", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpPermissions_AbpRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AbpRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbpPermissions_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpRoleClaims",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: true),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpRoleClaims_AbpRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AbpRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogs_TenantId_ExecutionDuration",
                table: "AbpAuditLogs",
                columns: new[] { "TenantId", "ExecutionDuration" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogs_TenantId_ExecutionTime",
                table: "AbpAuditLogs",
                columns: new[] { "TenantId", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogs_TenantId_UserId",
                table: "AbpAuditLogs",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpBackgroundJobs_IsAbandoned_NextTryTime",
                table: "AbpBackgroundJobs",
                columns: new[] { "IsAbandoned", "NextTryTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpDynamicEntityProperties_DynamicPropertyId",
                table: "AbpDynamicEntityProperties",
                column: "DynamicPropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpDynamicEntityProperties_EntityFullName_DynamicPropertyId~",
                table: "AbpDynamicEntityProperties",
                columns: new[] { "EntityFullName", "DynamicPropertyId", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpDynamicEntityPropertyValues_DynamicEntityPropertyId",
                table: "AbpDynamicEntityPropertyValues",
                column: "DynamicEntityPropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpDynamicProperties_PropertyName_TenantId",
                table: "AbpDynamicProperties",
                columns: new[] { "PropertyName", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpDynamicPropertyValues_DynamicPropertyId",
                table: "AbpDynamicPropertyValues",
                column: "DynamicPropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityChanges_EntityChangeSetId",
                table: "AbpEntityChanges",
                column: "EntityChangeSetId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityChanges_EntityTypeFullName_EntityId",
                table: "AbpEntityChanges",
                columns: new[] { "EntityTypeFullName", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityChangeSets_TenantId_CreationTime",
                table: "AbpEntityChangeSets",
                columns: new[] { "TenantId", "CreationTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityChangeSets_TenantId_Reason",
                table: "AbpEntityChangeSets",
                columns: new[] { "TenantId", "Reason" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityChangeSets_TenantId_UserId",
                table: "AbpEntityChangeSets",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityPropertyChanges_EntityChangeId",
                table: "AbpEntityPropertyChanges",
                column: "EntityChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatures_EditionId_Name",
                table: "AbpFeatures",
                columns: new[] { "EditionId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatures_TenantId_Name",
                table: "AbpFeatures",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpLanguages_TenantId_Name",
                table: "AbpLanguages",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpLanguageTexts_TenantId_Source_LanguageName_Key",
                table: "AbpLanguageTexts",
                columns: new[] { "TenantId", "Source", "LanguageName", "Key" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpNotificationSubscriptions_NotificationName_EntityTypeNam~",
                table: "AbpNotificationSubscriptions",
                columns: new[] { "NotificationName", "EntityTypeName", "EntityId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpNotificationSubscriptions_TenantId_NotificationName_Enti~",
                table: "AbpNotificationSubscriptions",
                columns: new[] { "TenantId", "NotificationName", "EntityTypeName", "EntityId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnitRoles_TenantId_OrganizationUnitId",
                table: "AbpOrganizationUnitRoles",
                columns: new[] { "TenantId", "OrganizationUnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnitRoles_TenantId_RoleId",
                table: "AbpOrganizationUnitRoles",
                columns: new[] { "TenantId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnits_ParentId",
                table: "AbpOrganizationUnits",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnits_TenantId_Code",
                table: "AbpOrganizationUnits",
                columns: new[] { "TenantId", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissions_RoleId",
                table: "AbpPermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissions_TenantId_Name",
                table: "AbpPermissions",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissions_UserId",
                table: "AbpPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoleClaims_RoleId",
                table: "AbpRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoleClaims_TenantId_ClaimType",
                table: "AbpRoleClaims",
                columns: new[] { "TenantId", "ClaimType" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoles_CreatorUserId",
                table: "AbpRoles",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoles_DeleterUserId",
                table: "AbpRoles",
                column: "DeleterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoles_LastModifierUserId",
                table: "AbpRoles",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoles_TenantId_NormalizedName",
                table: "AbpRoles",
                columns: new[] { "TenantId", "NormalizedName" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSettings_TenantId_Name_UserId",
                table: "AbpSettings",
                columns: new[] { "TenantId", "Name", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpSettings_UserId",
                table: "AbpSettings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenantNotifications_TenantId",
                table: "AbpTenantNotifications",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_CreatorUserId",
                table: "AbpTenants",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_DeleterUserId",
                table: "AbpTenants",
                column: "DeleterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_EditionId",
                table: "AbpTenants",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_LastModifierUserId",
                table: "AbpTenants",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_TenancyName",
                table: "AbpTenants",
                column: "TenancyName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserAccounts_EmailAddress",
                table: "AbpUserAccounts",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserAccounts_TenantId_EmailAddress",
                table: "AbpUserAccounts",
                columns: new[] { "TenantId", "EmailAddress" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserAccounts_TenantId_UserId",
                table: "AbpUserAccounts",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserAccounts_TenantId_UserName",
                table: "AbpUserAccounts",
                columns: new[] { "TenantId", "UserName" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserAccounts_UserName",
                table: "AbpUserAccounts",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserClaims_TenantId_ClaimType",
                table: "AbpUserClaims",
                columns: new[] { "TenantId", "ClaimType" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserClaims_UserId",
                table: "AbpUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserLoginAttempts_TenancyName_UserNameOrEmailAddress_Res~",
                table: "AbpUserLoginAttempts",
                columns: new[] { "TenancyName", "UserNameOrEmailAddress", "Result" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserLoginAttempts_UserId_TenantId",
                table: "AbpUserLoginAttempts",
                columns: new[] { "UserId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserLogins_ProviderKey_TenantId",
                table: "AbpUserLogins",
                columns: new[] { "ProviderKey", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserLogins_TenantId_LoginProvider_ProviderKey",
                table: "AbpUserLogins",
                columns: new[] { "TenantId", "LoginProvider", "ProviderKey" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserLogins_TenantId_UserId",
                table: "AbpUserLogins",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserLogins_UserId",
                table: "AbpUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserNotifications_UserId_State_CreationTime",
                table: "AbpUserNotifications",
                columns: new[] { "UserId", "State", "CreationTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserOrganizationUnits_TenantId_OrganizationUnitId",
                table: "AbpUserOrganizationUnits",
                columns: new[] { "TenantId", "OrganizationUnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserOrganizationUnits_TenantId_UserId",
                table: "AbpUserOrganizationUnits",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserRoles_TenantId_RoleId",
                table: "AbpUserRoles",
                columns: new[] { "TenantId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserRoles_TenantId_UserId",
                table: "AbpUserRoles",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserRoles_UserId",
                table: "AbpUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_CreatorUserId",
                table: "AbpUsers",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_DeleterUserId",
                table: "AbpUsers",
                column: "DeleterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_LastModifierUserId",
                table: "AbpUsers",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_TenantId_NormalizedEmailAddress",
                table: "AbpUsers",
                columns: new[] { "TenantId", "NormalizedEmailAddress" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_TenantId_NormalizedUserName",
                table: "AbpUsers",
                columns: new[] { "TenantId", "NormalizedUserName" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserTokens_TenantId_UserId",
                table: "AbpUserTokens",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserTokens_UserId",
                table: "AbpUserTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpWebhookSendAttempts_WebhookEventId",
                table: "AbpWebhookSendAttempts",
                column: "WebhookEventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbpAuditLogs");

            migrationBuilder.DropTable(
                name: "AbpBackgroundJobs");

            migrationBuilder.DropTable(
                name: "AbpDynamicEntityPropertyValues");

            migrationBuilder.DropTable(
                name: "AbpDynamicPropertyValues");

            migrationBuilder.DropTable(
                name: "AbpEntityPropertyChanges");

            migrationBuilder.DropTable(
                name: "AbpFeatures");

            migrationBuilder.DropTable(
                name: "AbpLanguages");

            migrationBuilder.DropTable(
                name: "AbpLanguageTexts");

            migrationBuilder.DropTable(
                name: "AbpNotifications");

            migrationBuilder.DropTable(
                name: "AbpNotificationSubscriptions");

            migrationBuilder.DropTable(
                name: "AbpOrganizationUnitRoles");

            migrationBuilder.DropTable(
                name: "AbpOrganizationUnits");

            migrationBuilder.DropTable(
                name: "AbpPermissions");

            migrationBuilder.DropTable(
                name: "AbpRoleClaims");

            migrationBuilder.DropTable(
                name: "AbpSettings");

            migrationBuilder.DropTable(
                name: "AbpTenantNotifications");

            migrationBuilder.DropTable(
                name: "AbpTenants");

            migrationBuilder.DropTable(
                name: "AbpUserAccounts");

            migrationBuilder.DropTable(
                name: "AbpUserClaims");

            migrationBuilder.DropTable(
                name: "AbpUserLoginAttempts");

            migrationBuilder.DropTable(
                name: "AbpUserLogins");

            migrationBuilder.DropTable(
                name: "AbpUserNotifications");

            migrationBuilder.DropTable(
                name: "AbpUserOrganizationUnits");

            migrationBuilder.DropTable(
                name: "AbpUserRoles");

            migrationBuilder.DropTable(
                name: "AbpUserTokens");

            migrationBuilder.DropTable(
                name: "AbpWebhookSendAttempts");

            migrationBuilder.DropTable(
                name: "AbpWebhookSubscriptions");

            migrationBuilder.DropTable(
                name: "AcademicYears");

            migrationBuilder.DropTable(
                name: "AccountBalances");

            migrationBuilder.DropTable(
                name: "AccountHeads");

            migrationBuilder.DropTable(
                name: "AccountingPeriods");

            migrationBuilder.DropTable(
                name: "AllowanceTypes");

            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "AssessmentTypes");

            migrationBuilder.DropTable(
                name: "AssignedClasses");

            migrationBuilder.DropTable(
                name: "BankAccounts");

            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropTable(
                name: "BikTypes");

            migrationBuilder.DropTable(
                name: "BillOfMaterialItems");

            migrationBuilder.DropTable(
                name: "BillPayments");

            migrationBuilder.DropTable(
                name: "BillSetups");

            migrationBuilder.DropTable(
                name: "BillTypes");

            migrationBuilder.DropTable(
                name: "BonusAndOnetimeAllowances");

            migrationBuilder.DropTable(
                name: "BookLendings");

            migrationBuilder.DropTable(
                name: "Budgets");

            migrationBuilder.DropTable(
                name: "BusinessDates");

            migrationBuilder.DropTable(
                name: "BusinessProfiles");

            migrationBuilder.DropTable(
                name: "CancelledBills");

            migrationBuilder.DropTable(
                name: "CancelledPayments");

            migrationBuilder.DropTable(
                name: "ClassStreams");

            migrationBuilder.DropTable(
                name: "Clubs");

            migrationBuilder.DropTable(
                name: "CoaControls");

            migrationBuilder.DropTable(
                name: "CoaDetails");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "CostCenters");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "CreditAdjustments");

            migrationBuilder.DropTable(
                name: "CreditSaleRepaymentSchedules");

            migrationBuilder.DropTable(
                name: "CreditSales");

            migrationBuilder.DropTable(
                name: "CreditTerms");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "Curricula");

            migrationBuilder.DropTable(
                name: "CustomerRanks");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "DeductionTypes");

            migrationBuilder.DropTable(
                name: "DefaultGlNumbers");

            migrationBuilder.DropTable(
                name: "Deliveries");

            migrationBuilder.DropTable(
                name: "DeliveryChannels");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "DisciplinaryActions");

            migrationBuilder.DropTable(
                name: "DmsComments");

            migrationBuilder.DropTable(
                name: "DmsCustomAttributes");

            migrationBuilder.DropTable(
                name: "DmsCustomIdentifiers");

            migrationBuilder.DropTable(
                name: "DmsDocumentMetadataValues");

            migrationBuilder.DropTable(
                name: "DmsDocumentTags");

            migrationBuilder.DropTable(
                name: "DmsDocumentTemplates");

            migrationBuilder.DropTable(
                name: "DmsTags");

            migrationBuilder.DropTable(
                name: "DmsWorkflowInstances");

            migrationBuilder.DropTable(
                name: "DmsWorkflowTasks");

            migrationBuilder.DropTable(
                name: "DmsWorkflowTemplates");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Dormitories");

            migrationBuilder.DropTable(
                name: "DormitoryRooms");

            migrationBuilder.DropTable(
                name: "ElectiveSubjects");

            migrationBuilder.DropTable(
                name: "EmployeeAllowances");

            migrationBuilder.DropTable(
                name: "EmployeeBenefitInKinds");

            migrationBuilder.DropTable(
                name: "EmployeeBioDatasx");

            migrationBuilder.DropTable(
                name: "EmployeeCategories");

            migrationBuilder.DropTable(
                name: "EmployeeDaysWorked");

            migrationBuilder.DropTable(
                name: "EmployeeDeductions");

            migrationBuilder.DropTable(
                name: "EmployeeLoanRepaymentSchedules");

            migrationBuilder.DropTable(
                name: "EmployeeLoans");

            migrationBuilder.DropTable(
                name: "EmployeeOnetimeDeductions");

            migrationBuilder.DropTable(
                name: "EmployeeRanks");

            migrationBuilder.DropTable(
                name: "EmployeeReliefs");

            migrationBuilder.DropTable(
                name: "EmployeeSalaryAdvances");

            migrationBuilder.DropTable(
                name: "EmployeeSalaryData");

            migrationBuilder.DropTable(
                name: "EmployeeSnits");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "ExamSchedules");

            migrationBuilder.DropTable(
                name: "FeesDescriptions");

            migrationBuilder.DropTable(
                name: "FieldTrips");

            migrationBuilder.DropTable(
                name: "FinalProductTests");

            migrationBuilder.DropTable(
                name: "FinishedGoodAllocation");

            migrationBuilder.DropTable(
                name: "FinishedGoods");

            migrationBuilder.DropTable(
                name: "Folders");

            migrationBuilder.DropTable(
                name: "GiftCards");

            migrationBuilder.DropTable(
                name: "GradingSchemes");

            migrationBuilder.DropTable(
                name: "Gratuities");

            migrationBuilder.DropTable(
                name: "ImprestCategories");

            migrationBuilder.DropTable(
                name: "Imprests");

            migrationBuilder.DropTable(
                name: "InProcessInspections");

            migrationBuilder.DropTable(
                name: "InstalledPlugins");

            migrationBuilder.DropTable(
                name: "InventoryCategories");

            migrationBuilder.DropTable(
                name: "ItemTags");

            migrationBuilder.DropTable(
                name: "JournalDetails");

            migrationBuilder.DropTable(
                name: "Journals");

            migrationBuilder.DropTable(
                name: "Laboratories");

            migrationBuilder.DropTable(
                name: "LaboratoryEquipments");

            migrationBuilder.DropTable(
                name: "LessonPlans");

            migrationBuilder.DropTable(
                name: "Levels");

            migrationBuilder.DropTable(
                name: "Libraries");

            migrationBuilder.DropTable(
                name: "LibraryBooks");

            migrationBuilder.DropTable(
                name: "LoanAmortizationHistory");

            migrationBuilder.DropTable(
                name: "LoanApplications");

            migrationBuilder.DropTable(
                name: "LoanTypes");

            migrationBuilder.DropTable(
                name: "MaterialConsumptions");

            migrationBuilder.DropTable(
                name: "Merchants");

            migrationBuilder.DropTable(
                name: "MessageTemplates");

            migrationBuilder.DropTable(
                name: "MiscellaneousPayments");

            migrationBuilder.DropTable(
                name: "MonthlyAllowanceHistory");

            migrationBuilder.DropTable(
                name: "MonthlyAllowances");

            migrationBuilder.DropTable(
                name: "MonthlyBenefitsInKindHistory");

            migrationBuilder.DropTable(
                name: "MonthlyBenefitsInKinds");

            migrationBuilder.DropTable(
                name: "MonthlyBonuses");

            migrationBuilder.DropTable(
                name: "MonthlyBonusHistory");

            migrationBuilder.DropTable(
                name: "MonthlyCumulativeDeductionHistory");

            migrationBuilder.DropTable(
                name: "MonthlyCumulativeDeductions");

            migrationBuilder.DropTable(
                name: "MonthlyDeductionHistory");

            migrationBuilder.DropTable(
                name: "MonthlyDeductions");

            migrationBuilder.DropTable(
                name: "MonthlyIrsTaxes");

            migrationBuilder.DropTable(
                name: "MonthlyIrsTaxHistory");

            migrationBuilder.DropTable(
                name: "MonthlyLoanDeductionHistory");

            migrationBuilder.DropTable(
                name: "MonthlyLoanDeductions");

            migrationBuilder.DropTable(
                name: "MonthlyOnetimeDeductionHistory");

            migrationBuilder.DropTable(
                name: "MonthlyOnetimeDeductions");

            migrationBuilder.DropTable(
                name: "MonthlyOvertimeHistory");

            migrationBuilder.DropTable(
                name: "MonthlyOvertimes");

            migrationBuilder.DropTable(
                name: "MonthlyPfDeductionHistory");

            migrationBuilder.DropTable(
                name: "MonthlyPfDeductions");

            migrationBuilder.DropTable(
                name: "MonthlyReliefHistory");

            migrationBuilder.DropTable(
                name: "MonthlyReliefs");

            migrationBuilder.DropTable(
                name: "MonthlySalaryAdvanceHistory");

            migrationBuilder.DropTable(
                name: "MonthlySalaryAdvances");

            migrationBuilder.DropTable(
                name: "MonthlySecPfDeductionHistory");

            migrationBuilder.DropTable(
                name: "MonthlySecPfDeductions");

            migrationBuilder.DropTable(
                name: "MonthlySsnitDeductionHistory");

            migrationBuilder.DropTable(
                name: "MonthlySsnitDeductions");

            migrationBuilder.DropTable(
                name: "NonConformance");

            migrationBuilder.DropTable(
                name: "NotificationAccounts");

            migrationBuilder.DropTable(
                name: "Operations");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "OvertimeTimeSheets");

            migrationBuilder.DropTable(
                name: "OvertimeTypes");

            migrationBuilder.DropTable(
                name: "ParentTeacherMeetings");

            migrationBuilder.DropTable(
                name: "PartnerCategories");

            migrationBuilder.DropTable(
                name: "PastQuestions");

            migrationBuilder.DropTable(
                name: "PayCalendars");

            migrationBuilder.DropTable(
                name: "PaymasterHistory");

            migrationBuilder.DropTable(
                name: "Paymasters");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "PaymentTypes");

            migrationBuilder.DropTable(
                name: "PayrollPeriods");

            migrationBuilder.DropTable(
                name: "PettyCashRecipients");

            migrationBuilder.DropTable(
                name: "PickupPoints");

            migrationBuilder.DropTable(
                name: "PluginActivityLogs");

            migrationBuilder.DropTable(
                name: "PosBranches");

            migrationBuilder.DropTable(
                name: "PosCustomers");

            migrationBuilder.DropTable(
                name: "PosInventoryItems");

            migrationBuilder.DropTable(
                name: "ProductionOrders");

            migrationBuilder.DropTable(
                name: "ProductionWorkflows");

            migrationBuilder.DropTable(
                name: "ProductReviews");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "PromotionHistory");

            migrationBuilder.DropTable(
                name: "PvRetentions");

            migrationBuilder.DropTable(
                name: "QcChecklists");

            migrationBuilder.DropTable(
                name: "RawMaterials");

            migrationBuilder.DropTable(
                name: "RefundAndReturns");

            migrationBuilder.DropTable(
                name: "ReportDownloads");

            migrationBuilder.DropTable(
                name: "ResultsUploads");

            migrationBuilder.DropTable(
                name: "ResultTypes");

            migrationBuilder.DropTable(
                name: "SalaryAdvanceApplications");

            migrationBuilder.DropTable(
                name: "SalaryGrades");

            migrationBuilder.DropTable(
                name: "SalaryIncrementHistory");

            migrationBuilder.DropTable(
                name: "SalesAgents");

            migrationBuilder.DropTable(
                name: "SchClasses");

            migrationBuilder.DropTable(
                name: "Scholarships");

            migrationBuilder.DropTable(
                name: "SchoolProfiles");

            migrationBuilder.DropTable(
                name: "ScrapAndWastes");

            migrationBuilder.DropTable(
                name: "ShopCarts");

            migrationBuilder.DropTable(
                name: "Siblings");

            migrationBuilder.DropTable(
                name: "SpecialNeeds");

            migrationBuilder.DropTable(
                name: "SportsCompetitions");

            migrationBuilder.DropTable(
                name: "SportsTeams");

            migrationBuilder.DropTable(
                name: "StaffDesignations");

            migrationBuilder.DropTable(
                name: "StaffMeetings");

            migrationBuilder.DropTable(
                name: "Staffs");

            migrationBuilder.DropTable(
                name: "StockReceipts");

            migrationBuilder.DropTable(
                name: "StockTransfers");

            migrationBuilder.DropTable(
                name: "StudentAttendances");

            migrationBuilder.DropTable(
                name: "StudentAwards");

            migrationBuilder.DropTable(
                name: "StudentBehaviorRecords");

            migrationBuilder.DropTable(
                name: "StudentBillReceipts");

            migrationBuilder.DropTable(
                name: "StudentBillReportHistory");

            migrationBuilder.DropTable(
                name: "StudentBills");

            migrationBuilder.DropTable(
                name: "StudentClubMemberships");

            migrationBuilder.DropTable(
                name: "StudentCounselings");

            migrationBuilder.DropTable(
                name: "StudentDormitoryAssignments");

            migrationBuilder.DropTable(
                name: "StudentHealthRecords");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "StudentStatements");

            migrationBuilder.DropTable(
                name: "SubjectCategories");

            migrationBuilder.DropTable(
                name: "SubjectRemarks");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "SubscriptionPayments");

            migrationBuilder.DropTable(
                name: "SubscriptionRenewalHistory");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Syllabi");

            migrationBuilder.DropTable(
                name: "SystemNumbers");

            migrationBuilder.DropTable(
                name: "SystemSetting");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "TaxReliefs");

            migrationBuilder.DropTable(
                name: "TaxTableHistory");

            migrationBuilder.DropTable(
                name: "TaxTables");

            migrationBuilder.DropTable(
                name: "TeacherSubjectAssignments");

            migrationBuilder.DropTable(
                name: "TerminalReports");

            migrationBuilder.DropTable(
                name: "UnitOfMeasure");

            migrationBuilder.DropTable(
                name: "UomCodes");

            migrationBuilder.DropTable(
                name: "WebHookRegistry");

            migrationBuilder.DropTable(
                name: "WorkCenters");

            migrationBuilder.DropTable(
                name: "WssdDataBuckets");

            migrationBuilder.DropTable(
                name: "WssdServiceInteractionConfigurations");

            migrationBuilder.DropTable(
                name: "WssdServiceReleases");

            migrationBuilder.DropTable(
                name: "WssdServiceRequests");

            migrationBuilder.DropTable(
                name: "WssdServices");

            migrationBuilder.DropTable(
                name: "WssdSessionInteractions");

            migrationBuilder.DropTable(
                name: "AbpDynamicEntityProperties");

            migrationBuilder.DropTable(
                name: "AbpEntityChanges");

            migrationBuilder.DropTable(
                name: "AbpRoles");

            migrationBuilder.DropTable(
                name: "AbpEditions");

            migrationBuilder.DropTable(
                name: "AbpWebhookEvents");

            migrationBuilder.DropTable(
                name: "AbpDynamicProperties");

            migrationBuilder.DropTable(
                name: "AbpEntityChangeSets");

            migrationBuilder.DropTable(
                name: "AbpUsers");
        }
    }
}
