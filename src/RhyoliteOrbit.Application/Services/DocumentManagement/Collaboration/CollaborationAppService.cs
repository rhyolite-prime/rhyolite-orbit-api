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
using RhyoliteOrbit.DomainServices.Dms.Acl;
using RhyoliteOrbit.DomainServices.Dms.Annotations;
using RhyoliteOrbit.DomainServices.Dms.Audit;
using RhyoliteOrbit.DomainServices.Dms.Comments;
using RhyoliteOrbit.DomainServices.Dms.Documents;
using RhyoliteOrbit.DomainServices.Dms.Shares;
using RhyoliteOrbit.Models.Dms;
using RhyoliteOrbit.Services.DocumentManagement.Dto;

namespace RhyoliteOrbit.Services.DocumentManagement.Collaboration;

/// <summary>Sharing, comments and annotations.</summary>
[AbpAuthorize]
public class CollaborationAppService: DmsAppServiceBase
{
    private readonly IShareManager _shares;
    private readonly ICommentManager _comments;
    private readonly IAnnotationManager _annotations;
    private readonly IMapper _mapper;

    public CollaborationAppService(
        IShareManager shares,
        ICommentManager comments,
        IAnnotationManager annotations,
        IAclManager aclManager,
        IDocumentManager documentManager,
        DomainServices.Dms.Folders.IFolderManager folderManager,
        IAuditTrail auditTrail,
        IRepository<Authorization.Users.UserRole, long> userRoleRepository,
        IRepository<Authorization.Roles.Role, int> roleRepository,
        IMapper mapper)
        : base(aclManager, documentManager, folderManager, auditTrail, userRoleRepository, roleRepository)
    {
        _shares = shares;
        _comments = comments;
        _annotations = annotations;
        _mapper = mapper;
    }

    // ------------------------------------------------------------------ shares

    /// <summary>Creates a share for a document or a folder.</summary>
    [AbpAuthorize(PermissionNames.DMS_ShareDocument)]
    [HttpPost]
    public async Task<ShareDto> CreateShare(CreateShareDto input)
    {
        var userId = CurrentUserId;

        if (input.DocumentId.HasValue)
        {
            await EnsureDocumentPermissionAsync(input.DocumentId.Value, RhyoliteOrbitConsts.DmsPermission.Share);
        }
        else if (input.FolderId.HasValue)
        {
            await EnsureFolderPermissionAsync(input.FolderId.Value, RhyoliteOrbitConsts.DmsPermission.Share);
        }
        else
        {
            throw new UserFriendlyException("DmsShareTarget", "A document or a folder must be provided.");
        }

        var share = new DmsShare
        {
            TenantId = CurrentTenantId,
            DocumentId = input.DocumentId,
            FolderId = input.FolderId,
            ShareType = input.ShareType,
            SharedWithEmail = input.SharedWithEmail,
            SharedWithUserId = input.SharedWithUserId,
            ExpiresAt = input.ExpiresAt,
            AllowPreview = input.AllowPreview,
            AllowDownload = input.AllowDownload,
            AllowUpload = input.AllowUpload,
            MaxDownloads = input.MaxDownloads,
            RequiresWatermark = input.RequiresWatermark,
            CreatedById = userId,
            CreatorUserId = userId
        };

        string rawToken = null;
        if (input.ShareType == (int)RhyoliteOrbitConsts.ShareType.PublicLink)
        {
            rawToken = ShareManager.GenerateToken();
            share.Token = ShareManager.HashToken(rawToken);
        }

        if (!string.IsNullOrWhiteSpace(input.Password))
        {
            share.PasswordHash = ShareManager.HashPassword(input.Password);
        }

        var created = await _shares.CreateAsync(share);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.ShareCreated, input.DocumentId, input.FolderId,
            System.Text.Json.JsonSerializer.Serialize(new { created.ShareType, created.SharedWithEmail }), userId);

        var dto = _mapper.Map<ShareDto>(created);

        // The stored token is hashed: hand the raw value back exactly once.
        dto.Token = rawToken;

        return dto;
    }

    /// <summary>Revokes a share.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageShares)]
    [HttpDelete]
    public async Task RevokeShare(Guid id)
    {
        await _shares.RevokeAsync(id, CurrentUserId);
        await AuditTrail.RecordAsync(RhyoliteOrbitConsts.AuditAction.ShareRevoked, null, null, null, CurrentUserId);
    }

    /// <summary>Lists the shares of the tenant.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageShares)]
    [HttpGet]
    public async Task<ListResultDto<ShareDto>> GetShares(Guid? documentId = null, Guid? folderId = null)
    {
        var shares = await _shares.ListAsync(documentId, folderId);
        return new ListResultDto<ShareDto>(_mapper.Map<List<ShareDto>>(shares));
    }

    /// <summary>Resolves a public share token.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpPost]
    public async Task<ShareDto> ResolveShare(ResolveShareDto input)
    {
        var share = await _shares.ResolveTokenAsync(input.Token, input.Password);
        return _mapper.Map<ShareDto>(share);
    }

    // ---------------------------------------------------------------- comments

    /// <summary>Adds a comment to a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpPost]
    public async Task<CommentDto> CreateComment(CreateCommentDto input)
    {
        await EnsureDocumentPermissionAsync(input.DocumentId, RhyoliteOrbitConsts.DmsPermission.View);

        var comment = new DmsComment
        {
            TenantId = CurrentTenantId,
            DocumentId = input.DocumentId,
            VersionId = input.VersionId,
            ParentCommentId = input.ParentCommentId,
            CommentText = input.CommentText,
            CommentType = input.CommentType,
            AnchorJson = input.AnchorJson,
            CreatorUserId = CurrentUserId
        };

        var created = await _comments.CreateAsync(comment);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.CommentAdded, input.DocumentId, null, null, CurrentUserId);

        return _mapper.Map<CommentDto>(created);
    }

    /// <summary>Edits a comment.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpPut]
    public async Task<CommentDto> UpdateComment(UpdateCommentDto input)
    {
        var updated = await _comments.UpdateAsync(input.Id, input.CommentText, CurrentUserId);
        return _mapper.Map<CommentDto>(updated);
    }

    /// <summary>Deletes a comment.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpDelete]
    public async Task DeleteComment(Guid id)
    {
        await _comments.DeleteAsync(id, CurrentUserId);
        await AuditTrail.RecordAsync(RhyoliteOrbitConsts.AuditAction.CommentDeleted, null, null, null, CurrentUserId);
    }

    /// <summary>Lists the comments of a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<ListResultDto<CommentDto>> GetComments(Guid documentId, Guid? versionId = null)
    {
        await EnsureDocumentPermissionAsync(documentId, RhyoliteOrbitConsts.DmsPermission.View);
        var comments = await _comments.ListAsync(documentId, versionId);
        return new ListResultDto<CommentDto>(_mapper.Map<List<CommentDto>>(comments));
    }

    /// <summary>Marks a comment as resolved.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpPost]
    public async Task ResolveComment(Guid id)
    {
        await _comments.ResolveAsync(id, CurrentUserId);
    }

    /// <summary>Counts the unresolved comments of a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<int> CountUnresolvedComments(Guid documentId)
    {
        return await _comments.CountUnresolvedAsync(documentId);
    }

    // ------------------------------------------------------------- annotations

    /// <summary>Adds an annotation (highlight, note, stamp or redaction).</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageAnnotations)]
    [HttpPost]
    public async Task<AnnotationDto> CreateAnnotation(CreateAnnotationDto input)
    {
        await EnsureDocumentPermissionAsync(input.DocumentId, RhyoliteOrbitConsts.DmsPermission.Edit);

        var annotation = new DmsAnnotation
        {
            TenantId = CurrentTenantId,
            DocumentId = input.DocumentId,
            VersionId = input.VersionId,
            PageNumber = input.PageNumber,
            Type = input.Type,
            GeometryJson = input.GeometryJson,
            Text = input.Text,
            Color = input.Color,
            CreatedById = CurrentUserId,
            IsRedaction = input.IsRedaction,
            CreatorUserId = CurrentUserId
        };

        var created = await _annotations.CreateAsync(annotation);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.AnnotationAdded, input.DocumentId, null, null, CurrentUserId);

        return _mapper.Map<AnnotationDto>(created);
    }

    /// <summary>Deletes an annotation.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageAnnotations)]
    [HttpDelete]
    public async Task DeleteAnnotation(Guid id)
    {
        await _annotations.DeleteAsync(id, CurrentUserId);
        await AuditTrail.RecordAsync(RhyoliteOrbitConsts.AuditAction.AnnotationDeleted, null, null, null, CurrentUserId);
    }

    /// <summary>Lists the annotations of a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<ListResultDto<AnnotationDto>> GetAnnotations(Guid documentId, Guid? versionId = null)
    {
        await EnsureDocumentPermissionAsync(documentId, RhyoliteOrbitConsts.DmsPermission.View);
        var annotations = await _annotations.ListAsync(documentId, versionId);
        return new ListResultDto<AnnotationDto>(_mapper.Map<List<AnnotationDto>>(annotations));
    }

    /// <summary>Lists the redactions of a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<ListResultDto<AnnotationDto>> GetRedactions(Guid documentId)
    {
        await EnsureDocumentPermissionAsync(documentId, RhyoliteOrbitConsts.DmsPermission.View);
        var annotations = await _annotations.ListRedactionsAsync(documentId);
        return new ListResultDto<AnnotationDto>(_mapper.Map<List<AnnotationDto>>(annotations));
    }
}
