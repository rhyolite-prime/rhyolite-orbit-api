using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Dms.Acl;
using RhyoliteOrbit.DomainServices.Dms.Audit;
using RhyoliteOrbit.DomainServices.Dms.BulkIo;
using RhyoliteOrbit.DomainServices.Dms.Documents;
using RhyoliteOrbit.DomainServices.Dms.Links;
using RhyoliteOrbit.DomainServices.Dms.Quota;
using RhyoliteOrbit.DomainServices.Dms.Retention;
using RhyoliteOrbit.DomainServices.Dms.Settings;
using RhyoliteOrbit.DomainServices.Dms.Signatures;
using RhyoliteOrbit.Models.Dms;
using RhyoliteOrbit.Services.DocumentManagement.Dto;

namespace RhyoliteOrbit.Services.DocumentManagement.Governance;

/// <summary>
/// Records management (retention and legal holds), signatures, audit trail,
/// quotas, settings and bulk import/export.
/// </summary>
[AbpAuthorize]
public class GovernanceAppService: DmsAppServiceBase
{
    private readonly IRetentionManager _retention;
    private readonly ILegalHoldManager _legalHolds;
    private readonly ISignatureService _signatures;
    private readonly IEntityLinkManager _links;
    private readonly IQuotaManager _quota;
    private readonly IDmsSettingsManager _settings;
    private readonly IBulkIoManager _bulkIo;
    private readonly IRepository<DmsSavedSearch, Guid> _savedSearchRepository;
    private readonly IRepository<DmsDocument, Guid> _documentRepository;
    private readonly IRepository<DmsFolder, Guid> _folderRepository;
    private readonly IRepository<DmsCollection, Guid> _collectionRepository;
    private readonly IRepository<DmsWorkflowTask, Guid> _workflowTaskRepository;
    private readonly IMapper _mapper;

    public GovernanceAppService(
        IRetentionManager retention,
        ILegalHoldManager legalHolds,
        ISignatureService signatures,
        IEntityLinkManager links,
        IQuotaManager quota,
        IDmsSettingsManager settings,
        IBulkIoManager bulkIo,
        IRepository<DmsSavedSearch, Guid> savedSearchRepository,
        IRepository<DmsDocument, Guid> documentRepository,
        IRepository<DmsFolder, Guid> folderRepository,
        IRepository<DmsCollection, Guid> collectionRepository,
        IRepository<DmsWorkflowTask, Guid> workflowTaskRepository,
        IAclManager aclManager,
        IDocumentManager documentManager,
        DomainServices.Dms.Folders.IFolderManager folderManager,
        IAuditTrail auditTrail,
        IRepository<Authorization.Users.UserRole, long> userRoleRepository,
        IRepository<Authorization.Roles.Role, int> roleRepository,
        IMapper mapper)
        : base(aclManager, documentManager, folderManager, auditTrail, userRoleRepository, roleRepository)
    {
        _retention = retention;
        _legalHolds = legalHolds;
        _signatures = signatures;
        _links = links;
        _quota = quota;
        _settings = settings;
        _bulkIo = bulkIo;
        _savedSearchRepository = savedSearchRepository;
        _documentRepository = documentRepository;
        _folderRepository = folderRepository;
        _collectionRepository = collectionRepository;
        _workflowTaskRepository = workflowTaskRepository;
        _mapper = mapper;
    }

    // ---------------------------------------------------------------- retention

    /// <summary>Lists the retention policies.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageRetention)]
    [HttpGet]
    public async Task<ListResultDto<RetentionPolicyDto>> GetPolicies(bool onlyActive = false)
    {
        var policies = await _retention.ListPoliciesAsync(onlyActive);
        return new ListResultDto<RetentionPolicyDto>(_mapper.Map<List<RetentionPolicyDto>>(policies));
    }

    /// <summary>Creates a retention policy.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageRetention)]
    [HttpPost]
    public async Task<RetentionPolicyDto> CreatePolicy(CreateRetentionPolicyDto input)
    {
        var policy = new DmsRetentionPolicy
        {
            TenantId = CurrentTenantId,
            Name = input.Name,
            Description = input.Description,
            Trigger = input.Trigger,
            RetentionDays = input.RetentionDays,
            DispositionAction = input.DispositionAction,
            AllowLegalHold = input.AllowLegalHold,
            IsActive = input.IsActive,
            FolderIds = input.FolderIds ?? new List<Guid>(),
            CreatorUserId = CurrentUserId
        };

        var created = await _retention.CreatePolicyAsync(policy);
        return _mapper.Map<RetentionPolicyDto>(created);
    }

    /// <summary>Updates a retention policy.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageRetention)]
    [HttpPut]
    public async Task<RetentionPolicyDto> UpdatePolicy(UpdateRetentionPolicyDto input)
    {
        var policy = new DmsRetentionPolicy
        {
            Id = input.Id,
            TenantId = CurrentTenantId,
            Name = input.Name,
            Description = input.Description,
            Trigger = input.Trigger,
            RetentionDays = input.RetentionDays,
            DispositionAction = input.DispositionAction,
            AllowLegalHold = input.AllowLegalHold,
            IsActive = input.IsActive,
            FolderIds = input.FolderIds ?? new List<Guid>(),
            LastModifierUserId = CurrentUserId
        };

        var updated = await _retention.UpdatePolicyAsync(policy);
        return _mapper.Map<RetentionPolicyDto>(updated);
    }

    /// <summary>Deletes a retention policy.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageRetention)]
    [HttpDelete]
    public async Task DeletePolicy(Guid id)
    {
        await _retention.DeletePolicyAsync(id);
    }

    /// <summary>Applies a retention policy to a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageRetention)]
    [HttpPost]
    public async Task ApplyPolicy(DocumentRetentionDto input)
    {
        await EnsureDocumentPermissionAsync(input.Id, RhyoliteOrbitConsts.DmsPermission.ManageRetention);
        await _retention.ApplyPolicyAsync(input.Id, input.PolicyId ?? Guid.Empty, input.EventDate, CurrentUserId);

        await AuditTrail.RecordAsync(RhyoliteOrbitConsts.AuditAction.RetentionApplied, input.Id, null, null, CurrentUserId);
    }

    /// <summary>Gets the documents that reached the end of their retention.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageRetention)]
    [HttpGet]
    public async Task<PagedResultDto<DocumentDto>> GetDispositionQueue(int pageNo = 1, int pageSize = 20)
    {
        var documents = await _retention.GetDispositionQueueAsync(pageNo, pageSize);

        var data = new List<DocumentDto>();
        foreach (var document in documents)
        {
            data.Add(_mapper.Map<DocumentDto>(document));
        }

        return new PagedResultDto<DocumentDto>(documents.Count, data);
    }

    /// <summary>Executes the disposition action for the expired documents.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageRetention)]
    [HttpPost]
    public async Task<int> ExecuteDisposition()
    {
        var processed = await _retention.ExecuteDispositionAsync(CurrentUserId);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.DispositionExecuted, null, null,
            System.Text.Json.JsonSerializer.Serialize(new { processed }), CurrentUserId);

        return processed;
    }

    /// <summary>Purges the documents whose trash retention expired.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageSettings)]
    [HttpPost]
    public async Task<int> PurgeExpiredTrash()
    {
        var settings = await _settings.GetAsync();
        return await _retention.PurgeExpiredTrashAsync(settings?.TrashRetentionDays ?? 30);
    }

    // -------------------------------------------------------------- legal holds

    /// <summary>Lists the legal holds.</summary>
    [AbpAuthorize(PermissionNames.DMS_ApplyLegalHold)]
    [HttpGet]
    public async Task<ListResultDto<LegalHoldDto>> GetLegalHolds(bool onlyActive = true)
    {
        var holds = await _legalHolds.ListAsync(onlyActive);
        return new ListResultDto<LegalHoldDto>(_mapper.Map<List<LegalHoldDto>>(holds));
    }

    /// <summary>Applies a legal hold to a set of documents and folders.</summary>
    [AbpAuthorize(PermissionNames.DMS_ApplyLegalHold)]
    [HttpPost]
    public async Task<LegalHoldDto> ApplyLegalHold(CreateLegalHoldDto input)
    {
        var hold = new DmsLegalHold
        {
            TenantId = CurrentTenantId,
            Name = input.Name,
            Reason = input.Reason,
            DocumentIds = input.DocumentIds ?? new List<Guid>(),
            FolderIds = input.FolderIds ?? new List<Guid>(),
            AppliedById = CurrentUserId,
            IsActive = true,
            CreatorUserId = CurrentUserId
        };

        var created = await _legalHolds.ApplyAsync(hold);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.LegalHoldApplied, null, null, created.Name, CurrentUserId);

        return _mapper.Map<LegalHoldDto>(created);
    }

    /// <summary>Releases a legal hold.</summary>
    [AbpAuthorize(PermissionNames.DMS_ApplyLegalHold)]
    [HttpPost]
    public async Task ReleaseLegalHold(Guid id)
    {
        await _legalHolds.ReleaseAsync(id, CurrentUserId);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.LegalHoldReleased, null, null, null, CurrentUserId);
    }

    /// <summary>Lists the documents currently under legal hold.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<ListResultDto<DocumentDto>> GetHeldDocuments()
    {
        var documents = await _legalHolds.GetHeldDocumentsAsync();
        return new ListResultDto<DocumentDto>(_mapper.Map<List<DocumentDto>>(documents));
    }

    // --------------------------------------------------------------- signatures

    /// <summary>Signs the current version of a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_SignDocument)]
    [HttpPost]
    public async Task<SignatureDto> Sign(SignDocumentDto input)
    {
        await EnsureDocumentPermissionAsync(input.DocumentId, RhyoliteOrbitConsts.DmsPermission.Edit);

        var signature = await _signatures.SignAsync(
            input.DocumentId,
            input.VersionId,
            CurrentUserId,
            input.SignatureType,
            input.SignatureImageBase64,
            input.Reason,
            null);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.SignatureApplied, input.DocumentId, null,
            System.Text.Json.JsonSerializer.Serialize(new { signature.SignedHash }), CurrentUserId);

        return _mapper.Map<SignatureDto>(signature);
    }

    /// <summary>Lists the signatures of a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<ListResultDto<SignatureDto>> GetSignatures(Guid documentId)
    {
        await EnsureDocumentPermissionAsync(documentId, RhyoliteOrbitConsts.DmsPermission.View);
        var signatures = await _signatures.ListAsync(documentId);
        return new ListResultDto<SignatureDto>(_mapper.Map<List<SignatureDto>>(signatures));
    }

    /// <summary>Verifies a signature against the checksum of its version.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<bool> VerifySignature(Guid id)
    {
        return await _signatures.VerifyAsync(id);
    }

    // ------------------------------------------------------------- entity links

    /// <summary>Links a document to an ERP record.</summary>
    [AbpAuthorize(PermissionNames.DMS_CreateDocument)]
    [HttpPost]
    public async Task<EntityLinkDto> CreateLink(CreateEntityLinkDto input)
    {
        await EnsureDocumentPermissionAsync(input.DocumentId, RhyoliteOrbitConsts.DmsPermission.View);

        var link = await _links.LinkAsync(
            (RhyoliteOrbitConsts.DmsEntityType)input.EntityType,
            input.EntityId,
            input.DocumentId,
            (RhyoliteOrbitConsts.DmsLinkType)input.LinkType,
            input.Note,
            CurrentUserId);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.EntityLinked, input.DocumentId, null, input.EntityId, CurrentUserId);

        return _mapper.Map<EntityLinkDto>(link);
    }

    /// <summary>Removes a link.</summary>
    [AbpAuthorize(PermissionNames.DMS_CreateDocument)]
    [HttpDelete]
    public async Task DeleteLink(Guid id)
    {
        await _links.UnlinkAsync(id, CurrentUserId);
        await AuditTrail.RecordAsync(RhyoliteOrbitConsts.AuditAction.EntityUnlinked, null, null, null, CurrentUserId);
    }

    /// <summary>Lists the documents linked to an ERP record.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<ListResultDto<EntityLinkDto>> GetLinksForEntity(int entityType, string entityId)
    {
        var links = await _links.GetForEntityAsync((RhyoliteOrbitConsts.DmsEntityType)entityType, entityId);
        return new ListResultDto<EntityLinkDto>(_mapper.Map<List<EntityLinkDto>>(links));
    }

    /// <summary>Lists the ERP records a document is linked to.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<ListResultDto<EntityLinkDto>> GetLinksForDocument(Guid documentId)
    {
        await EnsureDocumentPermissionAsync(documentId, RhyoliteOrbitConsts.DmsPermission.View);
        var links = await _links.GetForDocumentAsync(documentId);
        return new ListResultDto<EntityLinkDto>(_mapper.Map<List<EntityLinkDto>>(links));
    }

    // -------------------------------------------------------------------- audit

    /// <summary>Lists the audit trail.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewAuditTrail)]
    [HttpGet]
    public async Task<PagedResultDto<AuditEntryDto>> GetAuditTrail(
        Guid? documentId = null,
        Guid? folderId = null,
        int action = -1,
        int pageNo = 1,
        int pageSize = 20)
    {
        var result = await AuditTrail.ListAsync(documentId, folderId, action, pageNo, pageSize);

        return new PagedResultDto<AuditEntryDto>(result.TotalCount, _mapper.Map<List<AuditEntryDto>>(result.Data));
    }

    /// <summary>Verifies the integrity of the audit hash chain.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewAuditTrail)]
    [HttpGet]
    public async Task<AuditChainVerificationDto> VerifyAuditChain()
    {
        var result = await AuditTrail.VerifyChainAsync();

        return new AuditChainVerificationDto
        {
            IsValid = result.IsValid,
            VerifiedEntries = (int)result.VerifiedEntries,
            FirstBrokenSequence = result.FirstBrokenSequence.ToString()
        };
    }

    // ------------------------------------------------------------------ settings

    /// <summary>Gets the DMS settings of the tenant.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageSettings)]
    [HttpGet]
    public async Task<DmsSettingsDto> GetSettings()
    {
        var settings = await _settings.EnsureCreatedAsync();
        return _mapper.Map<DmsSettingsDto>(settings);
    }

    /// <summary>Updates the DMS settings of the tenant.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageSettings)]
    [HttpPut]
    public async Task<DmsSettingsDto> UpdateSettings(UpdateDmsSettingsDto input)
    {
        var settings = await _settings.EnsureCreatedAsync();

        settings.StorageProvider = input.StorageProvider;
        settings.LocalRootPath = input.LocalRootPath;
        settings.AllowedFileTypes = input.AllowedFileTypes ?? new List<string>();
        settings.MaxFileSizeMb = input.MaxFileSizeMb;
        settings.QuotaGb = input.QuotaGb;
        settings.OcrEnabled = input.OcrEnabled;
        settings.OcrLanguages = input.OcrLanguages ?? new List<string>();
        settings.AntivirusEnabled = input.AntivirusEnabled;
        settings.AiEnabled = input.AiEnabled;
        settings.WatermarkText = input.WatermarkText;
        settings.TrashRetentionDays = input.TrashRetentionDays;
        settings.EnforceVersioning = input.EnforceVersioning;
        settings.DefaultLanguage = input.DefaultLanguage;

        var updated = await _settings.UpdateAsync(settings);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.SettingsUpdated, null, null, null, CurrentUserId);

        return _mapper.Map<DmsSettingsDto>(updated);
    }

    // --------------------------------------------------------------------- quota

    /// <summary>Gets the storage usage of the tenant.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<object> GetQuotaUsage()
    {
        return await _quota.GetUsageAsync();
    }

    /// <summary>Recomputes the storage usage from the stored documents.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageQuotas)]
    [HttpPost]
    public async Task RecomputeQuota()
    {
        await _quota.RecomputeAsync();
    }

    // ----------------------------------------------------------------- bulk i/o

    /// <summary>Imports a ZIP archive into a folder.</summary>
    [AbpAuthorize(PermissionNames.DMS_BulkImport)]
    [HttpPost]
    public async Task<IngestJobDto> ImportZip([FromForm] ImportZipDto input)
    {
        await EnsureFolderPermissionAsync(input.FolderId, RhyoliteOrbitConsts.DmsPermission.Edit);

        if (input.File == null || input.File.Length == 0)
        {
            throw new UserFriendlyException("DmsNoFile", "A ZIP archive is required.");
        }

        await using var content = input.File.OpenReadStream();

        var job = await _bulkIo.ImportZipAsync(input.FolderId, content, input.File.FileName, CurrentUserId);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.BulkImportCompleted, null, input.FolderId,
            System.Text.Json.JsonSerializer.Serialize(new { job.TotalItems }), CurrentUserId);

        return _mapper.Map<IngestJobDto>(job);
    }

    /// <summary>Exports a folder (recursively) as a ZIP archive.</summary>
    [AbpAuthorize(PermissionNames.DMS_BulkExport)]
    [HttpPost]
    public async Task<IngestJobDto> ExportZip(Guid folderId)
    {
        await EnsureFolderPermissionAsync(folderId, RhyoliteOrbitConsts.DmsPermission.Download);

        var job = await _bulkIo.ExportZipAsync(folderId, CurrentUserId);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.BulkExportCompleted, null, folderId,
            System.Text.Json.JsonSerializer.Serialize(new { job.TotalItems }), CurrentUserId);

        return _mapper.Map<IngestJobDto>(job);
    }

    /// <summary>Downloads the result of an export job.</summary>
    [AbpAuthorize(PermissionNames.DMS_BulkExport)]
    [HttpGet]
    public async Task<IActionResult> DownloadExport(Guid jobId)
    {
        var job = await _bulkIo.GetJobAsync(jobId);

        if (job == null || string.IsNullOrWhiteSpace(job.ResultPath))
        {
            throw new UserFriendlyException("DmsExportNotReady", "The export is not available yet.");
        }

        var content = await System.IO.File.ReadAllBytesAsync(job.ResultPath);
        return File(new System.IO.MemoryStream(content), "application/zip", $"dms-export-{jobId:n}.zip");
    }

    /// <summary>Lists the bulk import/export jobs.</summary>
    [AbpAuthorize(PermissionNames.DMS_BulkImport)]
    [HttpGet]
    public async Task<PagedResultDto<IngestJobDto>> GetJobs(int pageNo = 1, int pageSize = 20)
    {
        var jobs = await _bulkIo.ListJobsAsync(pageNo, pageSize);
        return new PagedResultDto<IngestJobDto>(jobs.Count, _mapper.Map<List<IngestJobDto>>(jobs));
    }

    // ----------------------------------------------------------- saved searches

    /// <summary>Lists the saved searches of the current user.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<ListResultDto<SavedSearchDto>> GetSavedSearches()
    {
        var searches = await (await _savedSearchRepository.GetAllAsync())
            .Where(x => x.UserId == CurrentUserId || x.IsGlobal)
            .OrderByDescending(x => x.CreationTime)
            .ToListAsync();

        return new ListResultDto<SavedSearchDto>(_mapper.Map<List<SavedSearchDto>>(searches));
    }

    /// <summary>Creates a saved search.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpPost]
    public async Task<SavedSearchDto> CreateSavedSearch(CreateSavedSearchDto input)
    {
        var entity = new DmsSavedSearch
        {
            TenantId = CurrentTenantId,
            UserId = CurrentUserId,
            Name = input.Name,
            CriteriaJson = input.CriteriaJson,
            IsGlobal = input.IsGlobal,
            CreatorUserId = CurrentUserId
        };

        var created = await _savedSearchRepository.InsertAsync(entity);
        return _mapper.Map<SavedSearchDto>(created);
    }

    /// <summary>Updates a saved search.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpPut]
    public async Task<SavedSearchDto> UpdateSavedSearch(UpdateSavedSearchDto input)
    {
        var entity = await _savedSearchRepository.GetAsync(input.Id);

        entity.Name = input.Name;
        entity.CriteriaJson = input.CriteriaJson;
        entity.IsGlobal = input.IsGlobal;

        var updated = await _savedSearchRepository.UpdateAsync(entity);
        return _mapper.Map<SavedSearchDto>(updated);
    }

    /// <summary>Deletes a saved search.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpDelete]
    public async Task DeleteSavedSearch(Guid id)
    {
        await _savedSearchRepository.DeleteAsync(id);
    }

    // ------------------------------------------------------------- permissions

    /// <summary>Lists the ACL entries of a document, folder or collection.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManagePermissions)]
    [HttpGet]
    public async Task<ListResultDto<AclEntryDto>> GetPermissions(int securableType, Guid securableId)
    {
        var entries = await AclManager.GetEntriesAsync((RhyoliteOrbitConsts.SecurableType)securableType, securableId);
        return new ListResultDto<AclEntryDto>(_mapper.Map<List<AclEntryDto>>(entries));
    }

    /// <summary>Grants a permission on a document, folder or collection.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManagePermissions)]
    [HttpPost]
    public async Task<AclEntryDto> GrantPermission(GrantPermissionDto input)
    {
        var entry = await AclManager.GrantAsync(
            (RhyoliteOrbitConsts.SecurableType)input.SecurableType,
            input.SecurableId,
            (RhyoliteOrbitConsts.AclSubjectType)input.SubjectType,
            input.SubjectId,
            (RhyoliteOrbitConsts.DmsPermission)input.Permission,
            (RhyoliteOrbitConsts.AclEffect)input.Effect,
            CurrentUserId);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.PermissionGranted,
            input.SecurableType == (int)RhyoliteOrbitConsts.SecurableType.Document ? input.SecurableId : null,
            input.SecurableType == (int)RhyoliteOrbitConsts.SecurableType.Folder ? input.SecurableId : null,
            null,
            CurrentUserId);

        return _mapper.Map<AclEntryDto>(entry);
    }

    /// <summary>Revokes an ACL entry.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManagePermissions)]
    [HttpDelete]
    public async Task RevokePermission(Guid id)
    {
        await AclManager.RevokeAsync(id, CurrentUserId);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.PermissionRevoked, null, null, null, CurrentUserId);
    }

    /// <summary>Copies the ACL of a folder to another folder.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManagePermissions)]
    [HttpPost]
    public async Task CopyPermissions(Guid sourceFolderId, Guid targetFolderId)
    {
        await AclManager.CopyEntriesAsync(
            RhyoliteOrbitConsts.SecurableType.Folder, sourceFolderId, targetFolderId, CurrentUserId);
    }

    // ---------------------------------------------------------------- dashboard

    /// <summary>Summary counters for the DMS dashboard.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<DmsDashboardDto> GetDashboard()
    {
        var usage = await _quota.GetUsageAsync();
        var usageJson = System.Text.Json.JsonSerializer.Serialize(usage);
        using var document = System.Text.Json.JsonDocument.Parse(usageJson);

        long ReadInt64(string name) => document.RootElement.TryGetProperty(name, out var value)
            && value.ValueKind == System.Text.Json.JsonValueKind.Number
                ? value.GetInt64()
                : 0;

        var quotaBytes = await _quota.GetQuotaBytesAsync();
        var now = DateTime.UtcNow;

        var documents = await (await _documentRepository.GetAllAsync())
            .CountAsync(d => !d.IsDeleted);
        var folders = await (await _folderRepository.GetAllAsync())
            .CountAsync(f => !f.IsDeleted);
        var collections = await (await _collectionRepository.GetAllAsync())
            .CountAsync(c => !c.IsDeleted);
        var held = await (await _documentRepository.GetAllAsync())
            .CountAsync(d => d.IsOnLegalHold);
        var expiring = await (await _documentRepository.GetAllAsync())
            .CountAsync(d => !d.IsDeleted && d.ExpiryDate != null && d.ExpiryDate <= now.AddDays(30));
        var unindexed = await (await _documentRepository.GetAllAsync())
            .CountAsync(d => !d.IsDeleted && d.IndexStatus != (int)RhyoliteOrbitConsts.IndexStatus.Indexed);
        var tasks = await (await _workflowTaskRepository.GetAllAsync())
            .CountAsync(t => t.AssignedTo == CurrentUserId && t.Status == (int)RhyoliteOrbitConsts.WorkflowTaskStatus.Pending);

        return new DmsDashboardDto
        {
            TotalDocuments = documents,
            TotalFolders = folders,
            TotalCollections = collections,
            BytesUsed = ReadInt64("BytesUsed"),
            QuotaBytes = quotaBytes,
            PendingWorkflowTasks = tasks,
            DocumentsOnLegalHold = held,
            DocumentsExpiringSoon = expiring,
            UnindexedDocuments = unindexed
        };
    }
}
