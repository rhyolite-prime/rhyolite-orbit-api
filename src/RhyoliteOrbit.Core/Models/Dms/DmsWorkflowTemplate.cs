using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>A reusable workflow definition stored as an ordered list of steps.</summary>
public class DmsWorkflowTemplate: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(150)]
    public string Description { get; set; }

    [Column(TypeName = "jsonb")]
    public List<WorkflowTemplateStep> WorkflowTemplateSteps { get; set; }

    public bool IsActive { get; set; }

    /// <summary>Incremented on every change so running instances can be audited.</summary>
    public int Version { get; set; }

    public int TenantId { get; set; }
}
