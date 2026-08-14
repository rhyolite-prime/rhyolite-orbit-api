
using System;

namespace RhyoliteOrbit.Models.Wssd;

public class WssdServiceInteractionConfigurationParentNode
{
    public Guid Id { get; set; }
    
    public string Title { get; set; }
    
    public int SortOrder { get; set; }
    
    public DataSourceConfig DataSourceConfig { get; set; }
    
    public string ExecutionScope { get; set; }
    
    public string VisibilityRule { get; set; }
    
    public bool IsActive { get; set; }
}