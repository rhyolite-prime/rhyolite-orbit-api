using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Wssd;

public class PluginActivityLog: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    
    public Guid PluginId { get; set; }
    
    [MaxLength(50)]
    public string ActivityType { get; set; } 
    // e.g., "INSTALL", "UNINSTALL", "UPDATE", "ENABLE", "DISABLE", "ACTIVATE", 
    // "DEACTIVATE", "CONFIG_CHANGE", "ERROR", "WARNING", "INFO", "DEBUG", 
    // "PERMISSION_CHANGE", "DEPENDENCY_CHECK", "INTEGRITY_CHECK"

    [MaxLength(20)]
    public string Level { get; set; } // "Info", "Warning", "Error", "Debug", "Critical"
    
    [MaxLength(500)]
    public string Message { get; set; }

    [MaxLength(2000)]
    public string Details { get; set; } // JSON formatted detailed information

    [MaxLength(100)]
    public string Source { get; set; } // Component that generated the log

    [MaxLength(650)]
    public string StackTrace { get; set; } // For error logs

    public int ExecutionTimeMs { get; set; } // Performance tracking
    
    public int TenantId { get; set; }
}