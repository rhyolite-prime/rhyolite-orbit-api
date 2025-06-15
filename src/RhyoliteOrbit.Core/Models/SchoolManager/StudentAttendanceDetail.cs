using System;

namespace RhyoliteOrbit.Models.SchoolManager;

public class StudentAttendanceDetail
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentIdentifier { get; set; }
    public string StudentName { get; set; }
    public bool IsPresent { get; set; }
}