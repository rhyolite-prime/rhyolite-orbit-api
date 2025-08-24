using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Shared;

public class PartnerCategory: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(200)]
    public string Name { get; set; }
    public int PartnerType { get; set; } //controlled BusinessPartnerType enum
    public int TenantId { get; set; }
}

 