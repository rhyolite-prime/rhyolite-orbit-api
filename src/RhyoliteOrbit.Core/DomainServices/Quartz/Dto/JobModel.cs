using System;

namespace RhyoliteOrbit.DomainServices.Quartz.Dto;

public class JobModel
{
    public string Name { get; set; }
    
    public string Description { get; set; }
     
    public string Schedule { get; set; }
     
    public DateTime StartDate { get; set; }
     
    public string StartTime { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public CustomData CustomData { get; set; }
}

public class CustomData
{
    public string CallbackUrl { get; set; }
    public string UniqueId { get; set; }
}