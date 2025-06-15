using System;
using System.Collections.Generic;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.Students.Dto;

public class StudentPromotion
{
    public DateTime DatePromoted { get; set; }
    public Guid PromotedFrom { get; set; }
    public Guid PromotedTo { get; set; }
    public List<Student> Students { get; set; }
}

public class AlumniStudentPromotion
{
     
    public List<Guid> StudentIds { get; set; }

}