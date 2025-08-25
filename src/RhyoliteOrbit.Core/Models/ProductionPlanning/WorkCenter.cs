using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class WorkCenter: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(150)]
    public string Name { get; set; }
    public int CapacityPerHour { get; set; }
    [Column(TypeName = "jsonb")] public List<OperatingHour> OperatingHours { get; set; }
    public int TenantId { get; set; }
}