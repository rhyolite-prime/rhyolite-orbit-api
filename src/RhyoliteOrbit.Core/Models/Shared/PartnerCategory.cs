using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Shared;

public class PartnerCategory: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(200)]
    public string Name { get; set; }
    public BusinessPartnerType Type { get; set; }
    public int TenantId { get; set; }
}

public enum BusinessPartnerType
{
    Customer = 1,
    Supplier = 2,
    Both = 3
}