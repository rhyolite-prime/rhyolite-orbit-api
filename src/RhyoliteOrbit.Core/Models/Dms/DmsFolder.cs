using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>
/// A node of the document repository tree. Root folders have a null <see cref="ParentId"/>.
/// </summary>
public class DmsFolder: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(150)]
    public string Description { get; set; }

    [MaxLength(150)]
    public string Path { get; set; }

    /// <summary>Null for root folders of a collection.</summary>
    public Guid? ParentId { get; set; }

    public Guid CollectionId { get; set; }

    public long OwnerId { get; set; }

    /// <summary>Document template applied by default to documents created in this folder.</summary>
    public Guid? TemplateId { get; set; }

    /// <summary>Custom identifier applied by default to documents created in this folder.</summary>
    public Guid? IdentifierId { get; set; }

    /// <summary>When true, ACL entries of ancestor folders flow down to this folder.</summary>
    public bool InheritPermissions { get; set; }

    public bool IsSystem { get; set; }

    public int Depth { get; set; }

    public int TenantId { get; set; }
}
