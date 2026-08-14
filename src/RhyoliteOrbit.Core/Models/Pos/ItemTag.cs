using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Pos;

public class ItemTag: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(200)]
    public string Name { get; set; }
    [MaxLength(150)]
    public string Description { get; set; }
    public int NumberOfTaggedProducts { get; set; }
    public int TenantId { get; set; }
}