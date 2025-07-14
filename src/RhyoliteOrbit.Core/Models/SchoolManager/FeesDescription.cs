using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.SchoolManager;

public class FeesDescription: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Description { get; set; }
    public Guid BillTypeId { get; set; }
        
    public Guid ReceivableAccountId { get; set; }
    [MaxLength(150)]
    public string ReceivableAccountName { get; set; }
    [MaxLength(50)]
    public string BillTypeName { get; set; }
    public int TenantId { get; set; }
}