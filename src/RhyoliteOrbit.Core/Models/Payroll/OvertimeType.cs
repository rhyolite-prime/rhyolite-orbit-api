using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Payroll;

public class OvertimeType: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }
    
    public bool IsTaxable { get; set; }
    [Column(TypeName = "jsonb")] public List<OvertimeRate> Rates { get; set; }
    public int TenantId { get; set; }
}