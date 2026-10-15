using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.DocumentManagement.Dto;

// ------------------------------------------------------------------ folders
public class FolderDto: EntityDto<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string Path { get; set; }

    public Guid? ParentId { get; set; }

    public Guid CollectionId { get; set; }

    public long OwnerId { get; set; }

    public Guid? TemplateId { get; set; }

    public Guid? IdentifierId { get; set; }

    public bool InheritPermissions { get; set; }

    public bool IsSystem { get; set; }

    public int Depth { get; set; }

    public DateTime CreationTime { get; set; }

    public bool IsDeleted { get; set; }

    public List<FolderDto> Children { get; set; } = new();
}

public class CreateFolderDto
{
    public string Name { get; set; }

    public string Description { get; set; }

    public Guid? ParentId { get; set; }

    public Guid? CollectionId { get; set; }

    public Guid? TemplateId { get; set; }

    public Guid? IdentifierId { get; set; }
}

public class UpdateFolderDto: EntityDto<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }

    public bool? InheritPermissions { get; set; }
}

public class MoveFolderDto
{
    public Guid FolderId { get; set; }

    public Guid? NewParentId { get; set; }
}

// ------------------------------------------------------------- collections
public class CollectionDto: EntityDto<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }

    public int OwnerId { get; set; }

    public bool IsDefault { get; set; }

    public long StorageQuotaBytes { get; set; }

    public DateTime CreationTime { get; set; }
}

public class CreateCollectionDto
{
    public string Name { get; set; }

    public string Description { get; set; }

    public long StorageQuotaBytes { get; set; }
}

public class UpdateCollectionDto: EntityDto<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }

    public long StorageQuotaBytes { get; set; }
}

// --------------------------------------------------------------------- tags
public class TagDto: EntityDto<Guid>
{
    public string Name { get; set; }

    public string ColorCode { get; set; }

    public string Description { get; set; }

    public bool IsSystem { get; set; }
}

public class CreateTagDto
{
    public string Name { get; set; }

    public string ColorCode { get; set; }

    public string Description { get; set; }
}

public class UpdateTagDto: EntityDto<Guid>
{
    public string Name { get; set; }

    public string ColorCode { get; set; }

    public string Description { get; set; }
}

// ----------------------------------------------------------------- versions
public class DocumentVersionDto: EntityDto<Guid>
{
    public Guid DocumentId { get; set; }

    public int VersionNumber { get; set; }

    public int MinorNumber { get; set; }

    public string VersionLabel { get; set; }

    public string ChangeNote { get; set; }

    public string FileName { get; set; }

    public string Extension { get; set; }

    public long FileSize { get; set; }

    public string MimeType { get; set; }

    public string Checksum { get; set; }

    public bool IsCurrent { get; set; }

    public int PageCount { get; set; }

    public bool OcrApplied { get; set; }

    public string Language { get; set; }

    public DateTime CreationTime { get; set; }

    public long? CreatorUserId { get; set; }

    public bool IsDeleted { get; set; }
}

public class UploadVersionDto
{
    public string ChangeNote { get; set; }

    /// <summary>0 = Major, 1 = Minor.</summary>
    public int Scheme { get; set; }

    public Microsoft.AspNetCore.Http.IFormFile File { get; set; }
}

public class PromoteVersionDto
{
    public Guid VersionId { get; set; }
}

// ------------------------------------------------------------------ shares
public class ShareDto: EntityDto<Guid>
{
    public Guid? DocumentId { get; set; }

    public Guid? FolderId { get; set; }

    public int ShareType { get; set; }

    public string Token { get; set; }

    public string SharedWithEmail { get; set; }

    public long? SharedWithUserId { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public bool AllowPreview { get; set; }

    public bool AllowDownload { get; set; }

    public bool AllowUpload { get; set; }

    public int? MaxDownloads { get; set; }

    public int DownloadCount { get; set; }

    public bool RequiresWatermark { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime? LastAccessedAt { get; set; }

    public DateTime CreationTime { get; set; }
}

public class CreateShareDto
{
    public Guid? DocumentId { get; set; }

    public Guid? FolderId { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.ShareType"/>.</summary>
    public int ShareType { get; set; }

    public string SharedWithEmail { get; set; }

    public long? SharedWithUserId { get; set; }

    public string Password { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public bool AllowPreview { get; set; } = true;

    public bool AllowDownload { get; set; } = true;

    public bool AllowUpload { get; set; }

    public int? MaxDownloads { get; set; }

    public bool RequiresWatermark { get; set; }
}

public class ResolveShareDto
{
    public string Token { get; set; }

    public string Password { get; set; }
}

// ---------------------------------------------------------------- comments
public class CommentDto: EntityDto<Guid>
{
    public Guid DocumentId { get; set; }

    public Guid? VersionId { get; set; }

    public Guid? ParentCommentId { get; set; }

    public string CommentText { get; set; }

    public bool IsEdited { get; set; }

    public string CommentType { get; set; }

    public string AnchorJson { get; set; }

    public long? ResolvedById { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public DateTime CreationTime { get; set; }

    public long? CreatorUserId { get; set; }
}

public class CreateCommentDto
{
    public Guid DocumentId { get; set; }

    public Guid? VersionId { get; set; }

    public Guid? ParentCommentId { get; set; }

    public string CommentText { get; set; }

    public string CommentType { get; set; }

    public string AnchorJson { get; set; }
}

public class UpdateCommentDto: EntityDto<Guid>
{
    public string CommentText { get; set; }
}

// ------------------------------------------------------------- annotations
public class AnnotationDto: EntityDto<Guid>
{
    public Guid DocumentId { get; set; }

    public Guid VersionId { get; set; }

    public int PageNumber { get; set; }

    public int Type { get; set; }

    public string GeometryJson { get; set; }

    public string Text { get; set; }

    public string Color { get; set; }

    public long CreatedById { get; set; }

    public bool IsRedaction { get; set; }

    public DateTime CreationTime { get; set; }
}

public class CreateAnnotationDto
{
    public Guid DocumentId { get; set; }

    public Guid VersionId { get; set; }

    public int PageNumber { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.AnnotationType"/>.</summary>
    public int Type { get; set; }

    public string GeometryJson { get; set; }

    public string Text { get; set; }

    public string Color { get; set; }

    public bool IsRedaction { get; set; }
}

// -------------------------------------------------------------- saved search
public class SavedSearchDto: EntityDto<Guid>
{
    public long UserId { get; set; }

    public string Name { get; set; }

    public string CriteriaJson { get; set; }

    public bool IsGlobal { get; set; }

    public DateTime? LastRunAt { get; set; }
}

public class CreateSavedSearchDto
{
    public string Name { get; set; }

    public string CriteriaJson { get; set; }

    public bool IsGlobal { get; set; }
}

public class UpdateSavedSearchDto: EntityDto<Guid>
{
    public string Name { get; set; }

    public string CriteriaJson { get; set; }

    public bool IsGlobal { get; set; }
}
