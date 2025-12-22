using System;

namespace RhyoliteOrbit.Models.Shared;

public class OperatingHour
{
    public string WeekDay { get; set; }
    public DateTime OpeningTime { get; set; }
    public DateTime ClosingTime { get; set; }
}