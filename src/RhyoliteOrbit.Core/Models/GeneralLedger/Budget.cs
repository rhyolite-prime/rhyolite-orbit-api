using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.GeneralLedger;

public class Budget: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid AccountId { get; set; }
    [MaxLength(100)]
    public string AccountName { get; set; }
    public Guid OuId { get; set; }
    [MaxLength(350)]
    public string OuName { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime EndDate { get; set; }
    [Column(TypeName = "jsonb")] public List<BudgetDetail> BudgetDetails { get; set; }
    public int TenantId { get; set; }
}