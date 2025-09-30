using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class Operation: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid ProductionOrderId { get; set; }
    
    [MaxLength(20)]
    public string ProductionOrderIdentifier { get; set; }
    
    public int SequenceNumber { get; set; }
    
    [MaxLength(20)]
    public string JobTicketIdentifier { get; set; }
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(200)]
    public string Description { get; set; }
    public Guid WorkCenterId { get; set; }
    [MaxLength(100)]
    public string WorkCenterName { get; set; }
    public decimal EstimatedDurationInHours { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsMandatory { get; set; }
    public int PriorityLevel { get; set; } // Priority enum
    public int Status { get; set; } // JobTicketStatus enum
    [MaxLength(50)]
    public string Tag { get; set; } // Priority enum
    // Manager properties : from employees.
    // Not everyone in the company will be a user on the portal/app.
    public Guid SupervisorId { get; set; }
    [MaxLength(130)]
    public string SupervisorName { get; set; }
    [MaxLength(130)]
    public string SupervisorEmail { get; set; }
    [MaxLength(150)]
    public string SupervisorDepartment { get; set; }
    [MaxLength(20)]
    public string SupervisorPhone { get; set; }
    
    //operator
    public Guid AssignedOperatorId { get; set; }
    [MaxLength(130)]
    public string AssignedOperatorName { get; set; }
    public int TenantId { get; set; }
}