using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>
/// A page anchored annotation (highlight, note, stamp or redaction) on a document version.
/// </summary>
public class DmsAnnotation: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid DocumentId { get; set; }

    public Guid VersionId { get; set; }

    public int PageNumber { get; set; }

    /// <summary>0 = Highlight, 1 = Note, 2 = Stamp, 3 = Redaction (see <see cref="RhyoliteOrbitConsts.AnnotationType"/>).</summary>
    public int Type { get; set; }

    [Column(TypeName = "jsonb")]
    public string GeometryJson { get; set; }

    [MaxLength(1000)]
    public string Text { get; set; }

    [MaxLength(20)]
    public string Color { get; set; }

    public long CreatedById { get; set; }

    public bool IsRedaction { get; set; }

    public int TenantId { get; set; }
}
