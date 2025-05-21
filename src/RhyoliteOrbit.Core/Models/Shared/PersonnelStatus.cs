using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Shared;

public class PersonnelStatus: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)] 
    public string Name { get; set; }
    public bool IsDefault { get; set; }
    public int TenantId { get; set; }
}