using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>
/// A share grant. Internal shares reference a user; public shares are token based links
/// with optional password, expiry, download cap and watermarking.
/// </summary>
public class DmsShare: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid? DocumentId { get; set; }

    public Guid? FolderId { get; set; }

    /// <summary>0 = Internal, 1 = PublicLink, 2 = Email (see <see cref="RhyoliteOrbitConsts.ShareType"/>).</summary>
    public int ShareType { get; set; }

    /// <summary>SHA-256 hash of the public link token; empty for internal shares.</summary>
    [MaxLength(128)]
    public string Token { get; set; }

    [MaxLength(200)]
    public string PasswordHash { get; set; }

    [MaxLength(200)]
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

    public long CreatedById { get; set; }

    public int TenantId { get; set; }
}
