using System;
using System.Collections.Generic;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.Students.Dto;

public class StudentPromotion
{
    public DateTime DatePromoted { get; set; }
    public Guid CurrentClassId { get; set; }
    public Guid DestinationClassId { get; set; }
    public List<Guid> StudentIds { get; set; }
}

public class AlumniStudentPromotion
{
    public List<Guid> StudentIds { get; set; }

}