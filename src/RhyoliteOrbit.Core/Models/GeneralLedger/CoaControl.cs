using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.GeneralLedger;

public class CoaControl: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(120)]
    public string SubGroupName { get; set; }
    public int MinAccount { get; set; }
    public int MaxAccount { get; set; }
    public Guid AccountHeadId { get; set; }
    public Guid ParentSubGroupId { get; set; }
    [MaxLength(200)]
    public string ParentSubGroupName { get; set; }
    public int TenantId { get; set; }
}