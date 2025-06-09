using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.SchoolManager;

public class SubjectRemark: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public decimal MinimumMarks { get; set; }
    public decimal MaximumMarks { get; set; }
    [MaxLength(250)]
    public string Remarks { get; set; }
    public int TenantId { get; set; }
}