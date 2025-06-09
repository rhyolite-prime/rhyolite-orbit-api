using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.SchoolManager;

public class AssignedClass: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid AcademicYearId { get; set; }
    [MaxLength(50)]
    public string AcademicYearName { get; set; }
    public Guid TermId { get; set; }
    [MaxLength(50)]
    public string TermName { get; set; }
    public Guid StaffId { get; set; }
    [MaxLength(130)]
    public string StaffName { get; set; }
    [Column(TypeName = "jsonb")] public List<AssignedClassDetail> Details { get; set; }
    public int TenantId { get; set; }
}