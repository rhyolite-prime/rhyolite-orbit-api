using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Dms;

public class DmsDocumentTemplate: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(150)]
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public int TenantId { get; set; }
}