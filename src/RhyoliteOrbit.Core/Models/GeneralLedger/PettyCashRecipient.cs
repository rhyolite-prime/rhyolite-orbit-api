using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.GeneralLedger;

public class PettyCashRecipient: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(130)]
    public string RecipientName { get; set; }
    [MaxLength(20)]
    public string PhoneNo { get; set; }
    [MaxLength(100)]
    public string Email { get; set; }
    public int TenantId { get; set; }
}