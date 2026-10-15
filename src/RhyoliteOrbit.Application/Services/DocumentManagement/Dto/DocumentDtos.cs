using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Microsoft.AspNetCore.Http;

namespace RhyoliteOrbit.Services.DocumentManagement.Dto;

/// <summary>Full representation of a document.</summary>
public class DocumentDto: EntityDto<Guid>
{
    public string Name { get; set; }

    public Guid FolderId { get; set; }

    public string FolderName { get; set; }

    public Guid CollectionId { get; set; }

    public string FileName { get; set; }

    public string Extension { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string CustomId { get; set; }

    public long FileSize { get; set; }

    public int Status { get; set; }

    public bool IsStarred { get; set; }

    public string MimeType { get; set; }

    public string Checksum { get; set; }

    public int PageCount { get; set; }

    public string Language { get; set; }

    public string StorageProvider { get; set; }

    public string StorageKey { get; set; }

    public Guid? CurrentVersionId { get; set; }

    public long OwnerId { get; set; }

    public bool IsCheckedOut { get; set; }

    public long? CheckedOutBy { get; set; }

    public DateTime? CheckedOutAt { get; set; }

    public DateTime? LockExpiry { get; set; }

    public Guid? RetentionPolicyId { get; set; }

    public DateTime? RetentionEventDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public bool IsOnLegalHold { get; set; }

    public bool IsWorm { get; set; }

    public int IndexStatus { get; set; }

    public DateTime? IndexedAt { get; set; }

    public int ScanStatus { get; set; }

    public int OcrStatus { get; set; }

    public int AiStatus { get; set; }

    public string PreviewPath { get; set; }

    public string ThumbnailPath { get; set; }

    public DateTime CreationTime { get; set; }

    public long? CreatorUserId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public long? LastModifierUserId { get; set; }

    public bool IsDeleted { get; set; }

    public List<string> Tags { get; set; } = new();

    public string CurrentVersionLabel { get; set; }
}

/// <summary>Multipart upload request. <c>File</c> is the binary content.</summary>
public class UploadDocumentDto
{
    public Guid FolderId { get; set; }

    public string Name { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string CustomId { get; set; }

    public bool IsDraft { get; set; }

    public string ChangeNote { get; set; }

    public List<Guid> TagIds { get; set; }

    public IFormFile File { get; set; }
}

public class UpdateDocumentDto: EntityDto<Guid>
{
    public string Name { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string CustomId { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.DocumentStatus"/>.</summary>
    public int? Status { get; set; }
}

public class DocumentListInput: PagedAndSortedResultRequestDto
{
    public Guid? FolderId { get; set; }

    public Guid? CollectionId { get; set; }

    public string Query { get; set; }

    public Guid? TagId { get; set; }

    public int? Status { get; set; }

    public bool IncludeDeleted { get; set; }

    public bool OnlyStarred { get; set; }

    /// <summary>0 = Name, 1 = Created, 2 = Modified, 3 = Size.</summary>
    public int SortOrder { get; set; }
}

public class DocumentMetadataInput: EntityDto<Guid>
{
    /// <summary>Attribute id -> raw value.</summary>
    public Dictionary<Guid, string> Values { get; set; } = new();
}

public class DocumentTagsInput: EntityDto<Guid>
{
    public List<Guid> TagIds { get; set; } = new();
}

public class CheckInDocumentDto: EntityDto<Guid>
{
    public string ChangeNote { get; set; }

    /// <summary>0 = Major, 1 = Minor.</summary>
    public int Scheme { get; set; }

    /// <summary>Optional replacement content; without it the lock is simply released.</summary>
    public IFormFile File { get; set; }
}

public class DocumentRetentionDto: EntityDto<Guid>
{
    public Guid? PolicyId { get; set; }

    public DateTime? EventDate { get; set; }
}

public class DocumentFlagDto: EntityDto<Guid>
{
    public bool Value { get; set; }
}

public class MoveDocumentDto
{
    public Guid DocumentId { get; set; }

    public Guid TargetFolderId { get; set; }
}

public class CopyDocumentDto
{
    public Guid DocumentId { get; set; }

    public Guid TargetFolderId { get; set; }
}

public class DownloadResultDto
{
    public string FileName { get; set; }

    public string ContentType { get; set; }

    public string Url { get; set; }

    public long FileSize { get; set; }
}
