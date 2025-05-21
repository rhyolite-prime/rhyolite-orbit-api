using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Shared;

public class CostCenter: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid ParentId { get; set; }
    [MaxLength(100)]
    public string Name { get; set; }
    public int TenantId { get; set; }
}