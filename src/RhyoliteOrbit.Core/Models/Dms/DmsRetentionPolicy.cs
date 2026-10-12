using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>
/// Records management policy: how long documents are kept and what happens afterwards.
/// </summary>
public class DmsRetentionPolicy: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(250)]
    public string Description { get; set; }

    /// <summary>0 = Creation, 1 = LastModified, 2 = EventDate (see <see cref="RhyoliteOrbitConsts.RetentionTrigger"/>).</summary>
    public int Trigger { get; set; }

    public int RetentionDays { get; set; }

    /// <summary>0 = Review, 1 = Delete, 2 = Archive, 3 = Transfer (see <see cref="RhyoliteOrbitConsts.DispositionAction"/>).</summary>
    public int DispositionAction { get; set; }

    /// <summary>When true the policy can be suspended by a legal hold.</summary>
    public bool AllowLegalHold { get; set; }

    public bool IsActive { get; set; }

    /// <summary>Optional scope: only folders in this list are governed by the policy.</summary>
    [Column(TypeName = "jsonb")]
    public List<Guid> FolderIds { get; set; }

    public int TenantId { get; set; }
}
