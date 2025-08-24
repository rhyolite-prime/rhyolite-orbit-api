using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class OperatingHour
{
    public string WeekDay { get; set; }
    public DateTime OpeningTime { get; set; }
    public DateTime ClosingTime { get; set; }
}