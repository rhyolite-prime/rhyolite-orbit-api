using System;

namespace RhyoliteOrbit.Models.Wssd;

public class DataSourceConfig
{
    public Guid FlexConnectId { get; set; }
    
    public string ApiResponsePath { get; set; }
    
    public string ApiResponseKeyField { get; set; }
    
    public string ApiResponseValueTemplate { get; set; }
}