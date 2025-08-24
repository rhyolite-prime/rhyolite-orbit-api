using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.SchoolManager;

public class Curriculum:  Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string Code { get; set; }
    
    public int GradeLevel { get; set; }
    
    public DateTime EffectiveStartDate { get; set; }
    
    public DateTime? EffectiveEndDate { get; set; }
    
    public bool IsActive { get; set; }
    
    public Guid DepartmentId { get; set; }
    
    public string DepartmentName { get; set; }
    
    public string AcademicStandards { get; set; }
    
    public string LearningObjectives { get; set; }
    
    public string AssessmentMethods { get; set; }
    
    public string RequiredResources { get; set; }
    
    [Column(TypeName = "jsonb")] public List<Syllabus> Syllabi { get; set; }
    
    public int TenantId { get; set; }
    
}