using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.SchoolManager;

public class StudentStatement: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid StudentId { get; set; }
    [MaxLength(120)]
    public string StudentName { get; set; }
    [MaxLength(20)]
    public string StudentIdentifier { get; set; }
    [Column(TypeName = "jsonb")] public List<Statement> Statement { get; set; }
    public int TenantId { get; set; }
    
}