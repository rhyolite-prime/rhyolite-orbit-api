using System;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class InspectionResult
{
    public Guid Id { get; set; }
    public Guid InProcessInspectionId { get; set; }
    public string Parameter { get; set; }
    public string AcceptanceCriteria { get; set; }
    public string Method { get; set; }
    public string MeasuredValue { get; set; }
    public bool Passed { get; set; }
}