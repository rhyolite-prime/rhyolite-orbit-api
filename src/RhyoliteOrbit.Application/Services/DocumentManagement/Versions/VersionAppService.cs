using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Dms;
using RhyoliteOrbit.DomainServices.Dms.Acl;
using RhyoliteOrbit.DomainServices.Dms.Audit;
using RhyoliteOrbit.DomainServices.Dms.Documents;
using RhyoliteOrbit.DomainServices.Dms.Versions;
using RhyoliteOrbit.Models.Dms;
using RhyoliteOrbit.Services.DocumentManagement.Dto;

namespace RhyoliteOrbit.Services.DocumentManagement.Versions;

[AbpAuthorize]
public class VersionAppService: DmsAppServiceBase
{
    private readonly IVersionManager _versions;
    private readonly IDmsIndexingQueue _indexingQueue;
    private readonly IMapper _mapper;

    public VersionAppService(
        IVersionManager versions,
        IDmsIndexingQueue indexingQueue,
        IAclManager aclManager,
        IDocumentManager documentManager,
        DomainServices.Dms.Folders.IFolderManager folderManager,
        IAuditTrail auditTrail,
        IRepository<Authorization.Users.UserRole, long> userRoleRepository,
        IRepository<Authorization.Roles.Role, int> roleRepository,
        IMapper mapper)
        : base(aclManager, documentManager, folderManager, auditTrail, userRoleRepository, roleRepository)
    {
        _versions = versions;
        _indexingQueue = indexingQueue;
        _mapper = mapper;
    }

    /// <summary>Lists all versions of a document, newest first.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<ListResultDto<DocumentVersionDto>> GetAll(Guid documentId)
    {
        await EnsureDocumentPermissionAsync(documentId, RhyoliteOrbitConsts.DmsPermission.View);

        var versions = await _versions.ListAsync(documentId);
        versions.Reverse();

        return new ListResultDto<DocumentVersionDto>(_mapper.Map<List<DocumentVersionDto>>(versions));
    }

    /// <summary>Gets a single version.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<DocumentVersionDto> Get(Guid id)
    {
        var version = await _versions.GetAsync(id);
        await EnsureDocumentPermissionAsync(version.DocumentId, RhyoliteOrbitConsts.DmsPermission.View);
        return _mapper.Map<DocumentVersionDto>(version);
    }

    /// <summary>Uploads a new version of a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageVersions)]
    [HttpPost]
    public async Task<DocumentVersionDto> Add(Guid documentId, [FromForm] UploadVersionDto input)
    {
        await EnsureDocumentPermissionAsync(documentId, RhyoliteOrbitConsts.DmsPermission.Edit);

        if (input.File == null || input.File.Length == 0)
        {
            throw new UserFriendlyException("DmsNoFile", "A file is required.");
        }

        await using var content = input.File.OpenReadStream();

        var version = await _versions.AddAsync(documentId, new VersionUploadInput
        {
            Content = content,
            FileName = input.File.FileName,
            ContentType = input.File.ContentType,
            ChangeNote = input.ChangeNote,
            Scheme = input.Scheme,
            CreatedById = CurrentUserId
        });

        await _indexingQueue.EnqueueAsync(documentId, version.Id);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.VersionUploaded, documentId, null,
            System.Text.Json.JsonSerializer.Serialize(new { version.VersionNumber, version.MinorNumber }),
            CurrentUserId);

        return _mapper.Map<DocumentVersionDto>(version);
    }

    /// <summary>Promotes an older version to be the current one.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageVersions)]
    [HttpPost]
    public async Task Promote(PromoteVersionDto input)
    {
        var version = await _versions.GetAsync(input.VersionId);
        await EnsureDocumentPermissionAsync(version.DocumentId, RhyoliteOrbitConsts.DmsPermission.Edit);

        await _versions.PromoteAsync(input.VersionId, CurrentUserId);
        await _indexingQueue.EnqueueAsync(version.DocumentId, input.VersionId);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.VersionPromoted, version.DocumentId, null, null, CurrentUserId);
    }

    /// <summary>Downloads a specific version.</summary>
    [AbpAuthorize(PermissionNames.DMS_DownloadDocument)]
    [HttpGet]
    public async Task<IActionResult> Download(Guid id)
    {
        var version = await _versions.GetAsync(id);
        await EnsureDocumentPermissionAsync(version.DocumentId, RhyoliteOrbitConsts.DmsPermission.Download);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.VersionDownloaded, version.DocumentId, null, null, CurrentUserId);

        var content = await _versions.OpenReadAsync(id);
        return File(content, version.MimeType ?? "application/octet-stream", version.FileName);
    }

    /// <summary>Deletes a version (the current version cannot be deleted).</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageVersions)]
    [HttpDelete]
    public async Task Delete(Guid id)
    {
        var version = await _versions.GetAsync(id);

        if (version.IsCurrent)
        {
            throw new UserFriendlyException("DmsCurrentVersion", "The current version cannot be deleted.");
        }

        await EnsureDocumentPermissionAsync(version.DocumentId, RhyoliteOrbitConsts.DmsPermission.Edit);
        await _versions.DeleteAsync(id, CurrentUserId);
    }

    /// <summary>Re-runs the indexing pipeline for a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageVersions)]
    [HttpPost]
    public async Task Reindex(Guid documentId)
    {
        await EnsureDocumentPermissionAsync(documentId, RhyoliteOrbitConsts.DmsPermission.Edit);

        var current = await _versions.GetCurrentAsync(documentId);
        await _indexingQueue.EnqueueAsync(documentId, current?.Id ?? Guid.Empty);
    }
}
