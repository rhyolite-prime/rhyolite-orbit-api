using System;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class QcChecklistItem
{
    public Guid Id { get; set; }
    public string Parameter { get; set; } // e.g. "Paper Thickness"
    public string Method { get; set; } // e.g. "Caliper Gauge"
    public string AcceptanceCriteria { get; set; } // e.g. "±0.05mm"
}