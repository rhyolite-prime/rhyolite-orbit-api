using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>A threaded discussion entry attached to a document (optionally to a version).</summary>
public class DmsComment: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid DocumentId { get; set; }

    public Guid? VersionId { get; set; }

    public Guid? ParentCommentId { get; set; }

    [MaxLength(4000)] // Using 4000 instead of MAX for performance
    public string CommentText { get; set; }

    public bool IsEdited { get; set; }

    /// <summary>General, Question, Suggestion, Resolution</summary>
    [MaxLength(50)]
    public string CommentType { get; set; }

    /// <summary>JSON anchor (page/coordinates) when the comment is an annotation.</summary>
    [Column(TypeName = "jsonb")]
    public string AnchorJson { get; set; }

    public long? ResolvedById { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public int TenantId { get; set; }
}
