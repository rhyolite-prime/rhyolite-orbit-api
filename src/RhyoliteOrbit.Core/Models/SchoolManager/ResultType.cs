using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.SchoolManager;

public class ResultType: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(50)]
    public string Name { get; set; }
    public decimal Percentage { get; set; }
    public Guid LevelId { get; set; }
    [MaxLength(50)]
    public string LevelName { get; set; }
    public int TenantId { get; set; }
}