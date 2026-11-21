using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>An electronic signature applied to a document version.</summary>
public class DmsSignature: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid DocumentId { get; set; }

    public Guid VersionId { get; set; }

    public long SignerId { get; set; }

    /// <summary>0 = Draw, 1 = Type, 2 = Upload, 3 = Certificate (see <see cref="RhyoliteOrbitConsts.SignatureType"/>).</summary>
    public int SignatureType { get; set; }

    [MaxLength(500)]
    public string SignatureImagePath { get; set; }

    [MaxLength(100)]
    public string CertificateThumbprint { get; set; }

    /// <summary>SHA-256 of the signed binary; used to verify the signature afterwards.</summary>
    [MaxLength(64)]
    public string SignedHash { get; set; }

    public DateTime SignedAt { get; set; }

    [MaxLength(50)]
    public string IpAddress { get; set; }

    [MaxLength(200)]
    public string Reason { get; set; }

    public int TenantId { get; set; }
}
