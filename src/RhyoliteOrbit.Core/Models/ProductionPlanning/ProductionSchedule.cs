using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class ProductionSchedule: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid ProductionOrderId { get; set; }
    [MaxLength(20)]
    public string ProductionOrderIdentifier { get; set; }
    [MaxLength(20)]
    public string ProductionScheduleIdentifier { get; set; }
    public Guid ProductId { get; set; }
    [MaxLength(100)]
    public string ProductName { get; set; }
    public Guid WorkCenterId { get; set; }
    [MaxLength(150)]
    public string WorkCenterName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Status { get; set; } // 
    public int ScheduledQuantity { get; set; }
    public int Priority { get; set; } //Priority enum
    [MaxLength(150)]
    public string Notes { get; set; }
    public int TenantId { get; set; }
}