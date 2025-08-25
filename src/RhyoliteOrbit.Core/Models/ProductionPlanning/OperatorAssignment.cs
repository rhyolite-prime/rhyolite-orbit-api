using System;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class OperatorAssignment: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid OperatorId { get; set; }
    public Guid WorkCenterId { get; set; }
    public string WorkCenterName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string AssignedTask { get; set; }
    public int TenantId { get; set; }
}