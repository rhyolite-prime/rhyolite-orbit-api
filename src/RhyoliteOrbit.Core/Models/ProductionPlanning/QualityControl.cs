using System;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class QualityControl: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid ProductionOrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public string TestType { get; set; } 
    public int TestResult { get; set; }
    public DateTime TestDate { get; set; }
    public int TenantId { get; set; }
}