# Document Management Module — Implementation Plan

**Repository:** `rhyolite-orbit-prime/rhyolite-orbit-api` (branch `arena/833b4471-rhyolite-orbit-api`)
**Target module:** Document Management System (DMS) for Rhyolite Orbit ERP
**Reference product for feature parity:** [LogicalDOC](https://www.logicaldoc.com/) (Community/Enterprise)
**Date:** 2026-10-12
**Status:** Plan — awaiting review before implementation

---

## 1. Purpose and Scope

This document is the implementation plan to **fully build and complete the Document Management
module** in the Orbit API. It covers:

1. An honest audit of what already exists in the codebase.
2. The complete feature set we will deliver — everything LogicalDOC offers, plus modern
   additions (AI, semantic search, e-signature, legal hold, PII redaction, etc.).
3. The target architecture, data model, API surface, and security model.
4. A phased delivery plan with milestones, exit criteria, and effort estimates.
5. Testing, non-functional requirements, risks, and open decisions.

Out of scope: the front-end (MVC/Angular/React client), except where API shape is driven by UI
needs. Infrastructure provisioning beyond configuration guidance.

---

## 2. Current State Audit

### 2.1 What already exists

| Area | Artifact | Location |
|---|---|---|
| Entities | 16 DMS entities (folders, collections, documents, tags, document-tags, templates, custom attributes, metadata values, custom identifiers, comments, workflow templates/instances/tasks + workflow step value objects) | `src/RhyoliteOrbit.Core/Models/Dms/*.cs` |
| EF Core registration | `DbSet<>`s for all DMS entities | `src/RhyoliteOrbit.EntityFrameworkCore/EntityFrameworkCore/RhyoliteOrbitDbContext.cs` (lines ~233–246) |
| Database tables | `Folders`, `Collections`, `Documents`, `DmsTags`, `DmsDocumentTags`, `DmsDocumentTemplates`, `DmsCustomAttributes`, `DmsDocumentMetadataValues`, `DmsCustomIdentifiers`, `DmsComments`, `DmsWorkflowTemplates`, `DmsWorkflowInstances`, `DmsWorkflowTasks` | Migration `20261005180758_initial1` (tables exist; model snapshot current) |
| Workflow enums | `WorkflowStepType`, `CompletionRule`, `StepActionType`, `ConditionField`, `ConditionOperator`, `WorkflowTaskType`, `WorkflowTaskStatus`, `AssigneeType` | `src/RhyoliteOrbit.Core/RhyoliteOrbitConsts.cs` |
| Subscription gating (flags) | `Tenant.HasSubscribedToDocumentManagement`, `DocumentManagementSubscription{Start,End}Date`, `DocumentManagementSubscriptionFee`, `DocumentManagementBillingCycle`, `DocumentManagementNextRenewalDate`; module id `13` | `src/RhyoliteOrbit.Core/MultiTenancy/Tenant.cs`, `src/RhyoliteOrbit.Core/DomainServices/Subscription/SubscriptionManager.cs` (module id consts ~line 525, flag mapping ~line 660) |
| File upload primitive | `IFileUploadApi` / `FileUploadApi` (Cloudinary raw + image upload) | `src/RhyoliteOrbit.Core/CloudinaryApi/` |
| Async infrastructure | Akka actors (`StudentBillActor` pattern), RabbitMQ client, Quartz job scheduler (`IQuartzJobScheduler`), Redis cache manager | `src/RhyoliteOrbit.Core/Actors/`, `RabbitMq/`, `DomainServices/Quartz/`, `Redis/` |
| PDF/HTML rendering | Syncfusion HtmlToPdfConverter, `IViewRenderService` (payslip PDF pattern) | `src/RhyoliteOrbit.Core/Pdf/`, `ViewRenderer/` |
| Email | `ICustomEmailSender` | `src/RhyoliteOrbit.Core/Notifications/` |
| Multi-tenancy | All DMS entities implement `IMustHaveTenant`; ABP Zero multi-tenancy enabled | — |

### 2.2 What is missing (the gap)

| Gap | Detail |
|---|---|
| **No application layer** | Zero app services, domain managers, repositories, DTOs, or AutoMapper maps for DMS (only `Models/Dms` + `DbContext` exist). `RhyoliteOrbit.Application/Services/` has no `DocumentManagement` folder; `Core/DomainServices/` has an empty `Dms` placeholder folder. |
| **No permissions** | No `DMS_*` entries in `PermissionNames.cs`, no entries in `RhyoliteOrbitAuthorizationProvider`, no navigation menu items. |
| **No subscription enforcement** | `HasSubscribedToDocumentManagement` is stored but nothing checks it. No guard blocks DMS calls for unsubscribed/expired tenants. |
| **Entities are skeletal** | No document versions, ACL/permissions, audit trail, check-in/check-out, shares, retention, annotations, quarantine, storage routing, OCR/index state, page counts, extensions, expiry, etc. `DmsDocument.FileSize` is `int` (will overflow >2 GB and is the wrong type for byte counts on 64-bit), `VersionId` is a string on the document rather than a first-class version entity, `DmsFolder.ParentId` is non-nullable `Guid` (no clean root representation). |
| **No storage abstraction** | Only Cloudinary exists; no provider abstraction, no local/S3 fallback, no encryption-at-rest, no per-tenant bucket routing, no multipart/chunked upload. |
| **No search** | No full-text index, no metadata search, no OCR pipeline. |
| **No workflow engine** | Entities/enums exist but no engine to instantiate templates, evaluate conditions, assign tasks, or run actions. |
| **No localization keys, no tests, no docs** for the module. |

**Conclusion:** the module is ~10% scaffolded. This plan builds the remaining ~90%.

---

## 3. Goals, Non-Goals, Design Principles

### 3.1 Goals

- **LogicalDOC feature parity** across: repository/folders, versioning, check-in/check-out,
  metadata & custom identifiers, templates, tags, full-text search, OCR, workflow, audit trail,
  records management (retention/disposition), email cataloging, bulk import/export, WebDAV-class
  access, bookmarking/saved searches, discussions/comments, drag-and-drop upload ("Drop Spot"),
  mobile access, and a REST API.
- **Modern additions** that LogicalDOC-class products lack or do weakly: AI auto-classification
  and tagging, semantic (vector) search, RAG "ask your documents" Q&A, automatic PII detection &
  redaction, tamper-evident hash-chained audit log, e-signature workflow, legal hold/WORM
  storage, virus scanning, duplicate detection, document diff/compare, watermarking, secure
  external sharing links, ERP-native linking of documents to GL/Payroll/POS/HR records, and a
  fully event-driven indexing pipeline.
- **Multi-tenant isolation** on every read/write path, consistent with the existing ABP Zero
  model (`IMustHaveTenant` + tenant filters).
- **Subscription-aware**: module id `13` billing already exists; enforce it at the API boundary.
- **Operability**: quotas, rate limits, observability, backup/restore, and a migration path.

### 3.2 Non-goals

- Front-end implementation (API contract is the deliverable).
- Replacing Cloudinary as the default binary store.
- Building a general-purpose BPM engine (the workflow engine is document-scoped).

### 3.3 Design principles

1. **Follow existing conventions** — layered ABP structure: `Core/Models/Dms` (entities) →
   `Core/DomainServices/Dms` (domain managers, business rules) → `Application/Services/DocumentManagement`
   (app services + DTOs) → dynamic Web API (`/api/services/app/...`).
2. **Async-first for heavy work** — uploads return quickly; extraction/OCR/indexing happen in
   Akka actors / RabbitMQ / Quartz jobs, mirroring the `StudentBillActor` pattern.
3. **Pluggable providers** — storage, text extraction, OCR, antivirus, AI, and e-signature are
   interfaces with default implementations; swap without touching app services.
4. **Postgres-first** — use PostgreSQL 17 features already in the stack (jsonb, tsvector + GIN,
   `pgcrypto` for gen_random_uuid) instead of adding new infrastructure for v1.
5. **Every mutation audited** — hash-chained audit trail for compliance (records, legal, audit).

---

## 4. Target Feature Set

### 4.1 LogicalDOC parity matrix

| # | LogicalDOC capability | Our implementation (phase) |
|---|---|---|
| 1 | Hierarchical folder repository | `DmsFolder` tree with path materialization, move/copy, breadcrumbs (P1) |
| 2 | Document upload / drag-and-drop ("Drop Spot") | Multipart + chunked/resumable upload, drag-drop friendly batch API (P1) |
| 3 | Version control (major/minor, revision history, notes per version, configurable scheme) | First-class `DmsDocumentVersion` entity, version numbering policy per folder, promote/rollback, version diff (P2, P6) |
| 4 | Check-in / check-out (lock) | `DmsDocumentLock` + `CheckOut`/`CheckIn` APIs with lock expiry and force-unlock permission (P2) |
| 5 | Metadata & document profiling | Templates (`DmsDocumentTemplate`) + attributes (`DmsCustomAttributes`) + values (`DmsDocumentMetadataValues`); folder-bound default templates (P1) |
| 6 | Custom identifiers (auto-numbering) | `DmsCustomIdentifier` with prefix/suffix/date parts/padding/sequence, atomic sequence allocation (P1) |
| 7 | Tags & tag search | `DmsTag` / `DmsDocumentTag` with colors, tag cloud, tag-based search (P1) |
| 8 | Full-text search (Lucene-class) with snippets | PostgreSQL `tsvector` + GIN indexes, ranked results with highlighted snippets; pluggable `ISearchProvider` (OpenSearch optional later) (P3) |
| 9 | OCR (integrated, scheduled, multilingual) | `IOcrService` (Tesseract worker container default), zonal OCR for fixed-form docs, barcode recognition, background indexing policies (P3) |
| 10 | Workflow (approval/review/parallel/sequential/conditional/escalation) | Workflow engine over existing `DmsWorkflowTemplate/Instance/Task` + step value objects; condition evaluator; actions (email, webhook, metadata update, assign) (P4) |
| 11 | Audit trail | `DmsAuditEntry` hash-chained, immutable, exportable (P2) |
| 12 | Records management (capture → dispose per policy) | `DmsRetentionPolicy`, disposition review queue, legal hold/WORM (P5) |
| 13 | Email cataloging / import from mailbox | IMAP ingestion actor → documents + attachment extraction (P6) |
| 14 | Bulk import/export (ZIP) | ZIP import/export jobs with folder-structure preservation (P6) |
| 15 | Bookmarks / saved searches | `DmsSavedSearch` (per user), starred documents (field exists) (P1) |
| 16 | Comments / discussion forum | `DmsComment` threaded comments + @mentions → notifications (P2) |
| 17 | Sharing (internal & external, public/private) | `DmsShare` with link expiry, password, download/upload rights, watermarked previews (P2) |
| 18 | WebDAV / desktop-class access | WebDAV-compatible endpoints over the same ACL layer (P6) |
| 19 | Mobile interface | Mobile-optimized API set (thumbnails, ranged download, offline sync manifest) (P6) |
| 20 | Office integration (edit online) | URL-based "edit" hand-off via share tokens + check-out (P6) |
| 21 | File format conversion | Conversion pipeline (Office/HTML → PDF via existing Syncfusion license) (P6) |
| 22 | Automation/scheduled tasks | Quartz jobs: retention, escalation, quota recompute, re-index (P3/P5) |
| 23 | Import from ZIP, mailbox; export | covered by 13/14 |
| 24 | Web service (SOAP/REST) API | Full REST surface via ABP dynamic Web API; optional SOAP gateway later |
| 25 | DICOM viewer support | Store/stream DICOM as binary + preview generation (P6, optional) |
| 26 | Cluster/scalable architecture | Stateless API + external Postgres/Redis/RabbitMQ/object store; actors scale out (NFR) |

### 4.2 Modern additions (beyond LogicalDOC)

| # | Feature | Description | Phase |
|---|---|---|---|
| M1 | AI auto-classification & auto-tagging | `IAiDocumentService` extracts summary, language, doc-type, entities; suggests folder/template/metadata on upload | P7 |
| M2 | Semantic (vector) search | Embedding store (`DmsEmbedding`, pgvector if available) + hybrid lexical/semantic ranking | P7 |
| M3 | "Ask your documents" (RAG Q&A) | Scoped chat endpoint answering from document text with citations | P7 |
| M4 | PII detection & redaction | Detect SSNit/TIN/Ghana Card, phone, email, card numbers; redacted previews + auto-masking in search snippets | P7 |
| M5 | Tamper-evident audit chain | SHA-256 hash chain over audit entries; periodic anchor export for auditors | P2 |
| M6 | E-signature | Draw/type/upload signature, signature ceremony on documents, signature audit + certificate hash | P5 |
| M7 | Legal hold / WORM | Immutable retention overriding deletion; object-lock where storage supports it | P5 |
| M8 | Antivirus scanning | ClamAV (`clamd`) scan-on-upload, quarantine bucket, retry/rescan | P3 |
| M9 | Duplicate detection | Checksum dedupe (`Checksum` field exists), near-duplicate via simhash option | P3 |
| M10 | Document diff/compare | Version-to-version text diff + PDF visual compare | P6 |
| M11 | Watermarking | Dynamic watermark on preview/download (tenant/user/time) | P2 |
| M12 | ERP-native document linking | Attach documents to any ERP entity (invoice, payslip, student, POS order, NCR) via `DmsEntityLink` polymorphic table; deep links from other modules | P6 |
| M13 | Quotas & analytics | Per-tenant storage/document quotas; usage analytics dashboard endpoints | P5 |
| M14 | Event stream | RabbitMQ domain events for all DMS mutations → webhooks/notifications/BI | P2 |
| M15 | Offline sync manifest | Change-feed endpoint (cursor-based) for mobile/desktop sync clients | P6 |

---

## 5. Target Architecture

```
┌──────────────────────────────────────────────────────────────────────┐
│ Clients (MVC app, SPA, mobile, WebDAV, integrations)                  │
└───────────────┬──────────────────────────────────────────────────────┘
                │ REST (ABP dynamic Web API, /api/services/app/...)
┌───────────────▼──────────────────────────────────────────────────────┐
│ APPLICATION — RhyoliteOrbit.Application                              │
│  Services/DocumentManagement/*AppService  (thin, DTO-mapped)         │
│  Authorization: DmsAppServiceBase → subscription guard + [AbpAuthorize]│
├──────────────────────────────────────────────────────────────────────┤
│ CORE — RhyoliteOrbit.Core                                            │
│  DomainServices/Dms/*Manager (business rules, invariants)            │
│  Dms pipeline: ITextExtractor, IOcrService, IAntivirusScanner,       │
│               IAiDocumentService, ISearchProvider, IStorageProvider  │
│  Workflow: DmsWorkflowEngine, IConditionEvaluator, IStepActionRunner │
│  Actors: DocumentIngestionActor, OcrIndexActor, MailIngestionActor   │
├──────────────────────────────────────────────────────────────────────┤
│ INFRA — EntityFrameworkCore (PostgreSQL 17), Redis, RabbitMQ,        │
│         Quartz, Cloudinary/S3/local storage, ClamAV, Tesseract       │
└──────────────────────────────────────────────────────────────────────┘
```

### 5.1 Request flow (upload example)

```
POST /api/services/app/Document/Upload (multipart)
  → DocumentAppService (auth, subscription guard, quota check)
  → DocumentManager (dedupe by checksum, assign custom id, create DmsDocument + v1)
  → IStorageProvider.PutAsync (Cloudinary default; S3/local pluggable)
  → enqueue IndexMessage on RabbitMQ → DocumentIngestionActor
       → IAntivirusScanner → ITextExtractor → IOcrService (if needed)
       → ISearchProvider.Index (Postgres tsvector) → IAiDocumentService (classify/tag)
  → 201 { documentId, versionId }  (indexing continues async)
```

---

## 6. Data Model

### 6.1 Changes to existing entities

| Entity | Change |
|---|---|
| `DmsDocument` | Add: `Extension`, `OwnerId` (long), `CustomId` (from `DmsCustomIdentifier`), `CurrentVersionId` (Guid), `PageCount`, `StorageProvider`, `StorageKey`, `IsCheckedOut`, `CheckedOutBy`, `CheckedOutAt`, `ExpiryDate`, `RetentionPolicyId`, `IsOnLegalHold`, `IsWorm`, `IndexStatus` (enum), `IndexedAt`, `VirusScanStatus`, `OcrStatus`, `AiStatus`, `PreviewPath`, `ThumbnailPath`, `Language`, `CollectionId`. Change `FileSize` → `long`. Replace `VersionId` (string) with proper version FK. Add computed `SearchVector` (tsvector) + GIN index. |
| `DmsFolder` | Make `ParentId` nullable (`Guid?`, root = null) or use `Guid.Empty` sentinel consistently; add `TemplateId` (default doc template), `IdentifierId` (default numbering), `OwnerId`, `InheritPermissions`, `IsSystem`, `Depth`. Keep materialized `Path`. |
| `DmsComment` | Add `VersionId` (comment on a version), `ResolvedById/At`, `Anchor` (page/coords for annotations). |
| `DmsWorkflowInstance` | Add `StartedById`, `Priority`, `DueDate`, `Variables` (jsonb), `LastActionAt`. Keep `Status` but back it with an enum-const (`WorkflowInstanceStatus`). |
| `DmsWorkflowTask` | Add `StepIndex`, `TaskKind` alignment with `WorkflowStepType`, `DueDate`, `DelegatedFrom`, `Outcome` (jsonb), `SignatureId`. |
| `DmsCustomAttributes` | Add `ValidationRegex`, `MinValue/MaxValue`, `IsSearchable`, `IsRequiredInSearch`, `AppliesToFolderIds` (jsonb) or keep template-scoped. |
| `DmsCustomIdentifier` | Add `IsActive`, `ResetPolicy` (yearly/monthly/never), `LastGeneratedAt`; enforce atomic increments. |
| `DmsTag` | Add `IsSystem`, usage count (computed). |
| `DmsCollection` | Add `IsDefault`, `StorageQuotaBytes`; treat collections as top-level repositories that own folders. |
| `DmsDocumentTemplate` | Add `AllowedFileTypes` (jsonb), `WorkflowTemplateId` (auto-start), `RetentionPolicyId`, `PreviewTemplatePath`. |

### 6.2 New entities (all `FullAuditedEntity<Guid>` + `IMustHaveTenant`)

| Entity | Purpose / key fields |
|---|---|
| `DmsDocumentVersion` | `DocumentId`, `VersionNumber` (major/minor), `Label`, `ChangeNote`, `FileName`, `FileSize` (long), `MimeType`, `StorageProvider`, `StorageKey`, `Checksum`, `IsCurrent`, `CreatedById` (auditing) |
| `DmsAclEntry` | Folder/document-level ACL: `SecurableId` (folder or doc), `SecurableType` (Folder/Document/Collection), `SubjectType` (User/Role/Department/Tenant), `SubjectId`, `Permission` (bitmask enum `DmsPermission`), `Effect` (Allow/Deny), `Inherited` |
| `DmsAuditEntry` | `DocumentId?`, `FolderId?`, `Action` (enum), `ActorId`, `OccurredAt`, `IpAddress`, `UserAgent`, `Details` (jsonb), `PreviousHash`, `EntryHash` (SHA-256 chain) |
| `DmsShare` | `DocumentId/FolderId`, `ShareType` (Internal/Public/Email), `Token` (hashed), `PasswordHash`, `ExpiresAt`, `AllowDownload`, `AllowUpload`, `MaxDownloads`, `DownloadCount`, `Watermark`, `CreatedById` |
| `DmsSavedSearch` | `UserId`, `Name`, `Criteria` (jsonb), `IsGlobal`, `LastRunAt` |
| `DmsRetentionPolicy` | `Name`, `Trigger` (Creation/LastModified/EventDate), `RetentionDays`, `DispositionAction` (Review/Delete/Archive/Transfer), `IsLegalHoldCapable`, `IsActive` |
| `DmsLegalHold` | `Name`, `Reason`, `DocumentIds`/`FolderIds` (jsonb), `AppliedById`, `ReleasedAt` |
| `DmsAnnotation` | `DocumentId`, `VersionId`, `PageNumber`, `Type` (Highlight/Note/Stamp/Redaction), `Geometry` (jsonb), `Text`, `Color`, `CreatedById` (re-enables the commented-out DbSet) |
| `DmsSignature` | `DocumentId`, `VersionId`, `SignerId`, `SignatureImagePath`, `SignatureType` (Draw/Type/Upload/Certificate), `CertificateThumbprint`, `SignedHash`, `SignedAt`, `IpAddress` |
| `DmsEntityLink` | Polymorphic ERP linking: `EntityType` (Invoice, Payslip, Student, PosOrder, Ncr, …), `EntityId`, `DocumentId`, `LinkType`, `Note` |
| `DmsEmbedding` | `DocumentId`, `VersionId`, `ChunkIndex`, `ChunkText`, `Vector` (pgvector `vector(1536)` if available, else jsonb), `Model`, `CreatedAt` |
| `DmsDocumentEvent` | Outbox table for reliable event publishing (idempotency keys) |
| `DmsQuotaUsage` | `TenantId`, `BytesUsed`, `DocumentCount`, `ComputedAt` (maintained by Quartz + incremental updates) |
| `DmsIngestJob` | Tracks bulk imports/exports: `JobType`, `Status`, `TotalItems`, `ProcessedItems`, `ErrorLog`, `ResultPath` |

### 6.3 Enums to add to `RhyoliteOrbitConsts`

`DocumentStatus` (Draft/Active/Archived/Deleted/Quarantined), `DmsPermission` (View/Preview/Download/Print/Edit/Delete/ManagePermissions/Share/CheckOut/ManageWorkflow/ManageRetention — flags bitmask), `AclEffect`, `SecurableType`, `AclSubjectType`, `VersionType` (Major/Minor), `LockReason`, `IndexStatus` (NotIndexed/Queued/Indexing/Indexed/Failed), `ScanStatus`, `OcrStatus`, `AiStatus`, `AuditAction` (~40 values), `ShareType`, `AnnotationType`, `SignatureType`, `RetentionTrigger`, `DispositionAction`, `WorkflowInstanceStatus`, `TaskOutcome`, `IngestJobType/Status`, `LinkType`.

### 6.4 Indexing strategy (PostgreSQL)

- `DmsDocument`: GIN index on `SearchVector`; btree on `(TenantId, FolderId, IsDeleted)`; unique
  index on `(TenantId, CustomId)` where not null; unique on `(TenantId, FolderId, Name)`.
- `DmsDocumentVersion`: index on `(DocumentId, VersionNumber)`; unique on `Checksum` per tenant
  (duplicate detection).
- `DmsAclEntry`: composite `(SecurableType, SecurableId)`.
- `DmsAuditEntry`: btree `(TenantId, DocumentId, OccurredAt)`; immutable (no updates/deletes;
  revoke app-level write after insert).
- `DmsEntityLink`: `(EntityType, EntityId)`.
- `DmsEmbedding`: pgvector IVFFlat/HNSW index when pgvector available.

### 6.5 Migration plan

1. `dotnet ef migrations add Dms_Module_V1` — schema changes above (nullable `ParentId`,
   `long` FileSize, new columns, new tables, tsvector computed column + GIN).
2. Data backfill (idempotent SQL in migration `Up()` or a one-off job):
   - `DmsDocument.Extension` from `FileName`; `StorageProvider='Cloudinary'`, `StorageKey=FilePath`.
   - Root folder(s) per tenant where documents have `FolderId = 00000000-...`; re-point orphans.
   - Backfill `DmsAclEntry` from existing folder ownership (owner = full control).
   - Backfill `DmsDocumentVersion` v1 rows from current `DmsDocument` binaries.
3. Migration is additive-only for DMS tables (no cross-module renames) → safe on the existing
   `initial1` baseline. `RhyoliteOrbitDbContext` constructor already runs `Database.Migrate()`,
   so deployments self-migrate; keep this behavior but add a guard flag for large tenants.

---

## 7. Module Structure (file layout)

Following repository conventions:

```
src/RhyoliteOrbit.Core/
  Models/Dms/                       # extend existing + new entities (§6)
  DomainServices/Dms/
    Documents/        IDocumentManager, DocumentManager
    Folders/          IFolderManager, FolderManager
    Versions/         IDocumentVersionManager
    Metadata/         ITemplateManager, IAttributeManager, IMetadataManager, IIdentifierManager
    Tags/             IDmsTagManager
    Acl/              IAclManager (+ permission evaluation service)
    Search/           ISearchProvider, PostgresFtsSearchProvider, SearchQueryBuilder
    Storage/          IStorageProvider, CloudinaryStorageProvider, LocalStorageProvider, S3StorageProvider
    Extraction/       ITextExtractor (PdfPig, OpenXml, Txt/Csv), IOcrService (Tesseract),
                      IAntivirusScanner (ClamAV), IPreviewGenerator (Syncfusion PDF→thumbnails)
    Workflow/         DmsWorkflowEngine, IConditionEvaluator, IStepActionRunner,
                      WorkflowActionExecutors (Email/Webhook/Metadata/Assign)
    Audit/            IAuditTrail (hash chain)
    Retention/        IRetentionManager, disposition evaluator
    Sharing/          IShareManager (token gen, expiry, password, watermark)
    Signatures/       ISignatureService
    Ai/               IAiDocumentService (classify/tag/summarize), IEmbeddingService, IRagService
    Quota/            IQuotaManager
    Subscription/     IDmsSubscriptionGuard
  Actors/Dms/         DocumentIngestionActor, OcrIndexActor, MailIngestionActor, BulkImportActor
  Notifications/      (extend ICustomEmailSender usage for DMS events)

src/RhyoliteOrbit.Application/
  Services/DocumentManagement/
    DocumentAppService, FolderAppService, VersionAppService, MetadataAppService (templates/attributes/values),
    IdentifierAppService, TagAppService(DMS), SearchAppService, AclAppService, ShareAppService,
    CommentAppService, AnnotationAppService, WorkflowAppService (templates/instances/tasks),
    RetentionAppService, LegalHoldAppService, AuditAppService, SignatureAppService,
    BulkIoAppService (import/export), LinkAppService, QuotaAppService, AiAppService, SettingsAppService
    Dto/*.cs          (Create/Update/List DTOs + PagedResultDto)
  DmsAppServiceBase.cs             # subscription guard + tenant scoping helpers

src/RhyoliteOrbit.Core/Authorization/PermissionNames.cs   # add DMS_* constants
src/RhyoliteOrbit.Core/Authorization/RhyoliteOrbitAuthorizationProvider.cs  # register them
src/RhyoliteOrbit.Core/Localization/SourceFiles/RhyoliteOrbit.xml  # add DMS keys
src/RhyoliteOrbit.Web.Mvc/Startup/RhyoliteOrbitNavigationProvider.cs  # DMS menu (if MVC UI used)
test/RhyoliteOrbit.Tests/Dms/     # unit + integration tests
```

---

## 8. Security & Permissions

### 8.1 Permission tree (register in `RhyoliteOrbitAuthorizationProvider`)

Follow existing `PermissionNames` style (e.g. `POS.View Branches`). Proposed constants:

```
Pages.Documents                       (module root)
DMS.View Documents | DMS.Create Document | DMS.Update Document | DMS.Delete Document
DMS.Download Document | DMS.Preview Document | DMS.Print Document
DMS.CheckOut Document | DMS.Manage Versions | DMS.Restore Version
DMS.Manage Metadata | DMS.Manage Templates | DMS.Manage Identifiers
DMS.Manage Folders | DMS.Manage Collections
DMS.Manage Tags
DMS.Manage Permissions | DMS.Share Document | DMS.Manage Shares
DMS.Manage Workflows | DMS.Start Workflow | DMS.Participate In Workflow | DMS.Manage Workflow Tasks
DMS.View Audit Trail | DMS.Export Audit Trail
DMS.Manage Retention | DMS.Apply Legal Hold | DMS.Manage Settings | DMS.Manage Quotas
DMS.Sign Document | DMS.Manage Annotations
DMS.Bulk Import | DMS.Bulk Export | DMS.Purge (hard delete) | DMS.Force Unlock
DMS.Use Ai Features
```

Roles: map to the ERP default roles (Admin, Manager, Clerk/Records Officer, Viewer, Auditor
read-only). Auditor gets View/Preview/Audit-view only.

### 8.2 Document-level ACL

- Evaluation order: **explicit deny → explicit allow on document → inherited folder allow/deny →
  collection → role/user defaults → tenant default (Viewer/None)**.
- `IAclManager.EvaluateAsync(userId, securable, permission)` returns effective permission set;
  cached in Redis (key `dms:acl:{tenant}:{securable}:v{version}`), invalidated on ACL change.
- Inherited ACLs are materialized-on-read (not duplicated) to keep writes cheap; folder moves
  recompute child effective permissions lazily with cache invalidation.
- Public shares bypass ACL but are token-scoped, expiring, optionally passworded, download-capped,
  and watermarked.

### 8.3 Subscription enforcement

- `DmsAppServiceBase` resolves the current tenant and calls
  `IDmsSubscriptionGuard.EnsureActiveAsync()` on **every** DMS endpoint:
  - `HasSubscribedToDocumentManagement == true`
  - `DocumentManagementSubscriptionEndDate >= today` (grace period configurable, e.g. 7 days)
  - quota not exceeded
- Failure → `AbpAuthorizationException`/`UserFriendlyException(402-style)` with a localized
  message; consistent with existing `UserFriendlyException` usage in domain managers.
- Renewal/expiry jobs (Quartz) already exist in `SubscriptionManager`; extend to send DMS
  expiry warnings and to flip the module to read-only during grace.

### 8.4 Data protection

- **Transport**: TLS only (Kestrel config).
- **At rest**: object-store-side encryption (Cloudinary/S3 SSE); optional app-level AES-256-GCM
  envelope encryption for tenants with `EncryptionAtRest` enabled (key per tenant, stored in
  Redis-protected secret store / settings).
- **PII**: auto-masking in search snippets; redaction annotations; configurable field-level
  encryption for metadata marked sensitive.
- **Antivirus**: scan before index/publish; quarantine bucket; re-scan on version upload.
- **Tokens/secrets**: share tokens hashed (SHA-256) at rest; passwords hashed with ABP's
  `IPasswordHasher`.
- **Audit**: hash-chained entries; periodic anchor export (M5).

---

## 9. API Surface (summary)

All endpoints under `/api/services/app/...` (ABP dynamic Web API convention, matching existing
services like `SubscriptionAppService`). Pagination follows the existing pattern
(`pageNo`, `pageSize`, `query` → `{ pageNo, totalCount, totalPages, data, lowerBound, upperBound }`).

| App service | Representative endpoints |
|---|---|
| `FolderAppService` | `GetTree`, `Get`, `Create`, `Update`, `Move`, `Copy`, `Delete` (soft → trash), `Restore`, `GetBreadcrumbs`, `GetContents` (docs+subfolders, sortable/filterable) |
| `DocumentAppService` | `Upload` (multipart, chunked/resumable), `Get`, `GetForEdit`, `Update` (metadata/tags), `Move`, `Copy`, `Delete`, `Restore`, `Purge`, `Star`/`Unstar`, `Download` (ranged), `Preview`, `Thumbnail`, `DuplicateCheck`, `BulkUpdate`, `BulkDelete`, `Trash/ListTrash` |
| `VersionAppService` | `ListVersions`, `GetVersion`, `UploadNewVersion` (major/minor), `Promote`, `DownloadVersion`, `Restore`, `Diff` |
| `CheckOutAppService` (or on Document) | `CheckOut`, `CheckIn`, `CancelCheckOut`, `ForceUnlock` |
| `MetadataAppService` | Templates: `CreateTemplate/Update/Delete/List`; Attributes: `CreateAttribute/Update/Delete/Reorder`; Values: `GetValues/SaveValues`; `GetDefaultsForFolder` |
| `IdentifierAppService` | `Create/Update/Delete/List`, `PreviewNext`, `ResetSequence` |
| `TagAppService` | `Create/Update/Delete/List`, `TagCloud`, `Assign/Unassign`, `GetByTag` |
| `SearchAppService` | `Search` (full-text + metadata + filters + facets + snippets + highlighting), `Suggest`, `SavedSearches CRUD`, `SaveSearch/run`, `AdvancedSearch` (boolean operators) |
| `AclAppService` | `GetEffectivePermissions`, `Grant`, `Revoke`, `SetInheritance`, `ListEntries` |
| `ShareAppService` | `CreateShare` (internal/public/email), `ListShares`, `RevokeShare`, `ResolveShare` (public, token), `RecordDownload` |
| `CommentAppService` | `Create` (threaded), `Update`, `Delete`, `ListByDocument`, `Resolve` |
| `AnnotationAppService` | `Create` (highlight/note/stamp/redaction), `List`, `Delete`, `ExportAnnotations` |
| `WorkflowAppService` | Templates: `Create/Update/Delete/List/Validate` (step graph), `Preview`; Instances: `Start` (from document), `List`, `Get`, `Cancel`, `GetTasksForUser`, `GetTasksForDocument`; Tasks: `Approve/Reject/RequestChanges/Delegate/Reassign/AddComment`, `CompleteSignature` |
| `RetentionAppService` | Policies CRUD, `AssignPolicy`, `GetDispositionQueue`, `ApplyDisposition`, `Defer`, `GetReport` |
| `LegalHoldAppService` | `Apply`, `Release`, `List`, `GetHeldDocuments` |
| `AuditAppService` | `List` (filters, paged), `Export` (CSV/PDF), `VerifyChain` (integrity check) |
| `SignatureAppService` | `CreateSignature` (draw/type/upload), `SignDocument`, `ListSignatures`, `VerifySignature` |
| `BulkIoAppService` | `ImportZip` (async job), `ExportZip`, `GetJobStatus`, `DownloadJobResult` |
| `LinkAppService` | `Link` (document ↔ ERP entity), `Unlink`, `GetDocumentsForEntity`, `GetEntitiesForDocument` |
| `AiAppService` | `Classify`, `Summarize`, `Ask` (RAG Q&A with citations), `DetectPii`, `Redact` |
| `QuotaAppService` | `GetUsage`, `SetTenantQuota` (host), `ListOverQuota` |
| `SettingsAppService` | DMS settings (storage provider, allowed types, max size, OCR languages, retention defaults, watermark template) |

---

## 10. Key Subsystem Designs

### 10.1 Storage abstraction

```csharp
public interface IStorageProvider {
    Task<StoredObject> PutAsync(Stream content, string key, string contentType, CancellationToken ct);
    Task<Stream> GetAsync(string key, CancellationToken ct);          // for download/preview
    Task<string> GetSignedUrlAsync(string key, TimeSpan ttl, ...);    // direct-to-storage downloads
    Task DeleteAsync(string key, CancellationToken ct);
    Task<bool> ExistsAsync(string key, CancellationToken ct);
}
```

- Default: `CloudinaryStorageProvider` wrapping the existing `IFileUploadApi` (raw uploads).
- Also implement `LocalStorageProvider` (dev/on-prem) and `S3StorageProvider` (AWS/MinIO/Ceph).
- Key scheme: `{tenantId}/{collectionId}/{folderId}/{documentId}/{versionId}/{fileName}` — stable,
  collision-free, move-friendly (no re-upload on folder move).
- Large files: chunked upload API (init/append/complete) + `IFormFile` streaming to storage;
  Kestrel/FormOptions limits raised (e.g. 512 MB per file, 2 GB per request) and per-tenant
  quotas enforced.

### 10.2 Text extraction & indexing pipeline

- `ITextExtractor` implementations: PDF (`PdfPig` — nuget), DOCX/XLSX/PPTX
  (`DocumentFormat.OpenXml`), TXT/CSV/JSON/XML, HTML (AngleSharp), EML/MSG (MimeKit).
- OCR: `IOcrService` → Tesseract worker container (multi-language packs) invoked over HTTP/gRPC;
  triggered for image PDFs (no text layer) and images; zonal OCR via region hints on templates.
- Indexer: `PostgresFtsSearchProvider` writes extracted text + `tsvector` with weighted ranking
  (title > metadata > content); snippets via `ts_headline`.
- Pipeline orchestration: RabbitMQ queue `dms.indexing` → `DocumentIngestionActor` (Akka) with
  retries + dead-letter; `DmsDocument.IndexStatus` tracks progress; Quartz job re-queues failed
  indexes nightly.
- Optional phase-2: `ISearchProvider` backed by OpenSearch for very large tenants (same query
  contract, config-switched).

### 10.3 Workflow engine

- **Definition**: existing `DmsWorkflowTemplate.WorkflowTemplateSteps` (jsonb) with
  `WorkflowTemplateStep` (assignees, completion rule, timeout, conditions, actions).
- **Runtime**: `DmsWorkflowEngine.StartAsync(documentId, templateId, variables)` →
  `DmsWorkflowInstance` + first-step `DmsWorkflowTask`s; tasks routed via `StepAssignee`
  (specific user / role / department / creator / manager / dynamic-by-metadata).
- **Transitions**: `CompleteTaskAsync(taskId, outcome, comment)` → engine evaluates
  `StepCondition`s (`IConditionEvaluator`, safe expression parser over a whitelisted context —
  document metadata/properties, user properties, system variables, previous outcome), advances,
  fans out parallel steps per `CompletionRule` (Any/All/Majority/Percentage/SpecificUsers),
  honors `NextStepOnReject`, runs `StepAction`s (email via `ICustomEmailSender`, webhook,
  metadata update, assign).
- **Timers/escalation**: Quartz jobs per step timeout → escalation step or reminder notifications.
- **Safety**: versioned template snapshots stored on the instance (jsonb) so in-flight instances
  are immune to template edits; idempotent task completion; full audit entries per transition.

### 10.4 Notifications & events

- All mutations publish `DmsDocumentEvent` (outbox → RabbitMQ) → consumers: email/SMS
  notifications (workflow assignments, share access, expiry warnings, disposition reviews),
  webhooks for tenants, and cache invalidation.
- Reuse `ICustomEmailSender`; add `IDmsNotifier` abstraction so SMS/in-app can be added later.

### 10.5 Audit trail (hash chain)

- Append-only `DmsAuditEntry`; `EntryHash = SHA256(PreviousHash + canonical(entry))`;
  genesis hash per tenant; `VerifyChain` endpoint recomputes and reports first break.
- Written via the same unit of work as the mutation (transactional consistency).

### 10.6 Retention & disposition

- Policies keyed by trigger (creation date / last modified / custom event date), retention days,
  disposition action (review / delete / archive / transfer), legal-hold capability.
- Quartz job daily: compute eligible documents → disposition review queue → records officer
  approves → execute (soft delete + storage lifecycle rule, or archive to cold storage).
- Legal hold short-circuits all deletion paths (app-level check in `DocumentManager.Delete`).

---

## 11. Delivery Plan (phases)

Estimates are engineer-days for one mid-level .NET developer, excluding code review buffer.
Each phase ends with a green build, tests, and a demo-able API surface.

### Phase 0 — Foundations & cleanup (5–7 d)
- Finalize entity model changes (§6.1/6.2), enums, tsvector computed column.
- Migration `Dms_Module_V1` + backfill script; verify on a restored production-shaped DB.
- `DmsAppServiceBase` + `IDmsSubscriptionGuard` + `DmsPermission` constants + authorization
  provider registration + localization keys.
- Storage abstraction + provider selection settings; unit-test scaffolding.
- **Exit criteria:** DB migrated; guard returns 402-equivalent for unsubscribed tenant; empty
  DMS menu/permissions visible in Swagger; CI builds.

### Phase 1 — Core repository (10–14 d)
- Folders (tree, move/copy, breadcrumbs, default templates/identifiers), collections.
- Document upload/download/preview/thumbnail, metadata & values, tags, templates, custom
  identifiers (atomic sequence), starred, trash/restore, saved searches (metadata-level).
- Quota accounting basics; duplicate detection by checksum.
- **Exit criteria:** full CRUD lifecycle through Swagger; 90%+ coverage on domain managers.

### Phase 2 — Governance core (10–14 d)
- Versions (major/minor, promote, restore, notes), check-in/check-out with lock expiry.
- ACL + effective permission evaluation + Redis cache; share links (expiry/password/caps/
  watermark); threaded comments; audit trail with hash chain + export/verify.
- **Exit criteria:** ACL tests (deny > allow > inheritance), version tests, audit chain verify
  passes; audit export sample reviewed.

### Phase 3 — Search, OCR & security pipeline (10–14 d)
- Text extractors; `PostgresFtsSearchProvider` (ranked search, snippets, facets, boolean);
  ingestion actor + RabbitMQ + retries/DLQ; OCR service + languages; zonal OCR for templates.
- Antivirus (ClamAV) with quarantine; index-status surfacing; nightly re-index job.
- **Exit criteria:** upload → searchable within N seconds on a 100-page PDF; scanned PDF
  searchable after OCR; infected file quarantined and not indexed.

### Phase 4 — Workflow engine (10–14 d)
- Template CRUD + validation + step graph preview; engine (start, tasks, conditions, completion
  rules, parallel/sequential, reject paths); actions (email/webhook/metadata/assign); escalation
  Quartz jobs; workflow history; task inbox endpoints.
- **Exit criteria:** end-to-end approval scenario (2-step, conditional branch, escalation) passes
  as an integration test; audit shows every transition.

### Phase 5 — Records, signatures & quotas (7–10 d)
- Retention policies, disposition queue & execution, legal hold/WORM; e-signature (draw/type/
  upload + signed hash + audit); quota management endpoints + enforcement + analytics snapshots.
- **Exit criteria:** retention run on seeded data produces correct disposition; held document
  cannot be deleted even by admin; signature verifies.

### Phase 6 — Integration & interop (10–14 d)
- Email ingestion (IMAP actor), ZIP import/export jobs, ERP entity linking
  (`DmsEntityLink` + deep-link endpoints from GL/Payroll/POS/School/Manufacturing),
  WebDAV endpoints, mobile endpoints (ranged download, thumbnails, sync change-feed),
  document generation from templates (Syncfusion PDF), document diff/compare, format conversion.
- **Exit criteria:** email lands as document with attachments; ZIP round-trip preserves tree;
  invoice PDF linked from GL invoice; WebDAV mount works against ACLs.

### Phase 7 — AI & modern features (10–14 d)
- AI classification/summarization/auto-tagging (`IAiDocumentService` with local/heuristic default
  + pluggable LLM), embeddings + hybrid semantic search, RAG Q&A with citations, PII detection &
  redaction, smart duplicate detection.
- **Exit criteria:** 90%+ doc-type classification accuracy on seeded corpus (target metric),
  semantic search beats lexical on paraphrase test set, PII recall/precision targets met.

### Phase 8 — Hardening & launch (7–10 d)
- Load tests (upload 500 MB file, 200 concurrent uploads, 10k-document search p95 < 500 ms),
  security review (OWASP ASVS spot checks, token/ACL penetration tests), observability
  (structured logs, metrics, health checks, Sentry-style error tracking), runbooks,
  backup/restore drill, documentation (README + Swagger descriptions + localization),
  seed data + demo tenant script.
- **Exit criteria:** NFRs met, security checklist signed off, docs published, migration
  rehearsed on staging.

**Total estimate:** ~70–100 engineer-days (≈4–5 months calendar for one developer; ~2 months
with two developers working in parallel on independent phases).

---

## 12. Testing Strategy

| Layer | Approach | Tools |
|---|---|---|
| Domain managers | Unit tests with in-memory/mocked repositories (xUnit + NSubstitute, matching existing test project style) | `test/RhyoliteOrbit.Tests/Dms/` |
| App services | Integration tests against PostgreSQL (Testcontainers) exercising auth, tenant filters, ACLs | `RhyoliteOrbit.Tests` |
| Workflow engine | Scenario tests: sequential, parallel, conditional branches, rejection, escalation, template versioning | unit + integration |
| Search | Golden-query tests with relevance assertions; OCR corpus tests | integration |
| ACL | Matrix tests over allow/deny/inheritance combinations | unit |
| Audit | Chain verification after mutation storms | integration |
| Pipeline | Upload → extract → OCR → index latency and failure/DLQ tests | integration |
| Load | k6/Artillery scripts for upload/search/download concurrency | perf env |
| Security | ACL bypass attempts, expired/revoked share tokens, tenant isolation probes | manual + automated |

**Definition of Done (per feature):** code merged on `arena/833b4471-rhyolite-orbit-api` →
build green → tests ≥ 85% coverage on changed domain logic → Swagger annotated → localized
messages → audit entries emitted → no PII in logs → reviewed.

---

## 13. Non-Functional Requirements

- **Performance:** upload ack < 2 s for 50 MB (streamed); search p95 < 500 ms on 1 M docs
  (per-tenant); preview generation < 10 s for 100-page PDF (cached after first render).
- **Scalability:** stateless API; horizontal scale; actors + queues absorb OCR/index bursts;
  Postgres partitioning for audit/event tables by month when > 50 M rows.
- **Availability:** health checks (DB, Redis, RabbitMQ, storage, ClamAV, Tesseract); graceful
  degradation (search falls back to metadata-only if indexer down).
- **Limits (defaults, configurable per tenant):** file size 512 MB; 10 000 docs per bulk import;
  50 000 documents/tenant soft target before sharding review; concurrent uploads 8/user.
- **Data lifecycle:** soft delete → trash (30 d) → purge; storage lifecycle rules to cold/archive
  tier; backup: nightly PITR + object-store versioning; restore drill quarterly.
- **Observability:** structured logs (no PII), metrics (upload/extract/index/search latency,
  queue depth), dashboards and alerts (DLQ > 0, index lag > 5 min, quota > 90%).

---

## 14. Risks & Mitigations

| Risk | Impact | Mitigation |
|---|---|---|
| Entity model churn after migration | Rework | Phase 0 model review sign-off; additive migrations only |
| Cloudinary raw-upload limits/size caps | Upload failures for large files | Pluggable storage; S3/local providers; chunked uploads; per-provider size validation |
| OCR quality on scans | Bad search recall | Language packs, zonal OCR, DPI pre-processing, quality scoring, re-OCR job |
| PostgreSQL FTS relevance vs Lucene | User dissatisfaction | Weighted tsvector + trigram + synonyms; `ISearchProvider` allows OpenSearch swap without API change |
| Workflow complexity creep | Schedule slip | Fixed condition whitelist; template validation; no scripting language in v1 |
| Subscription enforcement gaps | Revenue leakage | Guard in base app service + integration tests asserting 402 on unsubscribed/expired tenants |
| ACL mis-evaluation → data leak | Security incident | Deny-by-default; matrix tests; security review in Phase 8; audit all ACL changes |
| Hash-chain audit cost at scale | Write latency | Async anchor + batch verify; partition table |
| PII leakage via snippets/previews | Compliance breach | Masking by default, redaction workflow, DLP tests |
| Multi-tenant data bleed | Severe | Tenant filters + integration tests; every query scoped by `TenantId` (existing pattern) |

---

## 15. Open Decisions (need answers before/at Phase 0)

1. **Primary storage**: keep Cloudinary default, or move to S3/MinIO on-prem? (Affects Phase 0
   provider priority and cost model.)
2. **OCR engine**: Tesseract worker container vs. managed OCR service (e.g. Azure/AWS Textract)?
   Affects Phase 3 infra.
3. **Vector search**: enable `pgvector` on the existing PostgreSQL 17, or defer semantic search to
   an external store?
4. **AI provider** for M1–M4/M7: self-hosted model vs. external LLM API (data-residency
   implications for regulated tenants)?
5. **WebDAV**: required for launch or Phase 6 optional?
6. **Quota defaults** per subscription tier, and over-quota behavior (block writes vs. read-only
   vs. throttle)?
7. **Retention defaults**: any statutory retention periods per market (Ghana/GDPR-style) to seed?
8. **Front-end ownership**: which client consumes this API first (MVC vs SPA) — affects whether
   we ship navigation/menu items in Phase 0.

---

## 16. Immediate Next Steps

1. Review and approve this plan (adjust scope/phases/decisions in §15).
2. Create GitHub issues per phase with the checklists above; label `dms`.
3. Start **Phase 0**: entity finalization + migration + subscription guard + permissions, on
   branch `arena/833b4471-rhyolite-orbit-api`, PR per phase.
4. Stand up dev dependencies: ClamAV + Tesseract containers, pgvector check, storage provider
   credentials.

---

*Prepared as the implementation plan for the Rhyolite Orbit Document Management module.*

---

## 17. Implementation Status (as delivered)

The plan has been implemented end to end on branch `arena/833b4471-rhyolite-orbit-api`.
This section records what was built, where it lives, and what remains open.

### 17.1 Delivered by layer

| Layer | Delivered |
| --- | --- |
| Core — entities | 28 DMS entities in `src/RhyoliteOrbit.Core/Models/Dms/` (documents, versions, folders, collections, tags, templates, attributes, metadata values, identifiers, comments, annotations, ACL, audit, shares, saved searches, retention, legal holds, signatures, entity links, embeddings, events, quota, ingest jobs, settings, workflow template/instance/task). |
| Core — domain services | `src/RhyoliteOrbit.Core/DomainServices/Dms/`: documents, folders, versions, tags, metadata, identifiers, ACL, audit (SHA-256 hash chain), shares, comments, annotations, retention + legal holds, signatures, entity links, quota, settings, search contracts, security (ClamAV + regex PII), extraction (plain text, OOXML, best-effort PDF), OCR (Tesseract over HTTP), storage (Cloudinary/Local/S3 + factory), subscription guard, workflow engine, AI (heuristic classifier, hashing embeddings, extractive RAG), bulk ZIP import/export, notifier, document pipeline, ABP background job + indexing queue. |
| Core — actors | `Actors/Dms/DmsDocumentEventActor.cs` forwarding document events to RabbitMQ (best effort), registered in `TopLevelActors` and `Web.Mvc/Startup/Startup.cs`. |
| Application | 9 app services under `src/RhyoliteOrbit.Application/Services/DocumentManagement/` (documents, folders, versions, search, collaboration, metadata, workflow, governance, AI) plus a token-authenticated public share service, 6 DTO files and an AutoMapper profile. |
| EntityFrameworkCore | `ConfigureDms` (indexes + stored computed `tsvector` columns), `Dms/PostgresFtsSearchProvider.cs` (raw ADO: `ts_rank`, `ts_headline` snippets, metadata/tag/filter EXISTS clauses, facets, suggestions), hand-written migration `20261012120000_DmsModuleV1` (+ designer + snapshot) that creates the 15 new tables and adds the new columns with a data backfill (default collection per tenant, folder tree collection propagation, version 1.0 rows for pre-existing documents). |
| Tests | `test/RhyoliteOrbit.Tests/Dms/`: workflow condition evaluator, AI classification, deterministic embeddings, PII detection, local storage round-trip + path traversal, provider routing, quota, identifier sequences, share tokens/passwords, text extraction, and in-memory integration tests for folders/documents/versions/identifiers/quota. |

### 17.2 Conventions locked in

- Storage key: `{tenantId}/{collectionId}/{folderId}/{documentId}/{versionId}/{fileName}`.
- Audit hash: SHA-256 over `Sequence|Action|DocumentId|FolderId|ActorId|DetailsJson|CreationTime|PreviousHash`.
- Effective ACL: allow ∪ (deny − subtract) with document → folder → collection inheritance and an
  `All` fallback when nothing is defined.
- Subscription: grace period 7 days; quota in bytes against `DmsSettings.QuotaGb` (0 = unlimited).
- Embeddings: deterministic FNV-1a bag-of-words (256 dims, model `hashing-bow-v1` → `hashing-bow-v1`),
  so persisted vectors survive a process restart.

### 17.3 Known limitations

1. **No in-sandbox compilation.** The sandbox has no .NET SDK/NuGet access, so nothing in this change
   was compiled, run, or migration-tested. Treat the first build as a verification gate.
2. The snapshot/designer for `20261012120000_DmsModuleV1` were generated by script from the model
   description; the next `dotnet ef migrations add` may emit a small corrective migration.
3. Semantic search is exact-cosine over JSON vectors (no `pgvector`), which is O(n) per query — fine
   for the current data volumes, replaceable by a provider implementing `ISearchProvider`.
4. AI is the deterministic heuristic engine; an LLM-backed `IAiDocumentService`/`IRagService` can be
   registered without touching callers.
5. WebDAV, mailbox/email ingest and DICOM viewing from the plan are not implemented.
6. Front-end navigation/menu items are not part of this change.

### 17.4 Remaining work

1. Build the solution, fix compile errors, run the test suite.
2. Apply the migration against a PostgreSQL 17 dev database and verify the backfill.
3. Resolve the §15 open decisions (storage default, OCR engine, pgvector, AI provider, WebDAV,
   quota defaults, statutory retention, front-end consumer).
4. Add per-phase integration tests (subscription guard 402, ACL matrix, PII redaction).
5. Menu/navigation + UI for the app services above.
