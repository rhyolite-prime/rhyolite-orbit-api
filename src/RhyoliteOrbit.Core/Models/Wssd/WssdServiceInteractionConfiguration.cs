using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Wssd;

public class WssdServiceInteractionConfiguration : Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid WssdServiceId { get; set; }
    
    [MaxLength(100)]
    public string WssdServiceName { get; set; }
    
    [Column(TypeName = "jsonb")] public List<WssdServiceInteractionConfigurationParentNode> ParentNodes { get; set; }
    
    [Column(TypeName = "jsonb")] public List<WssdServiceInteractionConfigurationOption> Options { get; set; }
     
    public int TenantId { get; set; }
}