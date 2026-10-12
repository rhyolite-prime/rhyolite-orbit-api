using System;
using Microsoft.EntityFrameworkCore.Migrations;
using RhyoliteOrbit.Models.Dms;

namespace RhyoliteOrbit.Migrations;

/// <summary>
/// Completes the Document Management module schema: new entities (versions, ACL,
/// audit, shares, retention, legal holds, signatures, links, embeddings, events,
/// quota, ingest jobs, settings, annotations) and the columns required by the
/// full DMS feature set. Includes a data backfill for the pre-existing tables.
/// </summary>
public partial class DmsModuleV1: Migration
{
    private const string EmptyGuid = "'00000000-0000-0000-0000-000000000000'";

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // ---------------------------------------------------------------- documents
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""CollectionId"" uuid NOT NULL DEFAULT " + EmptyGuid + ";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""Extension"" character varying(30) NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""CustomId"" character varying(100) NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ALTER COLUMN ""FileSize"" TYPE bigint;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""PageCount"" integer NOT NULL DEFAULT 0;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""Language"" character varying(20) NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""StorageProvider"" character varying(30) NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""StorageKey"" character varying(500) NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""CurrentVersionId"" uuid NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""OwnerId"" bigint NOT NULL DEFAULT 0;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""IsCheckedOut"" boolean NOT NULL DEFAULT false;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""CheckedOutBy"" bigint NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""CheckedOutAt"" timestamp without time zone NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""LockExpiry"" timestamp without time zone NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""RetentionPolicyId"" uuid NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""RetentionEventDate"" timestamp without time zone NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""ExpiryDate"" timestamp without time zone NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""IsOnLegalHold"" boolean NOT NULL DEFAULT false;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""IsWorm"" boolean NOT NULL DEFAULT false;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""IndexStatus"" integer NOT NULL DEFAULT 0;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""IndexedAt"" timestamp without time zone NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""ScanStatus"" integer NOT NULL DEFAULT 0;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""OcrStatus"" integer NOT NULL DEFAULT 0;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""AiStatus"" integer NOT NULL DEFAULT 0;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""PreviewPath"" character varying(500) NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""ThumbnailPath"" character varying(500) NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""VersionId"";");
        migrationBuilder.Sql(@"
            ALTER TABLE ""Documents"" ADD COLUMN ""SearchVector"" tsvector
            GENERATED ALWAYS AS (
                to_tsvector('simple',
                    coalesce(""Name"", '') || ' ' ||
                    coalesce(""Title"", '') || ' ' ||
                    coalesce(""Description"", '') || ' ' ||
                    coalesce(""CustomId"", ''))
            ) STORED;");

        // ---------------------------------------------------------------- folders
        migrationBuilder.Sql(@"UPDATE ""Folders"" SET ""ParentId"" = NULL WHERE ""ParentId"" = " + EmptyGuid + ";");
        migrationBuilder.Sql(@"ALTER TABLE ""Folders"" ALTER COLUMN ""ParentId"" DROP NOT NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""Folders"" ADD COLUMN ""CollectionId"" uuid NOT NULL DEFAULT " + EmptyGuid + ";");
        migrationBuilder.Sql(@"ALTER TABLE ""Folders"" ADD COLUMN ""OwnerId"" bigint NOT NULL DEFAULT 0;");
        migrationBuilder.Sql(@"ALTER TABLE ""Folders"" ADD COLUMN ""TemplateId"" uuid NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""Folders"" ADD COLUMN ""IdentifierId"" uuid NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""Folders"" ADD COLUMN ""InheritPermissions"" boolean NOT NULL DEFAULT true;");
        migrationBuilder.Sql(@"ALTER TABLE ""Folders"" ADD COLUMN ""IsSystem"" boolean NOT NULL DEFAULT false;");
        migrationBuilder.Sql(@"ALTER TABLE ""Folders"" ADD COLUMN ""Depth"" integer NOT NULL DEFAULT 0;");

        // ---------------------------------------------------------------- collections
        migrationBuilder.Sql(@"ALTER TABLE ""Collections"" ADD COLUMN ""IsDefault"" boolean NOT NULL DEFAULT false;");
        migrationBuilder.Sql(@"ALTER TABLE ""Collections"" ADD COLUMN ""StorageQuotaBytes"" bigint NOT NULL DEFAULT 0;");

        // ---------------------------------------------------------------- tags
        migrationBuilder.Sql(@"ALTER TABLE ""DmsTags"" ADD COLUMN ""IsSystem"" boolean NOT NULL DEFAULT false;");

        // ---------------------------------------------------------------- templates / attributes / identifiers
        migrationBuilder.Sql(@"ALTER TABLE ""DmsDocumentTemplates"" ADD COLUMN ""AllowedFileTypes"" jsonb NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsDocumentTemplates"" ADD COLUMN ""WorkflowTemplateId"" uuid NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsDocumentTemplates"" ADD COLUMN ""RetentionPolicyId"" uuid NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsDocumentTemplates"" ADD COLUMN ""VersionScheme"" integer NOT NULL DEFAULT 0;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsDocumentTemplates"" ADD COLUMN ""RequiresApproval"" boolean NOT NULL DEFAULT false;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsCustomAttributes"" ADD COLUMN ""IsSearchable"" boolean NOT NULL DEFAULT false;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsCustomAttributes"" ADD COLUMN ""IsSensitive"" boolean NOT NULL DEFAULT false;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsCustomAttributes"" ADD COLUMN ""ValidationRegex"" character varying(200) NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsCustomIdentifiers"" ADD COLUMN ""IsActive"" boolean NOT NULL DEFAULT false;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsCustomIdentifiers"" ADD COLUMN ""ResetPolicy"" integer NOT NULL DEFAULT 0;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsCustomIdentifiers"" ADD COLUMN ""LastGeneratedAt"" timestamp without time zone NULL;");

        // ---------------------------------------------------------------- comments
        migrationBuilder.Sql(@"ALTER TABLE ""DmsComments"" ADD COLUMN ""VersionId"" uuid NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsComments"" ADD COLUMN ""AnchorJson"" jsonb NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsComments"" ADD COLUMN ""ResolvedById"" bigint NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsComments"" ADD COLUMN ""ResolvedAt"" timestamp without time zone NULL;");

        // ---------------------------------------------------------------- workflow
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowTemplates"" ADD COLUMN ""IsActive"" boolean NOT NULL DEFAULT false;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowTemplates"" ADD COLUMN ""Version"" integer NOT NULL DEFAULT 0;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowInstances"" ADD COLUMN ""TemplateName"" character varying(100) NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowInstances"" ADD COLUMN ""StartedById"" bigint NOT NULL DEFAULT 0;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowInstances"" ADD COLUMN ""DueDate"" timestamp without time zone NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowInstances"" ADD COLUMN ""LastActionAt"" timestamp without time zone NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowInstances"" ADD COLUMN ""TemplateSnapshotJson"" jsonb NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowInstances"" ADD COLUMN ""VariablesJson"" jsonb NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowTasks"" ADD COLUMN ""StepIndex"" integer NOT NULL DEFAULT 0;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowTasks"" ADD COLUMN ""StepName"" character varying(100) NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowTasks"" ADD COLUMN ""DueDate"" timestamp without time zone NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowTasks"" ADD COLUMN ""DelegatedFrom"" bigint NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowTasks"" ADD COLUMN ""OutcomeJson"" jsonb NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowTasks"" ADD COLUMN ""SignatureId"" uuid NULL;");

        // ---------------------------------------------------------------- new tables
        migrationBuilder.Sql(@"
            CREATE TABLE IF NOT EXISTS ""DmsAnnotations"" (
                ""Id"" uuid NOT NULL,
                ""DocumentId"" uuid NOT NULL,
                ""VersionId"" uuid NOT NULL,
                ""PageNumber"" integer NOT NULL,
                ""Type"" integer NOT NULL,
                ""GeometryJson"" jsonb NULL,
                ""Text"" character varying(1000) NULL,
                ""Color"" character varying(20) NULL,
                ""CreatedById"" bigint NOT NULL,
                ""IsRedaction"" boolean NOT NULL,
                ""TenantId"" integer NOT NULL,
                ""CreationTime"" timestamp without time zone NOT NULL,
                ""CreatorUserId"" bigint NULL,
                ""LastModificationTime"" timestamp without time zone NULL,
                ""LastModifierUserId"" bigint NULL,
                ""IsDeleted"" boolean NOT NULL,
                ""DeleterUserId"" bigint NULL,
                ""DeletionTime"" timestamp without time zone NULL,
                CONSTRAINT ""PK_DmsAnnotations"" PRIMARY KEY (""Id"")
            );");

        migrationBuilder.Sql(@"
            CREATE TABLE IF NOT EXISTS ""DmsDocumentVersions"" (
                ""Id"" uuid NOT NULL,
                ""DocumentId"" uuid NOT NULL,
                ""VersionNumber"" integer NOT NULL,
                ""MinorNumber"" integer NOT NULL,
                ""VersionLabel"" character varying(50) NULL,
                ""ChangeNote"" character varying(500) NULL,
                ""FileName"" character varying(255) NULL,
                ""Extension"" character varying(30) NULL,
                ""FileSize"" bigint NOT NULL,
                ""MimeType"" character varying(255) NULL,
                ""StorageProvider"" character varying(30) NULL,
                ""StorageKey"" character varying(500) NULL,
                ""Checksum"" character varying(64) NULL,
                ""IsCurrent"" boolean NOT NULL,
                ""PageCount"" integer NOT NULL,
                ""ExtractedText"" text NULL,
                ""OcrApplied"" boolean NOT NULL,
                ""Language"" character varying(20) NULL,
                ""PreviewPath"" character varying(500) NULL,
                ""ThumbnailPath"" character varying(500) NULL,
                ""TenantId"" integer NOT NULL,
                ""CreationTime"" timestamp without time zone NOT NULL,
                ""CreatorUserId"" bigint NULL,
                ""LastModificationTime"" timestamp without time zone NULL,
                ""LastModifierUserId"" bigint NULL,
                ""IsDeleted"" boolean NOT NULL,
                ""DeleterUserId"" bigint NULL,
                ""DeletionTime"" timestamp without time zone NULL,
                CONSTRAINT ""PK_DmsDocumentVersions"" PRIMARY KEY (""Id"")
            );");

        migrationBuilder.Sql(@"
            CREATE TABLE IF NOT EXISTS ""DmsAclEntries"" (
                ""Id"" uuid NOT NULL,
                ""SecurableType"" integer NOT NULL,
                ""SecurableId"" uuid NOT NULL,
                ""SubjectType"" integer NOT NULL,
                ""SubjectId"" bigint NOT NULL,
                ""Permission"" integer NOT NULL,
                ""Effect"" integer NOT NULL,
                ""IsInherited"" boolean NOT NULL,
                ""TenantId"" integer NOT NULL,
                ""CreationTime"" timestamp without time zone NOT NULL,
                ""CreatorUserId"" bigint NULL,
                ""LastModificationTime"" timestamp without time zone NULL,
                ""LastModifierUserId"" bigint NULL,
                ""IsDeleted"" boolean NOT NULL,
                ""DeleterUserId"" bigint NULL,
                ""DeletionTime"" timestamp without time zone NULL,
                CONSTRAINT ""PK_DmsAclEntries"" PRIMARY KEY (""Id"")
            );");

        migrationBuilder.Sql(@"
            CREATE TABLE IF NOT EXISTS ""DmsAuditEntries"" (
                ""Id"" uuid NOT NULL,
                ""DocumentId"" uuid NULL,
                ""FolderId"" uuid NULL,
                ""Action"" integer NOT NULL,
                ""ActorId"" bigint NULL,
                ""DetailsJson"" jsonb NULL,
                ""IpAddress"" character varying(100) NULL,
                ""UserAgent"" character varying(300) NULL,
                ""Sequence"" bigint NOT NULL,
                ""PreviousHash"" character varying(64) NULL,
                ""EntryHash"" character varying(64) NULL,
                ""TenantId"" integer NOT NULL,
                ""CreationTime"" timestamp without time zone NOT NULL,
                ""CreatorUserId"" bigint NULL,
                CONSTRAINT ""PK_DmsAuditEntries"" PRIMARY KEY (""Id"")
            );");

        migrationBuilder.Sql(@"
            CREATE TABLE IF NOT EXISTS ""DmsShares"" (
                ""Id"" uuid NOT NULL,
                ""DocumentId"" uuid NULL,
                ""FolderId"" uuid NULL,
                ""ShareType"" integer NOT NULL,
                ""Token"" character varying(128) NULL,
                ""PasswordHash"" character varying(200) NULL,
                ""SharedWithEmail"" character varying(200) NULL,
                ""SharedWithUserId"" bigint NULL,
                ""ExpiresAt"" timestamp without time zone NULL,
                ""AllowPreview"" boolean NOT NULL,
                ""AllowDownload"" boolean NOT NULL,
                ""AllowUpload"" boolean NOT NULL,
                ""MaxDownloads"" integer NULL,
                ""DownloadCount"" integer NOT NULL,
                ""RequiresWatermark"" boolean NOT NULL,
                ""IsRevoked"" boolean NOT NULL,
                ""LastAccessedAt"" timestamp without time zone NULL,
                ""CreatedById"" bigint NOT NULL,
                ""TenantId"" integer NOT NULL,
                ""CreationTime"" timestamp without time zone NOT NULL,
                ""CreatorUserId"" bigint NULL,
                ""LastModificationTime"" timestamp without time zone NULL,
                ""LastModifierUserId"" bigint NULL,
                ""IsDeleted"" boolean NOT NULL,
                ""DeleterUserId"" bigint NULL,
                ""DeletionTime"" timestamp without time zone NULL,
                CONSTRAINT ""PK_DmsShares"" PRIMARY KEY (""Id"")
            );");

        migrationBuilder.Sql(@"
            CREATE TABLE IF NOT EXISTS ""DmsSavedSearches"" (
                ""Id"" uuid NOT NULL,
                ""UserId"" bigint NOT NULL,
                ""Name"" character varying(100) NULL,
                ""CriteriaJson"" jsonb NULL,
                ""IsGlobal"" boolean NOT NULL,
                ""LastRunAt"" timestamp without time zone NULL,
                ""TenantId"" integer NOT NULL,
                ""CreationTime"" timestamp without time zone NOT NULL,
                ""CreatorUserId"" bigint NULL,
                ""LastModificationTime"" timestamp without time zone NULL,
                ""LastModifierUserId"" bigint NULL,
                ""IsDeleted"" boolean NOT NULL,
                ""DeleterUserId"" bigint NULL,
                ""DeletionTime"" timestamp without time zone NULL,
                CONSTRAINT ""PK_DmsSavedSearches"" PRIMARY KEY (""Id"")
            );");

        migrationBuilder.Sql(@"
            CREATE TABLE IF NOT EXISTS ""DmsRetentionPolicies"" (
                ""Id"" uuid NOT NULL,
                ""Name"" character varying(100) NULL,
                ""Description"" character varying(250) NULL,
                ""Trigger"" integer NOT NULL,
                ""RetentionDays"" integer NOT NULL,
                ""DispositionAction"" integer NOT NULL,
                ""AllowLegalHold"" boolean NOT NULL,
                ""IsActive"" boolean NOT NULL,
                ""FolderIds"" jsonb NULL,
                ""TenantId"" integer NOT NULL,
                ""CreationTime"" timestamp without time zone NOT NULL,
                ""CreatorUserId"" bigint NULL,
                ""LastModificationTime"" timestamp without time zone NULL,
                ""LastModifierUserId"" bigint NULL,
                ""IsDeleted"" boolean NOT NULL,
                ""DeleterUserId"" bigint NULL,
                ""DeletionTime"" timestamp without time zone NULL,
                CONSTRAINT ""PK_DmsRetentionPolicies"" PRIMARY KEY (""Id"")
            );");

        migrationBuilder.Sql(@"
            CREATE TABLE IF NOT EXISTS ""DmsLegalHolds"" (
                ""Id"" uuid NOT NULL,
                ""Name"" character varying(100) NULL,
                ""Reason"" character varying(250) NULL,
                ""DocumentIds"" jsonb NULL,
                ""FolderIds"" jsonb NULL,
                ""AppliedById"" bigint NOT NULL,
                ""ReleasedAt"" timestamp without time zone NULL,
                ""IsActive"" boolean NOT NULL,
                ""TenantId"" integer NOT NULL,
                ""CreationTime"" timestamp without time zone NOT NULL,
                ""CreatorUserId"" bigint NULL,
                ""LastModificationTime"" timestamp without time zone NULL,
                ""LastModifierUserId"" bigint NULL,
                ""IsDeleted"" boolean NOT NULL,
                ""DeleterUserId"" bigint NULL,
                ""DeletionTime"" timestamp without time zone NULL,
                CONSTRAINT ""PK_DmsLegalHolds"" PRIMARY KEY (""Id"")
            );");

        migrationBuilder.Sql(@"
            CREATE TABLE IF NOT EXISTS ""DmsSignatures"" (
                ""Id"" uuid NOT NULL,
                ""DocumentId"" uuid NOT NULL,
                ""VersionId"" uuid NOT NULL,
                ""SignerId"" bigint NOT NULL,
                ""SignatureType"" integer NOT NULL,
                ""SignatureImagePath"" character varying(500) NULL,
                ""CertificateThumbprint"" character varying(100) NULL,
                ""SignedHash"" character varying(64) NULL,
                ""SignedAt"" timestamp without time zone NOT NULL,
                ""IpAddress"" character varying(50) NULL,
                ""Reason"" character varying(200) NULL,
                ""TenantId"" integer NOT NULL,
                ""CreationTime"" timestamp without time zone NOT NULL,
                ""CreatorUserId"" bigint NULL,
                ""LastModificationTime"" timestamp without time zone NULL,
                ""LastModifierUserId"" bigint NULL,
                ""IsDeleted"" boolean NOT NULL,
                ""DeleterUserId"" bigint NULL,
                ""DeletionTime"" timestamp without time zone NULL,
                CONSTRAINT ""PK_DmsSignatures"" PRIMARY KEY (""Id"")
            );");

        migrationBuilder.Sql(@"
            CREATE TABLE IF NOT EXISTS ""DmsEntityLinks"" (
                ""Id"" uuid NOT NULL,
                ""EntityType"" integer NOT NULL,
                ""EntityId"" character varying(100) NULL,
                ""DocumentId"" uuid NOT NULL,
                ""LinkType"" integer NOT NULL,
                ""Note"" character varying(250) NULL,
                ""CreatedById"" bigint NOT NULL,
                ""TenantId"" integer NOT NULL,
                ""CreationTime"" timestamp without time zone NOT NULL,
                ""CreatorUserId"" bigint NULL,
                ""LastModificationTime"" timestamp without time zone NULL,
                ""LastModifierUserId"" bigint NULL,
                ""IsDeleted"" boolean NOT NULL,
                ""DeleterUserId"" bigint NULL,
                ""DeletionTime"" timestamp without time zone NULL,
                CONSTRAINT ""PK_DmsEntityLinks"" PRIMARY KEY (""Id"")
            );");

        migrationBuilder.Sql(@"
            CREATE TABLE IF NOT EXISTS ""DmsEmbeddings"" (
                ""Id"" uuid NOT NULL,
                ""DocumentId"" uuid NOT NULL,
                ""VersionId"" uuid NOT NULL,
                ""ChunkIndex"" integer NOT NULL,
                ""ChunkText"" text NULL,
                ""VectorJson"" jsonb NULL,
                ""Model"" character varying(50) NULL,
                ""TenantId"" integer NOT NULL,
                ""CreationTime"" timestamp without time zone NOT NULL,
                ""CreatorUserId"" bigint NULL,
                CONSTRAINT ""PK_DmsEmbeddings"" PRIMARY KEY (""Id"")
            );");

        migrationBuilder.Sql(@"
            CREATE TABLE IF NOT EXISTS ""DmsDocumentEvents"" (
                ""Id"" uuid NOT NULL,
                ""EventType"" character varying(100) NULL,
                ""AggregateId"" uuid NOT NULL,
                ""PayloadJson"" jsonb NULL,
                ""OccurredAt"" timestamp without time zone NOT NULL,
                ""ProcessedAt"" timestamp without time zone NULL,
                ""RetryCount"" integer NOT NULL,
                ""LastError"" character varying(500) NULL,
                ""TenantId"" integer NOT NULL,
                ""CreationTime"" timestamp without time zone NOT NULL,
                ""CreatorUserId"" bigint NULL,
                CONSTRAINT ""PK_DmsDocumentEvents"" PRIMARY KEY (""Id"")
            );");

        migrationBuilder.Sql(@"
            CREATE TABLE IF NOT EXISTS ""DmsQuotaUsages"" (
                ""Id"" uuid NOT NULL,
                ""BytesUsed"" bigint NOT NULL,
                ""DocumentCount"" integer NOT NULL,
                ""VersionCount"" integer NOT NULL,
                ""ComputedAt"" timestamp without time zone NOT NULL,
                ""TenantId"" integer NOT NULL,
                ""CreationTime"" timestamp without time zone NOT NULL,
                ""CreatorUserId"" bigint NULL,
                ""LastModificationTime"" timestamp without time zone NULL,
                ""LastModifierUserId"" bigint NULL,
                ""IsDeleted"" boolean NOT NULL,
                ""DeleterUserId"" bigint NULL,
                ""DeletionTime"" timestamp without time zone NULL,
                CONSTRAINT ""PK_DmsQuotaUsages"" PRIMARY KEY (""Id"")
            );");

        migrationBuilder.Sql(@"
            CREATE TABLE IF NOT EXISTS ""DmsIngestJobs"" (
                ""Id"" uuid NOT NULL,
                ""JobType"" integer NOT NULL,
                ""Status"" integer NOT NULL,
                ""TotalItems"" integer NOT NULL,
                ""ProcessedItems"" integer NOT NULL,
                ""FailedItems"" integer NOT NULL,
                ""SourcePath"" text NULL,
                ""ResultPath"" text NULL,
                ""ErrorLog"" text NULL,
                ""StartedAt"" timestamp without time zone NULL,
                ""CompletedAt"" timestamp without time zone NULL,
                ""CreatedById"" bigint NOT NULL,
                ""TenantId"" integer NOT NULL,
                ""CreationTime"" timestamp without time zone NOT NULL,
                ""CreatorUserId"" bigint NULL,
                ""LastModificationTime"" timestamp without time zone NULL,
                ""LastModifierUserId"" bigint NULL,
                ""IsDeleted"" boolean NOT NULL,
                ""DeleterUserId"" bigint NULL,
                ""DeletionTime"" timestamp without time zone NULL,
                CONSTRAINT ""PK_DmsIngestJobs"" PRIMARY KEY (""Id"")
            );");

        migrationBuilder.Sql(@"
            CREATE TABLE IF NOT EXISTS ""DmsSettings"" (
                ""Id"" uuid NOT NULL,
                ""StorageProvider"" character varying(30) NULL,
                ""LocalRootPath"" character varying(300) NULL,
                ""AllowedFileTypes"" jsonb NULL,
                ""MaxFileSizeMb"" integer NOT NULL,
                ""QuotaGb"" integer NOT NULL,
                ""OcrEnabled"" boolean NOT NULL,
                ""OcrLanguages"" jsonb NULL,
                ""AntivirusEnabled"" boolean NOT NULL,
                ""AiEnabled"" boolean NOT NULL,
                ""WatermarkText"" character varying(200) NULL,
                ""TrashRetentionDays"" integer NOT NULL,
                ""EnforceVersioning"" boolean NOT NULL,
                ""DefaultLanguage"" character varying(30) NULL,
                ""TenantId"" integer NOT NULL,
                ""CreationTime"" timestamp without time zone NOT NULL,
                ""CreatorUserId"" bigint NULL,
                ""LastModificationTime"" timestamp without time zone NULL,
                ""LastModifierUserId"" bigint NULL,
                ""IsDeleted"" boolean NOT NULL,
                ""DeleterUserId"" bigint NULL,
                ""DeletionTime"" timestamp without time zone NULL,
                CONSTRAINT ""PK_DmsSettings"" PRIMARY KEY (""Id"")
            );");

        // ---------------------------------------------------------------- indexes
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_Documents_TenantId_FolderId_IsDeleted"" ON ""Documents"" (""TenantId"", ""FolderId"", ""IsDeleted"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_Documents_TenantId_CollectionId"" ON ""Documents"" (""TenantId"", ""CollectionId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_Documents_Checksum"" ON ""Documents"" (""Checksum"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_Documents_TenantId_CustomId"" ON ""Documents"" (""TenantId"", ""CustomId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_Documents_CurrentVersionId"" ON ""Documents"" (""CurrentVersionId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_Documents_SearchVector"" ON ""Documents"" USING GIN (""SearchVector"");");

        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsDocumentVersions_DocumentId_VersionNumber_MinorNumber"" ON ""DmsDocumentVersions"" (""DocumentId"", ""VersionNumber"", ""MinorNumber"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsDocumentVersions_TenantId_Checksum"" ON ""DmsDocumentVersions"" (""TenantId"", ""Checksum"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsDocumentVersions_SearchVector"" ON ""DmsDocumentVersions"" USING GIN (""SearchVector"");");

        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_Folders_TenantId_ParentId"" ON ""Folders"" (""TenantId"", ""ParentId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_Folders_CollectionId"" ON ""Folders"" (""CollectionId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_Collections_TenantId"" ON ""Collections"" (""TenantId"");");

        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsAclEntries_SecurableType_SecurableId"" ON ""DmsAclEntries"" (""SecurableType"", ""SecurableId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsAclEntries_TenantId_SubjectType_SubjectId"" ON ""DmsAclEntries"" (""TenantId"", ""SubjectType"", ""SubjectId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsAuditEntries_TenantId_Sequence"" ON ""DmsAuditEntries"" (""TenantId"", ""Sequence"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsAuditEntries_DocumentId"" ON ""DmsAuditEntries"" (""DocumentId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsAuditEntries_FolderId"" ON ""DmsAuditEntries"" (""FolderId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsShares_DocumentId"" ON ""DmsShares"" (""DocumentId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsShares_FolderId"" ON ""DmsShares"" (""FolderId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsShares_TenantId_Token"" ON ""DmsShares"" (""TenantId"", ""Token"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsDocumentTags_DocumentId"" ON ""DmsDocumentTags"" (""DocumentId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsDocumentTags_TagId"" ON ""DmsDocumentTags"" (""TagId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsCustomAttributes_TemplateId"" ON ""DmsCustomAttributes"" (""TemplateId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsDocumentMetadataValues_DocumentId_AttributeId"" ON ""DmsDocumentMetadataValues"" (""DocumentId"", ""AttributeId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsWorkflowInstances_DocumentId"" ON ""DmsWorkflowInstances"" (""DocumentId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsWorkflowInstances_TenantId_Status"" ON ""DmsWorkflowInstances"" (""TenantId"", ""Status"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsWorkflowTasks_WorkflowInstanceId"" ON ""DmsWorkflowTasks"" (""WorkflowInstanceId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsWorkflowTasks_TenantId_AssignedTo_Status"" ON ""DmsWorkflowTasks"" (""TenantId"", ""AssignedTo"", ""Status"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsEntityLinks_EntityType_EntityId"" ON ""DmsEntityLinks"" (""EntityType"", ""EntityId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsEntityLinks_DocumentId"" ON ""DmsEntityLinks"" (""DocumentId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsEmbeddings_DocumentId_VersionId"" ON ""DmsEmbeddings"" (""DocumentId"", ""VersionId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsDocumentEvents_TenantId_ProcessedAt"" ON ""DmsDocumentEvents"" (""TenantId"", ""ProcessedAt"");");
        migrationBuilder.Sql(@"CREATE UNIQUE INDEX IF NOT EXISTS ""IX_DmsQuotaUsages_TenantId"" ON ""DmsQuotaUsages"" (""TenantId"");");
        migrationBuilder.Sql(@"CREATE UNIQUE INDEX IF NOT EXISTS ""IX_DmsSettings_TenantId"" ON ""DmsSettings"" (""TenantId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsRetentionPolicies_TenantId"" ON ""DmsRetentionPolicies"" (""TenantId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsLegalHolds_TenantId_IsActive"" ON ""DmsLegalHolds"" (""TenantId"", ""IsActive"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsSignatures_DocumentId"" ON ""DmsSignatures"" (""DocumentId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsSignatures_VersionId"" ON ""DmsSignatures"" (""VersionId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsAnnotations_DocumentId"" ON ""DmsAnnotations"" (""DocumentId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsComments_DocumentId"" ON ""DmsComments"" (""DocumentId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsSavedSearches_TenantId_UserId"" ON ""DmsSavedSearches"" (""TenantId"", ""UserId"");");
        migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_DmsIngestJobs_TenantId_Status"" ON ""DmsIngestJobs"" (""TenantId"", ""Status"");");

        // ---------------------------------------------------------------- backfill
        Backfill(migrationBuilder);
    }

    private static void Backfill(MigrationBuilder migrationBuilder)
    {
        // 1. a default collection per tenant that owns folders or documents
        migrationBuilder.Sql(@"
            INSERT INTO ""Collections"" (""Id"", ""Name"", ""Description"", ""OwnerId"", ""IsDefault"", ""StorageQuotaBytes"", ""TenantId"", ""CreationTime"", ""IsDeleted"")
            SELECT gen_random_uuid(), 'Default', 'Default document repository', 0, true, 0, t.""TenantId"", now(), false
              FROM (SELECT DISTINCT ""TenantId"" FROM ""Folders""
                    UNION
                    SELECT DISTINCT ""TenantId"" FROM ""Documents"") t
             WHERE NOT EXISTS (SELECT 1 FROM ""Collections"" c WHERE c.""TenantId"" = t.""TenantId"" AND c.""IsDefault"");");

        // 2. root folders inherit the default collection of their tenant
        migrationBuilder.Sql(@"
            UPDATE ""Folders"" f
               SET ""CollectionId"" = c.""Id""
              FROM ""Collections"" c
             WHERE c.""TenantId"" = f.""TenantId""
               AND c.""IsDefault""
               AND f.""CollectionId"" = " + EmptyGuid + ";");

        // 3. propagate collections down the folder tree
        migrationBuilder.Sql(@"
            WITH RECURSIVE tree AS (
                SELECT f.""Id"", f.""CollectionId""
                  FROM ""Folders"" f
                 WHERE f.""CollectionId"" <> " + EmptyGuid + @"
                UNION ALL
                SELECT child.""Id"", tree.""CollectionId""
                  FROM ""Folders"" child
                  JOIN tree ON child.""ParentId"" = tree.""Id""
                 WHERE child.""CollectionId"" = " + EmptyGuid + @"
            )
            UPDATE ""Folders"" f
               SET ""CollectionId"" = tree.""CollectionId""
              FROM tree
             WHERE f.""Id"" = tree.""Id"";");

        // 4. documents inherit the collection of their folder
        migrationBuilder.Sql(@"
            UPDATE ""Documents"" d
               SET ""CollectionId"" = f.""CollectionId""
              FROM ""Folders"" f
             WHERE d.""FolderId"" = f.""Id""
               AND d.""CollectionId"" = " + EmptyGuid + ";");

        // 5. derived values
        migrationBuilder.Sql(@"UPDATE ""Documents"" SET ""Extension"" = lower(substring(""FileName"" from '\.([^.]*)$')) WHERE ""Extension"" IS NULL AND ""FileName"" IS NOT NULL;");
        migrationBuilder.Sql(@"UPDATE ""Documents"" SET ""OwnerId"" = COALESCE(""CreatorUserId"", 0);");
        migrationBuilder.Sql(@"UPDATE ""Folders"" SET ""OwnerId"" = COALESCE(""CreatorUserId"", 0);");
        migrationBuilder.Sql(@"UPDATE ""Folders"" SET ""Depth"" = 0;");
        migrationBuilder.Sql(@"UPDATE ""DmsCustomIdentifiers"" SET ""IsActive"" = true;");
        migrationBuilder.Sql(@"UPDATE ""DmsWorkflowTemplates"" SET ""IsActive"" = true;");
        migrationBuilder.Sql(@"UPDATE ""DmsDocumentTemplates"" SET ""IsActive"" = true WHERE ""IsActive"" IS NULL;");
        migrationBuilder.Sql(@"UPDATE ""DmsDocumentTemplates"" SET ""VersionScheme"" = 0 WHERE ""VersionScheme"" IS NULL;");
        migrationBuilder.Sql(@"UPDATE ""DmsWorkflowInstances"" SET ""Status"" = 'In-Progress' WHERE ""Status"" IS NULL;");
        migrationBuilder.Sql(@"UPDATE ""DmsWorkflowTasks"" SET ""StepIndex"" = 0 WHERE ""StepIndex"" IS NULL;");
        migrationBuilder.Sql(@"UPDATE ""DmsWorkflowTasks"" SET ""StepName"" = 'Step' WHERE ""StepName"" IS NULL;");
        migrationBuilder.Sql(@"UPDATE ""DmsWorkflowTasks"" SET ""DelegatedFrom"" = NULL WHERE ""DelegatedFrom"" IS NULL;");
        migrationBuilder.Sql(@"UPDATE ""DmsComments"" SET ""ResolvedById"" = NULL WHERE ""ResolvedById"" IS NULL;");

        // 6. create version 1 rows for the existing documents
        migrationBuilder.Sql(@"
            INSERT INTO ""DmsDocumentVersions"" (
                ""Id"", ""DocumentId"", ""VersionNumber"", ""MinorNumber"", ""VersionLabel"", ""ChangeNote"",
                ""FileName"", ""Extension"", ""FileSize"", ""MimeType"", ""StorageProvider"", ""StorageKey"",
                ""Checksum"", ""IsCurrent"", ""PageCount"", ""ExtractedText"", ""OcrApplied"", ""Language"",
                ""TenantId"", ""CreationTime"", ""CreatorUserId"", ""IsDeleted"")
            SELECT gen_random_uuid(),
                   d.""Id"",
                   1,
                   0,
                   '1.0',
                   'Initial upload (migrated)',
                   d.""FileName"",
                   d.""Extension"",
                   d.""FileSize"",
                   d.""MimeType"",
                   COALESCE(d.""StorageProvider"", 'Cloudinary'),
                   COALESCE(d.""StorageKey"", d.""FilePath""),
                   d.""Checksum"",
                   true,
                   0,
                   NULL,
                   false,
                   'en',
                   d.""TenantId"",
                   d.""CreationTime"",
                   d.""CreatorUserId"",
                   false
              FROM ""Documents"" d
             WHERE NOT EXISTS (SELECT 1 FROM ""DmsDocumentVersions"" v WHERE v.""DocumentId"" = d.""Id"");");

        // 7. point documents at their current version
        migrationBuilder.Sql(@"
            UPDATE ""Documents"" d
               SET ""CurrentVersionId"" = v.""Id"",
                   ""IndexStatus"" = 1
              FROM ""DmsDocumentVersions"" v
             WHERE v.""DocumentId"" = d.""Id""
               AND v.""IsCurrent""
               AND d.""CurrentVersionId"" IS NULL;");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsIngestJobs_TenantId_Status"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsSavedSearches_TenantId_UserId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsComments_DocumentId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsAnnotations_DocumentId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsSignatures_VersionId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsSignatures_DocumentId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsLegalHolds_TenantId_IsActive"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsRetentionPolicies_TenantId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsSettings_TenantId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsQuotaUsages_TenantId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsDocumentEvents_TenantId_ProcessedAt"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsEmbeddings_DocumentId_VersionId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsEntityLinks_DocumentId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsEntityLinks_EntityType_EntityId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsWorkflowTasks_TenantId_AssignedTo_Status"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsWorkflowTasks_WorkflowInstanceId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsWorkflowInstances_TenantId_Status"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsWorkflowInstances_DocumentId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsDocumentMetadataValues_DocumentId_AttributeId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsCustomAttributes_TemplateId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsDocumentTags_TagId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsDocumentTags_DocumentId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsShares_TenantId_Token"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsShares_FolderId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsShares_DocumentId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsAuditEntries_FolderId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsAuditEntries_DocumentId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsAuditEntries_TenantId_Sequence"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsAclEntries_TenantId_SubjectType_SubjectId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsAclEntries_SecurableType_SecurableId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Collections_TenantId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Folders_CollectionId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Folders_TenantId_ParentId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsDocumentVersions_SearchVector"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsDocumentVersions_TenantId_Checksum"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_DmsDocumentVersions_DocumentId_VersionNumber_MinorNumber"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Documents_SearchVector"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Documents_CurrentVersionId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Documents_TenantId_CustomId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Documents_Checksum"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Documents_TenantId_CollectionId"";");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Documents_TenantId_FolderId_IsDeleted"";");

        foreach (var table in new[]
        {
            "DmsSettings", "DmsIngestJobs", "DmsQuotaUsages", "DmsDocumentEvents", "DmsEmbeddings",
            "DmsEntityLinks", "DmsSignatures", "DmsLegalHolds", "DmsRetentionPolicies", "DmsSavedSearches",
            "DmsShares", "DmsAuditEntries", "DmsAclEntries", "DmsDocumentVersions", "DmsAnnotations"
        })
        {
            migrationBuilder.Sql($"DROP TABLE IF EXISTS \"{table}\";");
        }

        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowTasks"" DROP COLUMN ""SignatureId"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowTasks"" DROP COLUMN ""OutcomeJson"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowTasks"" DROP COLUMN ""DelegatedFrom"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowTasks"" DROP COLUMN ""DueDate"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowTasks"" DROP COLUMN ""StepName"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowTasks"" DROP COLUMN ""StepIndex"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowInstances"" DROP COLUMN ""VariablesJson"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowInstances"" DROP COLUMN ""TemplateSnapshotJson"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowInstances"" DROP COLUMN ""LastActionAt"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowInstances"" DROP COLUMN ""DueDate"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowInstances"" DROP COLUMN ""StartedById"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowInstances"" DROP COLUMN ""TemplateName"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowTemplates"" DROP COLUMN ""Version"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsWorkflowTemplates"" DROP COLUMN ""IsActive"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsComments"" DROP COLUMN ""ResolvedAt"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsComments"" DROP COLUMN ""ResolvedById"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsComments"" DROP COLUMN ""AnchorJson"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsComments"" DROP COLUMN ""VersionId"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsCustomIdentifiers"" DROP COLUMN ""LastGeneratedAt"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsCustomIdentifiers"" DROP COLUMN ""ResetPolicy"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsCustomIdentifiers"" DROP COLUMN ""IsActive"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsCustomAttributes"" DROP COLUMN ""ValidationRegex"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsCustomAttributes"" DROP COLUMN ""IsSensitive"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsCustomAttributes"" DROP COLUMN ""IsSearchable"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsDocumentTemplates"" DROP COLUMN ""RequiresApproval"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsDocumentTemplates"" DROP COLUMN ""VersionScheme"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsDocumentTemplates"" DROP COLUMN ""RetentionPolicyId"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsDocumentTemplates"" DROP COLUMN ""WorkflowTemplateId"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsDocumentTemplates"" DROP COLUMN ""AllowedFileTypes"";");
        migrationBuilder.Sql(@"ALTER TABLE ""DmsTags"" DROP COLUMN ""IsSystem"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Collections"" DROP COLUMN ""StorageQuotaBytes"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Collections"" DROP COLUMN ""IsDefault"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Folders"" DROP COLUMN ""Depth"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Folders"" DROP COLUMN ""IsSystem"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Folders"" DROP COLUMN ""InheritPermissions"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Folders"" DROP COLUMN ""IdentifierId"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Folders"" DROP COLUMN ""TemplateId"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Folders"" DROP COLUMN ""OwnerId"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Folders"" DROP COLUMN ""CollectionId"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Folders"" ALTER COLUMN ""ParentId"" SET NOT NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""Folders"" ALTER COLUMN ""ParentId"" SET DEFAULT " + EmptyGuid + ";");
        migrationBuilder.Sql(@"UPDATE ""Folders"" SET ""ParentId"" = " + EmptyGuid + @" WHERE ""ParentId"" IS NULL;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""SearchVector"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""ThumbnailPath"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""PreviewPath"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""AiStatus"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""OcrStatus"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""ScanStatus"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""IndexedAt"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""IndexStatus"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""IsWorm"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""IsOnLegalHold"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""ExpiryDate"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""RetentionEventDate"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""RetentionPolicyId"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""LockExpiry"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""CheckedOutAt"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""CheckedOutBy"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""IsCheckedOut"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""OwnerId"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""CurrentVersionId"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""StorageKey"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""StorageProvider"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""Language"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""PageCount"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ALTER COLUMN ""FileSize"" TYPE integer;");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""CustomId"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""Extension"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN ""CollectionId"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" ADD COLUMN ""VersionId"" character varying(64) NULL;");
    }
}
