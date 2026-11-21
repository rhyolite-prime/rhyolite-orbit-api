using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>
/// An immutable revision of a <see cref="DmsDocument"/>. Every upload of new content
/// creates a new version; versions are never modified in place.
/// </summary>
public class DmsDocumentVersion: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid DocumentId { get; set; }

    /// <summary>Major version number (1, 2, 3 ...).</summary>
    public int VersionNumber { get; set; }

    /// <summary>Minor version number (0, 1, 2 ...).</summary>
    public int MinorNumber { get; set; }

    [MaxLength(50)]
    public string VersionLabel { get; set; }

    [MaxLength(500)]
    public string ChangeNote { get; set; }

    [MaxLength(255)]
    public string FileName { get; set; }

    [MaxLength(30)]
    public string Extension { get; set; }

    public long FileSize { get; set; }

    [MaxLength(255)]
    public string MimeType { get; set; }

    [MaxLength(30)]
    public string StorageProvider { get; set; }

    [MaxLength(500)]
    public string StorageKey { get; set; }

    [MaxLength(64)]
    public string Checksum { get; set; }

    public bool IsCurrent { get; set; }

    public int PageCount { get; set; }

    /// <summary>Text extracted from the binary (used by full text search).</summary>
    public string ExtractedText { get; set; }

    public bool OcrApplied { get; set; }

    [MaxLength(20)]
    public string Language { get; set; }

    [MaxLength(500)]
    public string PreviewPath { get; set; }

    [MaxLength(500)]
    public string ThumbnailPath { get; set; }

    [Column(TypeName = "tsvector")]
    public string SearchVector { get; set; }

    public int TenantId { get; set; }
}
