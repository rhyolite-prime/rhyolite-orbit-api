using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.SchoolManager;

public class Subject : Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(50)]
    public string Name { get; set; }
    public Guid SubjectCategoryId { get; set; }
    [MaxLength(100)]
    public string SubjectCategoryName { get; set; }
    public int TenantId { get; set; }
}