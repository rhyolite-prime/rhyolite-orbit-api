using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class PtaAttendance
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; }
    public string StudentClass { get; set; }
    public string GuardianRepresentative { get; set; }
}