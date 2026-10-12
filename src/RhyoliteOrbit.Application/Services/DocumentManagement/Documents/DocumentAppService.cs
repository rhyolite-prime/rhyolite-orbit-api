using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.UI;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Dms;
using RhyoliteOrbit.DomainServices.Dms.Documents;
using RhyoliteOrbit.DomainServices.Dms.Storage;
using RhyoliteOrbit.DomainServices.Dms.Subscription;
using RhyoliteOrbit.DomainServices.Dms.Versions;
using RhyoliteOrbit.Models.Dms;
using RhyoliteOrbit.Services.DocumentManagement.Dto;

namespace RhyoliteOrbit.Services.DocumentManagement.Documents;

[AbpAuthorize]
public class DocumentAppService: DmsAppServiceBase
{
    private readonly IDocumentManager _documents;
    private readonly IVersionManager _versions;
    private readonly IDmsIndexingQueue _indexingQueue;
    private readonly IStorageProviderFactory _storageFactory;
    private readonly IDmsSubscriptionGuard _subscriptionGuard;
    private readonly IMapper _mapper;

    public DocumentAppService(
        IDocumentManager documents,
        IVersionManager versions,
        IDmsIndexingQueue indexingQueue,
        IStorageProviderFactory storageFactory,
        IDmsSubscriptionGuard subscriptionGuard,
        DomainServices.Dms.Acl.IAclManager aclManager,
        DomainServices.Dms.Audit.IAuditTrail auditTrail,
        DomainServices.Dms.Folders.IFolderManager folderManager,
        Abp.Domain.Repositories.IRepository<Authorization.Users.UserRole, long> userRoleRepository,
        Abp.Domain.Repositories.IRepository<Authorization.Roles.Role, int> roleRepository,
        IMapper mapper)
        : base(aclManager, documents, folderManager, auditTrail, userRoleRepository, roleRepository)
    {
        _documents = documents;
        _versions = versions;
        _indexingQueue = indexingQueue;
        _storageFactory = storageFactory;
        _subscriptionGuard = subscriptionGuard;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>Uploads a document and its first version.</summary>
    [AbpAuthorize(PermissionNames.DMS_CreateDocument)]
    [HttpPost]
    public async Task<DocumentDto> Upload([FromForm] UploadDocumentDto input)
    {
        await _subscriptionGuard.EnsureActiveAsync();

        if (input.File == null || input.File.Length == 0)
        {
            throw new UserFriendlyException("DmsNoFile", "A file is required.");
        }

        await EnsureFolderPermissionAsync(input.FolderId, RhyoliteOrbitConsts.DmsPermission.Edit);

        await using var content = input.File.OpenReadStream();
        var upload = new DocumentUploadInput
        {
            FolderId = input.FolderId,
            Name = input.Name,
            FileName = input.File.FileName,
            Title = input.Title,
            Description = input.Description,
            CustomId = input.CustomId,
            Content = content,
            ContentType = input.File.ContentType,
            IsDraft = input.IsDraft,
            CreatedById = CurrentUserId,
            TagIds = input.TagIds ?? new List<Guid>()
        };

        var document = await _documents.UploadAsync(upload);

        await _indexingQueue.EnqueueAsync(document.Id, document.CurrentVersionId ?? Guid.Empty);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.DocumentCreated,
            document.Id,
            document.FolderId,
            System.Text.Json.JsonSerializer.Serialize(new { document.Name, document.FileSize, document.Checksum }),
            CurrentUserId);

        return await ToDtoAsync(document);
    }

    /// <summary>Updates the metadata of a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_UpdateDocument)]
    [HttpPut]
    public async Task<DocumentDto> Update(UpdateDocumentDto input)
    {
        var document = await _documents.GetAsync(input.Id);
        await EnsureDocumentPermissionAsync(document.Id, RhyoliteOrbitConsts.DmsPermission.Edit);

        document.Name = input.Name;
        document.Title = input.Title;
        document.Description = input.Description;
        document.CustomId = input.CustomId;

        if (input.Status.HasValue)
        {
            document.Status = input.Status.Value;
        }

        var updated = await _documents.UpdateAsync(document, CurrentUserId);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.DocumentUpdated, updated.Id, updated.FolderId, null, CurrentUserId);

        return await ToDtoAsync(updated);
    }

    /// <summary>Gets a single document.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<DocumentDto> Get(Guid id)
    {
        var document = await _documents.GetAsync(id);
        await EnsureDocumentPermissionAsync(document.Id, RhyoliteOrbitConsts.DmsPermission.View);
        return await ToDtoAsync(document);
    }

    /// <summary>Gets a document by its custom identifier.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<DocumentDto> GetByCustomId(string customId)
    {
        var document = await _documents.GetByCustomIdAsync(customId);
        await EnsureDocumentPermissionAsync(document.Id, RhyoliteOrbitConsts.DmsPermission.View);
        return await ToDtoAsync(document);
    }

    /// <summary>Lists documents with paging, filtering and sorting.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<PagedResultDto<DocumentDto>> GetAll(DocumentListInput input)
    {
        var request = new DocumentListRequest
        {
            FolderId = input.FolderId,
            CollectionId = input.CollectionId,
            Query = input.Query,
            TagId = input.TagId,
            Status = input.Status,
            IncludeDeleted = input.IncludeDeleted,
            OnlyStarred = input.OnlyStarred,
            SortOrder = input.SortOrder,
            PageNo = input.MaxResultCount == 0 ? input.SkipCount + 1 : (input.SkipCount / Math.Max(input.MaxResultCount, 1)) + 1,
            PageSize = input.MaxResultCount == 0 ? 20 : input.MaxResultCount
        };

        var result = await _documents.ListAsync(request);

        var data = new List<DocumentDto>();
        foreach (var document in result.Data)
        {
            data.Add(await ToDtoAsync(document));
        }

        return new PagedResultDto<DocumentDto>(result.TotalCount, data);
    }

    /// <summary>Lists the documents currently in the trash.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<PagedResultDto<DocumentDto>> GetTrash(DocumentListInput input)
    {
        input.IncludeDeleted = true;
        input.Status = (int)RhyoliteOrbitConsts.DocumentStatus.Deleted;
        return await GetAll(input);
    }

    /// <summary>Moves a document to another folder.</summary>
    [AbpAuthorize(PermissionNames.DMS_UpdateDocument)]
    [HttpPut]
    public async Task Move(MoveDocumentDto input)
    {
        var document = await _documents.GetAsync(input.DocumentId);
        await EnsureDocumentPermissionAsync(document.Id, RhyoliteOrbitConsts.DmsPermission.Edit);
        await EnsureFolderPermissionAsync(input.TargetFolderId, RhyoliteOrbitConsts.DmsPermission.Edit);

        await _documents.MoveAsync(input.DocumentId, input.TargetFolderId, CurrentUserId);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.DocumentMoved, input.DocumentId, input.TargetFolderId, null, CurrentUserId);
    }

    /// <summary>Copies a document to another folder.</summary>
    [AbpAuthorize(PermissionNames.DMS_CreateDocument)]
    [HttpPost]
    public async Task<DocumentDto> Copy(CopyDocumentDto input)
    {
        var document = await _documents.GetAsync(input.DocumentId);
        await EnsureDocumentPermissionAsync(document.Id, RhyoliteOrbitConsts.DmsPermission.View);
        await EnsureFolderPermissionAsync(input.TargetFolderId, RhyoliteOrbitConsts.DmsPermission.Edit);

        var copy = await _documents.CopyAsync(input.DocumentId, input.TargetFolderId, CurrentUserId);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.DocumentCopied, copy.Id, copy.FolderId, null, CurrentUserId);

        return await ToDtoAsync(copy);
    }

    /// <summary>Soft deletes a document (moves it to the trash).</summary>
    [AbpAuthorize(PermissionNames.DMS_DeleteDocument)]
    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await EnsureDocumentPermissionAsync(id, RhyoliteOrbitConsts.DmsPermission.Delete);
        await _documents.DeleteAsync(id, CurrentUserId);
        await AuditTrail.RecordAsync(RhyoliteOrbitConsts.AuditAction.DocumentDeleted, id, null, null, CurrentUserId);
    }

    /// <summary>Restores a document from the trash.</summary>
    [AbpAuthorize(PermissionNames.DMS_UpdateDocument)]
    [HttpPut]
    public async Task Restore(Guid id)
    {
        await EnsureDocumentPermissionAsync(id, RhyoliteOrbitConsts.DmsPermission.Delete);
        await _documents.RestoreAsync(id, CurrentUserId);
        await AuditTrail.RecordAsync(RhyoliteOrbitConsts.AuditAction.DocumentRestored, id, null, null, CurrentUserId);
    }

    /// <summary>Permanently deletes a document and all of its versions.</summary>
    [AbpAuthorize(PermissionNames.DMS_PurgeDocument)]
    [HttpDelete]
    public async Task Purge(Guid id)
    {
        await EnsureDocumentPermissionAsync(id, RhyoliteOrbitConsts.DmsPermission.Delete);
        await _documents.PurgeAsync(id, CurrentUserId);
        await AuditTrail.RecordAsync(RhyoliteOrbitConsts.AuditAction.DocumentPurged, id, null, null, CurrentUserId);
    }

    /// <summary>Stars or unstars a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpPut]
    public async Task SetStarred(DocumentFlagDto input)
    {
        await EnsureDocumentPermissionAsync(input.Id, RhyoliteOrbitConsts.DmsPermission.View);
        await _documents.SetStarredAsync(input.Id, input.Value, CurrentUserId);

        await AuditTrail.RecordAsync(
            input.Value ? RhyoliteOrbitConsts.AuditAction.DocumentStarred : RhyoliteOrbitConsts.AuditAction.DocumentUnstarred,
            input.Id, null, null, CurrentUserId);
    }

    /// <summary>Downloads the content of a document (or a specific version).</summary>
    [AbpAuthorize(PermissionNames.DMS_DownloadDocument)]
    [HttpGet]
    public async Task<IActionResult> Download(Guid id, Guid? versionId = null)
    {
        await EnsureDocumentPermissionAsync(id, RhyoliteOrbitConsts.DmsPermission.Download);

        var document = await _documents.GetAsync(id);
        var version = versionId.HasValue
            ? await _versions.GetAsync(versionId.Value)
            : await _versions.GetCurrentAsync(id);

        var fileName = version?.FileName ?? document.FileName;
        var contentType = version?.MimeType ?? document.MimeType;

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.VersionDownloaded, id, null,
            System.Text.Json.JsonSerializer.Serialize(new { versionId }), CurrentUserId);

        var content = await _documents.OpenReadAsync(id, versionId);
        return File(content, contentType ?? "application/octet-stream", fileName);
    }

    /// <summary>
    /// Returns a short lived signed URL for the current version. Used by the
    /// browser preview component so that large files never pass through the API.
    /// </summary>
    [AbpAuthorize(PermissionNames.DMS_PreviewDocument)]
    [HttpGet]
    public async Task<DownloadResultDto> GetPreviewUrl(Guid id, Guid? versionId = null)
    {
        await EnsureDocumentPermissionAsync(id, RhyoliteOrbitConsts.DmsPermission.Preview);

        var document = await _documents.GetAsync(id);
        var version = versionId.HasValue
            ? await _versions.GetAsync(versionId.Value)
            : await _versions.GetCurrentAsync(id);

        var key = version?.StorageKey ?? document.StorageKey;
        var fileName = version?.FileName ?? document.FileName;

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new UserFriendlyException("DmsNoContent", "This document has no stored content yet.");
        }

        var provider = await _storageFactory.GetProviderAsync(CurrentTenantId);
        var url = await provider.GetSignedUrlAsync(key, TimeSpan.FromMinutes(30), fileName);

        return new DownloadResultDto
        {
            FileName = fileName,
            ContentType = version?.MimeType ?? document.MimeType,
            Url = url,
            FileSize = version?.FileSize ?? document.FileSize
        };
    }

    /// <summary>Checks out a document (locks it for editing).</summary>
    [AbpAuthorize(PermissionNames.DMS_CheckOutDocument)]
    [HttpPost]
    public async Task CheckOut(Guid id)
    {
        await EnsureDocumentPermissionAsync(id, RhyoliteOrbitConsts.DmsPermission.CheckOut);
        await _documents.CheckOutAsync(id, CurrentUserId, TimeSpan.FromHours(8));

        await AuditTrail.RecordAsync(RhyoliteOrbitConsts.AuditAction.DocumentCheckedOut, id, null, null, CurrentUserId);
    }

    /// <summary>Checks a document back in, optionally uploading a new version.</summary>
    [AbpAuthorize(PermissionNames.DMS_CheckOutDocument)]
    [HttpPost]
    public async Task<DocumentVersionDto> CheckIn([FromForm] CheckInDocumentDto input)
    {
        await EnsureDocumentPermissionAsync(input.Id, RhyoliteOrbitConsts.DmsPermission.CheckOut);

        if (input.File != null && input.File.Length > 0)
        {
            await using var content = input.File.OpenReadStream();
            var version = await _versions.AddAsync(input.Id, new VersionUploadInput
            {
                Content = content,
                FileName = input.File.FileName,
                ContentType = input.File.ContentType,
                ChangeNote = input.ChangeNote,
                Scheme = input.Scheme,
                CreatedById = CurrentUserId
            });

            await _indexingQueue.EnqueueAsync(input.Id, version.Id);

            await AuditTrail.RecordAsync(
                RhyoliteOrbitConsts.AuditAction.VersionUploaded, input.Id, null,
                System.Text.Json.JsonSerializer.Serialize(new { version.VersionNumber, version.MinorNumber }),
                CurrentUserId);

            return _mapper.Map<DocumentVersionDto>(version);
        }

        await _documents.CheckInAsync(input.Id, CurrentUserId);

        await AuditTrail.RecordAsync(RhyoliteOrbitConsts.AuditAction.DocumentCheckedIn, input.Id, null, null, CurrentUserId);

        var current = await _versions.GetCurrentAsync(input.Id);
        return current == null ? null : _mapper.Map<DocumentVersionDto>(current);
    }

    /// <summary>Releases a lock held by another user.</summary>
    [AbpAuthorize(PermissionNames.DMS_ForceUnlockDocument)]
    [HttpPost]
    public async Task ForceUnlock(Guid id)
    {
        await EnsureDocumentPermissionAsync(id, RhyoliteOrbitConsts.DmsPermission.CheckOut);
        await _documents.ForceUnlockAsync(id, CurrentUserId);
        await AuditTrail.RecordAsync(RhyoliteOrbitConsts.AuditAction.LockForceReleased, id, null, null, CurrentUserId);
    }

    /// <summary>Saves the metadata values of a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_UpdateDocument)]
    [HttpPost]
    public async Task SaveMetadata(DocumentMetadataInput input)
    {
        await EnsureDocumentPermissionAsync(input.Id, RhyoliteOrbitConsts.DmsPermission.Edit);
        await _documents.SaveMetadataAsync(input.Id, input.Values ?? new Dictionary<Guid, string>(), CurrentUserId);
        await AuditTrail.RecordAsync(RhyoliteOrbitConsts.AuditAction.MetadataUpdated, input.Id, null, null, CurrentUserId);
    }

    /// <summary>Replaces the tags of a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_UpdateDocument)]
    [HttpPost]
    public async Task SaveTags(DocumentTagsInput input)
    {
        await EnsureDocumentPermissionAsync(input.Id, RhyoliteOrbitConsts.DmsPermission.Edit);
        await _documents.AssignTagsAsync(input.Id, input.TagIds ?? new List<Guid>(), CurrentUserId);
        await AuditTrail.RecordAsync(RhyoliteOrbitConsts.AuditAction.TagAssigned, input.Id, null, null, CurrentUserId);
    }

    /// <summary>Assigns a retention policy to a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageRetention)]
    [HttpPost]
    public async Task SetRetention(DocumentRetentionDto input)
    {
        await EnsureDocumentPermissionAsync(input.Id, RhyoliteOrbitConsts.DmsPermission.ManageRetention);
        await _documents.SetRetentionAsync(input.Id, input.PolicyId, input.EventDate, CurrentUserId);
        await AuditTrail.RecordAsync(RhyoliteOrbitConsts.AuditAction.RetentionApplied, input.Id, null, null, CurrentUserId);
    }

    /// <summary>Places or releases a legal hold on a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_ApplyLegalHold)]
    [HttpPost]
    public async Task SetLegalHold(DocumentFlagDto input)
    {
        await EnsureDocumentPermissionAsync(input.Id, RhyoliteOrbitConsts.DmsPermission.ManageRetention);
        await _documents.SetLegalHoldAsync(input.Id, input.Value, CurrentUserId);

        await AuditTrail.RecordAsync(
            input.Value ? RhyoliteOrbitConsts.AuditAction.LegalHoldApplied : RhyoliteOrbitConsts.AuditAction.LegalHoldReleased,
            input.Id, null, null, CurrentUserId);
    }

    /// <summary>Marks a document as write-once-read-many.</summary>
    [AbpAuthorize(PermissionNames.DMS_UpdateDocument)]
    [HttpPost]
    public async Task SetWorm(DocumentFlagDto input)
    {
        await EnsureDocumentPermissionAsync(input.Id, RhyoliteOrbitConsts.DmsPermission.Edit);
        await _documents.SetWormAsync(input.Id, input.Value, CurrentUserId);
    }

    /// <summary>Checks whether a document with the same checksum already exists.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<bool> IsDuplicate(string checksum)
    {
        return await _documents.IsDuplicateAsync(checksum);
    }

    private async Task<DocumentDto> ToDtoAsync(DmsDocument document)
    {
        var dto = _mapper.Map<DocumentDto>(document);

        var current = document.CurrentVersionId.HasValue
            ? await _versions.GetAsync(document.CurrentVersionId.Value)
            : null;

        dto.CurrentVersionLabel = current?.VersionLabel;
        dto.Tags = new List<string>();

        return dto;
    }
}
