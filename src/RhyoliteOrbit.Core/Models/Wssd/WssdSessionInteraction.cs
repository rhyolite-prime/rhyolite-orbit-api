using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Wssd;

public class WssdSessionInteraction: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(80)]
    public string SessionId { get; set; }
    
    [MaxLength(80)]
    public string WssdServiceId { get; set; }
    
    [MaxLength(150)]
    public string WssdServiceName { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    [MaxLength(20)]
    public string InteractionSource { get; set; }
    
    [MaxLength(20)]
    public string MobileNo { get; set; }
    
    [MaxLength(50)]
    public string Operator { get; set; } //mtn, telecel, vodafone
    
    [Column(TypeName = "jsonb")] public List<InteractionDetail> Details { get; set; }
    
    public int Score { get; set; }
    
    public int TenantId { get; set; }
}