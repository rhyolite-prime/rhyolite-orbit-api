using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Dms;

public class DmsFolder: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(150)]
    public string Description { get; set; }
    [MaxLength(150)]
    public string Path { get; set; }
    public Guid ParentId { get; set; }
    public int TenantId { get; set; }
}