using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>
/// A document profile: the set of custom attributes, allowed file types and the
/// workflow/retention defaults applied to documents created from the template.
/// </summary>
public class DmsDocumentTemplate: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(150)]
    public string Description { get; set; }

    public bool IsActive { get; set; }

    [Column(TypeName = "jsonb")]
    public List<string> AllowedFileTypes { get; set; }

    /// <summary>Workflow automatically started when a document is created from this template.</summary>
    public Guid? WorkflowTemplateId { get; set; }

    public Guid? RetentionPolicyId { get; set; }

    /// <summary>0 = Major, 1 = Minor, 2 = None (see <see cref="RhyoliteOrbitConsts.DocumentVersionScheme"/>).</summary>
    public int VersionScheme { get; set; }

    public bool RequiresApproval { get; set; }

    public int TenantId { get; set; }
}
