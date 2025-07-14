using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Payroll;

public class DeductionType: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }
    public Guid LiabilityAccountId { get; set; }
    [MaxLength(150)]
    public string LiabilityAccountName { get; set; }
    [Column(TypeName = "jsonb")] public List<DeductionRate> Rates { get; set; }
    public int TenantId { get; set; }
}