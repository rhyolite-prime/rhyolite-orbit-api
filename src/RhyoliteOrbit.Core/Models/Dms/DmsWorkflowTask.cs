using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>A single work item assigned to a user by a workflow instance.</summary>
public class DmsWorkflowTask: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid WorkflowInstanceId { get; set; }

    public long AssignedTo { get; set; }

    public RhyoliteOrbitConsts.WorkflowTaskType TaskType { get; set; }

    public RhyoliteOrbitConsts.WorkflowTaskStatus Status { get; set; }

    [MaxLength(250)]
    public string Comments { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int StepIndex { get; set; }

    [MaxLength(100)]
    public string StepName { get; set; }

    public DateTime? DueDate { get; set; }

    public long? DelegatedFrom { get; set; }

    [Column(TypeName = "jsonb")]
    public string OutcomeJson { get; set; }

    public Guid? SignatureId { get; set; }

    public int TenantId { get; set; }
}
