using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Payroll;

public class IrsSignature: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(150)]
    public string Name { get; set; }
    [MaxLength(150)]
    public string Title { get; set; }
    public int TenantId { get; set; }
}