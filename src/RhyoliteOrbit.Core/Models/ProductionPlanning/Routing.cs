using System;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class Routing: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public int OperationNumber { get; set; }
    public Guid WorkCenterId { get; set; }
    public string WorkCenterName { get; set; }
    public string Description { get; set; } = string.Empty;
    public TimeSpan StandardTime { get; set; }
    public Guid PreviousOperationId { get; set; }
    public int TenantId { get; set; }
}