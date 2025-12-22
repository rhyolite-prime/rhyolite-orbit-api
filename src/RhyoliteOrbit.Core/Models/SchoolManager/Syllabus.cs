using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class Syllabus
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public Guid CurriculumId { get; set; }

    public Guid SubjectId { get; set; }

    public string SubjectName { get; set; }
    
    public Guid TeacherId { get; set; }
    
    public string TeacherName { get; set; }

    public string CourseObjectives { get; set; }

    public string GradingPolicy { get; set; }

    public string RequiredMaterials { get; set; }

    public string ClassPolicies { get; set; }

    public string WeeklySchedule { get; set; }
    
    public string AcademicTerm { get; set; }
    
    public int AcademicYear { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    public int TenantId { get; set; }
    
}