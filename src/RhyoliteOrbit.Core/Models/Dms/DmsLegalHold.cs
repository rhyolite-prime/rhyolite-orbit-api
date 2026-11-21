using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>
/// A legal hold suspends disposition and deletion for the referenced documents/folders.
/// </summary>
public class DmsLegalHold: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(250)]
    public string Reason { get; set; }

    [Column(TypeName = "jsonb")]
    public List<Guid> DocumentIds { get; set; }

    [Column(TypeName = "jsonb")]
    public List<Guid> FolderIds { get; set; }

    public long AppliedById { get; set; }

    public DateTime? ReleasedAt { get; set; }

    public bool IsActive { get; set; }

    public int TenantId { get; set; }
}
