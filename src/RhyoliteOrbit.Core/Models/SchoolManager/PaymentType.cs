using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.SchoolManager;

public class PaymentType: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(50)]
    public string Name { get; set; }
    [MaxLength(50)]
    public string TargetAudience { get; set; }
    public int TenantId { get; set; }
}