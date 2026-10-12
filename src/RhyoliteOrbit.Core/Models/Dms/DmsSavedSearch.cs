using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>A persisted search definition (bookmark) owned by a user or shared tenant wide.</summary>
public class DmsSavedSearch: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public long UserId { get; set; }

    [MaxLength(100)]
    public string Name { get; set; }

    [Column(TypeName = "jsonb")]
    public string CriteriaJson { get; set; }

    public bool IsGlobal { get; set; }

    public DateTime? LastRunAt { get; set; }

    public int TenantId { get; set; }
}
