using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.SchoolManager;

public class PromotionHistory: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid PromotedFrom { get; set; }
    [MaxLength(50)]
    public string CurrentClassName { get; set; }
    public Guid PromotedTo { get; set; }
    [MaxLength(50)]
    public string DestinationClassName { get; set; }
    public Guid AcademicYearId { get; set; }
    [MaxLength(50)]
    public string AcademicYearName { get; set; }
    public DateTime DatePromoted { get; set; }
    public Guid StudentId { get; set; }
    [MaxLength(20)]
    public string StudentIdentifier { get; set; }
    [MaxLength(130)]
    public string StudentName { get; set; }
    public int TenantId { get; set; }
}