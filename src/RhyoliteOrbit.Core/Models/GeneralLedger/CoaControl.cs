using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.GeneralLedger;

public class CoaControl: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(120)]
    public string Name { get; set; }
    public int MinAccount { get; set; }
    public int MaxAccount { get; set; }
    public Guid AccountHeadId { get; set; }
    
    public int AccountHeadType { get; set; }
    
    public Guid ParentControlId { get; set; }
    public int TenantId { get; set; }
}