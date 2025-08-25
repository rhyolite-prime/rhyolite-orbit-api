using System;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class ProcessStep
{
    public Guid Id { get; set; }
    public int ProcessFlowId { get; set; }
    public int SequenceNumber { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid WorkCenterId { get; set; }
    public string WorkCenterName { get; set; }
    public TimeSpan EstimatedDuration { get; set; }
    public bool IsMandatory { get; set; }
    public Guid NextStepId { get; set; }
}