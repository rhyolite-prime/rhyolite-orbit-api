using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>
/// A logical document (a set of versions) stored in the document management repository.
/// Binary content lives in <see cref="DmsDocumentVersion"/> rows; this row carries the
/// metadata, classification and lifecycle state of the document.
/// </summary>
public class DmsDocument: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }

    public Guid FolderId { get; set; }

    [MaxLength(100)]
    public string FolderName { get; set; }

    public Guid CollectionId { get; set; }

    [MaxLength(255)]
    public string FileName { get; set; }

    [MaxLength(30)]
    public string Extension { get; set; }

    [MaxLength(500)]
    public string Title { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

    /// <summary>Human readable identifier generated from a <see cref="DmsCustomIdentifier"/>.</summary>
    [MaxLength(100)]
    public string CustomId { get; set; }

    public long FileSize { get; set; }

    public int Status { get; set; }

    public bool IsStarred { get; set; }

    [MaxLength(255)]
    public string MimeType { get; set; }

    [MaxLength(500)]
    public string FilePath { get; set; }

    [MaxLength(64)]
    public string Checksum { get; set; }

    public int PageCount { get; set; }

    [MaxLength(20)]
    public string Language { get; set; }

    /// <summary>Identifier of the storage provider that holds the binaries ("Cloudinary", "Local", "S3").</summary>
    [MaxLength(30)]
    public string StorageProvider { get; set; }

    /// <summary>Provider specific key/path of the current version binary.</summary>
    [MaxLength(500)]
    public string StorageKey { get; set; }

    public Guid? CurrentVersionId { get; set; }

    public long OwnerId { get; set; }

    // check-out / check-in (document locking)
    public bool IsCheckedOut { get; set; }

    public long? CheckedOutBy { get; set; }

    public DateTime? CheckedOutAt { get; set; }

    public DateTime? LockExpiry { get; set; }

    // retention &amp; records management
    public Guid? RetentionPolicyId { get; set; }

    public DateTime? RetentionEventDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public bool IsOnLegalHold { get; set; }

    public bool IsWorm { get; set; }

    // asynchronous pipeline state
    public int IndexStatus { get; set; }

    public DateTime? IndexedAt { get; set; }

    public int ScanStatus { get; set; }

    public int OcrStatus { get; set; }

    public int AiStatus { get; set; }

    [MaxLength(500)]
    public string PreviewPath { get; set; }

    [MaxLength(500)]
    public string ThumbnailPath { get; set; }

    /// <summary>PostgreSQL tsvector used by the full text search index.</summary>
    [Column(TypeName = "tsvector")]
    public string SearchVector { get; set; }

    public int TenantId { get; set; }
}
