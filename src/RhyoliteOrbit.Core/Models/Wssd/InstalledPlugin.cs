using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Wssd;

public class InstalledPlugin: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid PluginId { get; set; }
    
    [MaxLength(255)]
    public string Name { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }
    
    [MaxLength(20)]
    public string Version { get; set; }
    
    [MaxLength(150)]
    public string Author { get; set; }

    public DateTime? LastUpdatedDate { get; set; }

    public bool IsActive { get; set; }
    
    [Column(TypeName = "jsonb")] public Dictionary<string,object> MetaData { get; set; }
    
    [Column(TypeName = "jsonb")] public Dictionary<string,object> Executable { get; set; }
    
    public int TenantId { get; set; }
}