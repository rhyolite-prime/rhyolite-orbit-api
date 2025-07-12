using System;

namespace RhyoliteOrbit.DomainServices.SchoolManager.AcademicYears.Dto;

public class TermDto
{
    public Guid AcademicYearId { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Position { get; set; }
    public int NoOfDays { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}