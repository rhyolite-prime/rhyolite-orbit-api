using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.Services.ProductionPlanning.WorkCenters.Dto;

public class CreateWorkCenterInput
{
    public string Name { get; set; }
    public int CapacityPerHour { get; set; }
    public Guid SupervisorId { get; set; }
    public string SupervisorName { get; set; }
    public string SupervisorPhoneNo { get; set; }
    public string SupervisorEmail { get; set; }
    public List<OperatingHour> OperatingHours { get; set; }
}

public class UpdateWorkCenterInput: EntityDto<Guid>
{
    public string Name { get; set; }
    public int CapacityPerHour { get; set; }
    public Guid SupervisorId { get; set; }
    public string SupervisorName { get; set; }
    public string SupervisorPhoneNo { get; set; }
    public string SupervisorEmail { get; set; }
    public List<OperatingHour> OperatingHours { get; set; }
}