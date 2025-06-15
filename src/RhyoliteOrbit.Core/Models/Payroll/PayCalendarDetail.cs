using System;

namespace RhyoliteOrbit.Models.Payroll;

public class PayCalendarDetail
{
    public Guid Id { get; set; }
    public Guid PayCalendarId { get; set; }
    public int Year { get; set; }
    public int Days { get; set; }
    public int Month { get; set; }
}