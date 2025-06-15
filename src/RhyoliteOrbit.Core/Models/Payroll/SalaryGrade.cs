using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Payroll;

public class SalaryGrade: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }
    public int NumericalOrder { get; set; }
    [Column(TypeName = "jsonb")] public List<SalaryNotch> SalaryNotches { get; set; }
    public int TenantId { get; set; }
}