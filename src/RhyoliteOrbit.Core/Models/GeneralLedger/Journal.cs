using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.GeneralLedger;

public class Journal: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(25)]
    public string BatchNumber { get; set; }
    public DateTime BatchDate { get; set; }
    public decimal TotalCredit { get; set; }
    public decimal TotalDebit { get; set; }
    public bool IsPosted { get; set; }
    public bool IsMultiCurrency { get; set; }
    
    public Guid OuId { get; set; }
    [MaxLength(350)]
    
    public string OuName { get; set; }
    [MaxLength(20)]
    
    public string TransactionSource { get; set; }
    
    public int TenantId { get; set; }
}