using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class ProcessFlow: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsDefault { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    [Column(TypeName = "jsonb")] public List<ProcessStep> Steps { get; set; }
    public int TenantId { get; set; }
}