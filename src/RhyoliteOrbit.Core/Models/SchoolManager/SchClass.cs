using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.SchoolManager;

public class SchClass: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid LevelId { get; set; }
    [MaxLength(50)]
    public string LevelName { get; set; }
    [MaxLength(50)]
    public string ClassName { get; set; }
    public Guid StreamId { get; set; }
    [MaxLength(50)]
    public string StreamName { get; set; }
    public int TenantId { get; set; }
    
}