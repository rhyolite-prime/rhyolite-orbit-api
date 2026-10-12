using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>Per tenant configuration of the document management module.</summary>
public class DmsSettings: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(30)]
    public string StorageProvider { get; set; }

    [MaxLength(300)]
    public string LocalRootPath { get; set; }

    [Column(TypeName = "jsonb")]
    public List<string> AllowedFileTypes { get; set; }

    public int MaxFileSizeMb { get; set; }

    /// <summary>Tenant storage ceiling in GB; 0 = unlimited.</summary>
    public int QuotaGb { get; set; }

    public bool OcrEnabled { get; set; }

    [Column(TypeName = "jsonb")]
    public List<string> OcrLanguages { get; set; }

    public bool AntivirusEnabled { get; set; }

    public bool AiEnabled { get; set; }

    [MaxLength(200)]
    public string WatermarkText { get; set; }

    public int TrashRetentionDays { get; set; }

    public bool EnforceVersioning { get; set; }

    /// <summary>Base64/UTF-8 text stamped on previews and downloads.</summary>
    [MaxLength(30)]
    public string DefaultLanguage { get; set; }

    public int TenantId { get; set; }
}
