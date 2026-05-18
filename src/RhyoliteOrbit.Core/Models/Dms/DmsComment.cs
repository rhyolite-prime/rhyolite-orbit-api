using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Dms;

public class DmsComment: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid DocumentId { get; set; }

    public Guid? ParentCommentId { get; set; }

    [MaxLength(4000)] // Using 4000 instead of MAX for performance
    public string CommentText { get; set; }

    // Optional: Additional useful fields
    public bool IsEdited { get; set; }

    [MaxLength(50)]
    public string CommentType { get; set; } // General, Question, Suggestion, Resolution

    public int TenantId { get; set; }
}