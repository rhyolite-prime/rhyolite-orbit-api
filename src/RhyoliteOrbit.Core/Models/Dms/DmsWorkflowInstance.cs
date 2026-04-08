using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Dms;

public class DmsWorkflowInstance: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid TemplateId { get; set; }

    public Guid DocumentId { get; set; }

    public int CurrentStepIndex { get; set; }

    [DefaultValue("In-Progress")]
    [MaxLength(20)]
    public string Status { get; set; } //Completed, Cancelled

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }
    
    public int TenantId { get; set; }
}