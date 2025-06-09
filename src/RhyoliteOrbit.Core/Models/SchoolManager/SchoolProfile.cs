using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.SchoolManager;

public class SchoolProfile: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(50)]
    public string SchoolName { get; set; }
    public Guid CurrentAcademicYearId { get; set; }
    [MaxLength(50)]
    public string CurrentAcademicYearName { get; set; }
    public Guid CurrentTermId { get; set; }
    [MaxLength(50)]
    public string CurrentTermName { get; set; }
    [MaxLength(100)]
    public string SecondaryEmailAddress { get; set; }
    [MaxLength(100)]
    public string RegionOrState { get; set; }
    [MaxLength(100)]
    public string City { get; set; }
    [MaxLength(20)]
    public string PrimaryPhoneNo { get; set; }
    [MaxLength(100)]
    public string SchoolHead { get; set; }
    [MaxLength(20)]
    public string SecondaryPhoneNo { get; set; }
    [MaxLength(100)]
    public string AssistantSchoolHead { get; set; }
    [MaxLength(100)]
    public string Website { get; set; }
    [MaxLength(100)]
    public string PostalAddress { get; set; }
    [MaxLength(100)]
    public string District { get; set; }
    [MaxLength(100)]
    public string Address { get; set; }
    
    public int TenantId { get; set; }
}