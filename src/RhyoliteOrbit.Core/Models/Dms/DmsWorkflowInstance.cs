using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>A running workflow attached to a document.</summary>
public class DmsWorkflowInstance: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid TemplateId { get; set; }

    [MaxLength(100)]
    public string TemplateName { get; set; }

    public Guid DocumentId { get; set; }

    public int CurrentStepIndex { get; set; }

    /// <summary>In-Progress, Completed, Cancelled, Rejected, Suspended</summary>
    [MaxLength(20)]
    public string Status { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public long StartedById { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? LastActionAt { get; set; }

    /// <summary>Snapshot of the template steps taken when the instance started.</summary>
    [Column(TypeName = "jsonb")]
    public string TemplateSnapshotJson { get; set; }

    /// <summary>Runtime variables available to step conditions.</summary>
    [Column(TypeName = "jsonb")]
    public string VariablesJson { get; set; }

    public int TenantId { get; set; }
}
