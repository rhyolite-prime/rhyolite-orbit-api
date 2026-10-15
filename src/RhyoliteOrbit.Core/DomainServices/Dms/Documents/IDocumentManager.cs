using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Dms;

namespace RhyoliteOrbit.DomainServices.Dms.Documents;

/// <summary>Content of an upload together with its metadata.</summary>
public class DocumentUploadInput
{
    public Guid FolderId { get; set; }

    public string Name { get; set; }

    public string FileName { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string CustomId { get; set; }

    public Stream Content { get; set; }

    public string ContentType { get; set; }

    public bool IsDraft { get; set; }

    public long CreatedById { get; set; }

    public List<Guid> TagIds { get; set; }

    /// <summary>Attribute id -> raw value.</summary>
    public Dictionary<Guid, string> Metadata { get; set; }
}

public class DocumentListRequest
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

    public int PageNo { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}

public class PagedDocumentResult
{
    public int PageNo { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }

    public List<DmsDocument> Data { get; set; }
}

public interface IDocumentManager
{
    Task<DmsDocument> UploadAsync(DocumentUploadInput input);

    Task<DmsDocument> UpdateAsync(DmsDocument document, long updatedBy);

    Task MoveAsync(Guid documentId, Guid targetFolderId, long movedBy);

    Task<DmsDocument> CopyAsync(Guid documentId, Guid targetFolderId, long copiedBy);

    Task DeleteAsync(Guid documentId, long deletedBy);

    Task RestoreAsync(Guid documentId, long restoredBy);

    Task PurgeAsync(Guid documentId, long purgedBy);

    Task SetStarredAsync(Guid documentId, bool starred, long userId);

    Task<DmsDocument> GetAsync(Guid documentId);

    Task<DmsDocument> GetByCustomIdAsync(string customId);

    Task<PagedDocumentResult> ListAsync(DocumentListRequest request);

    Task CheckOutAsync(Guid documentId, long userId, TimeSpan? lockDuration = null);

    Task CheckInAsync(Guid documentId, long userId);

    Task ForceUnlockAsync(Guid documentId, long unlockedBy);

    Task<Stream> OpenReadAsync(Guid documentId, Guid? versionId = null);

    Task<bool> IsDuplicateAsync(string checksum);

    Task SetRetentionAsync(Guid documentId, Guid? policyId, DateTime? eventDate, long userId);

    Task SetLegalHoldAsync(Guid documentId, bool onHold, long userId);

    Task SetWormAsync(Guid documentId, bool worm, long userId);

    Task SaveMetadataAsync(Guid documentId, Dictionary<Guid, string> values, long updatedBy);

    Task AssignTagsAsync(Guid documentId, List<Guid> tagIds, long updatedBy);
}
