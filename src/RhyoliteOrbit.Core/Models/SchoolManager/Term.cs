using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class Term
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int PrecedenceNo { get; set; }
    public int NoOfDays { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}