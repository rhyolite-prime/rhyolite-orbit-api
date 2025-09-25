using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class QcChecklist:Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(200)]
    public string Name { get; set; }
    [MaxLength(500)]
    public string Description { get; set; }

    public Guid ProductId { get; set; }
    [MaxLength(100)]
    public string ProductName { get; set; }

    [Column(TypeName = "jsonb")] public List<QcChecklistItem> Items { get; set; }
    public int TenantId { get; set; }
}