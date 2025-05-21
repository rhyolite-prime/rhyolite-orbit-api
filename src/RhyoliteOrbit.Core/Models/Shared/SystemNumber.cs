using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Shared;

public class SystemNumber: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string ModuleId { get; set; } //payroll, school-manager, ledger, asset,
    [MaxLength(100)]
    public string ItemName { get; set; }
    [MaxLength(100)]
    public string Prefix { get; set; }
    [MaxLength(100)]
    public string Suffix { get; set; }
    public int TenantId { get; set; }
    public int LastNo { get; set; }
}