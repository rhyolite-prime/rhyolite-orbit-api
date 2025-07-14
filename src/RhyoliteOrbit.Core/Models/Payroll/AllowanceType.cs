using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Payroll;

public class AllowanceType: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }
    public int AllowanceDays { get; set; }
    public Guid ExpenseAccountId { get; set; }
    [MaxLength(150)]
    public string ExpenseAccountName { get; set; }
    public bool IsTaxable { get; set; }
    [Column(TypeName = "jsonb")] public List<AllowanceRate> AllowanceRates { get; set; }
    public int TenantId { get; set; }
}