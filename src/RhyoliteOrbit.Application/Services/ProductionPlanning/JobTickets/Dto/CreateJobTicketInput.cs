using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.ProductionPlanning.JobTickets.Dto;

public class CreateJobTicketInput
{
    public Guid ProductionOrderId { get; set; }
    
    public string ProductionOrderIdentifier { get; set; }
    
    public int SequenceNumber { get; set; }
    
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid WorkCenterId { get; set; }
    public string WorkCenterName { get; set; }
    public decimal EstimatedDurationInHours { get; set; }
    public DateTime ActualStartTime { get; set; }
    public DateTime ActualEndTime { get; set; }
    public bool IsMandatory { get; set; }
    public Guid SupervisorId { get; set; }
     
}

public class UpdateJobTicketInput: EntityDto<Guid>
{
    public Guid ProductionOrderId { get; set; }
    public string ProductionOrderIdentifier { get; set; }
    public int SequenceNumber { get; set; }
    public string JobTicketIdentifier { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid WorkCenterId { get; set; }
    public decimal EstimatedDurationInHours { get; set; }
    public DateTime ActualStartTime { get; set; }
    public DateTime ActualEndTime { get; set; }
    public bool IsMandatory { get; set; }
    public Guid SupervisorId { get; set; }
}