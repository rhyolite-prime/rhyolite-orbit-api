using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Dms;

public class DmsWorkflowTask: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid WorkflowInstanceId { get; set; }

    public Guid AssignedTo { get; set; }

    public RhyoliteOrbitConsts.WorkflowTaskType TaskType { get; set; }

    public RhyoliteOrbitConsts.WorkflowTaskStatus Status { get; set; }

    [MaxLength(250)]
    public string Comments { get; set; }

    public DateTime? CompletedAt { get; set; }
    public int TenantId { get; set; }
}