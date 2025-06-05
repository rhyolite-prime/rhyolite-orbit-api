using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.SchoolManager;

public class AcademicYear: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TenantId { get; set; }
    public int PrecedenceNo { get; set; }
    [Column(TypeName = "jsonb")] public List<Term> Terms { get; set; }
}