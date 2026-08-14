using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Pos;

public class DeliveryChannel: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(150)]
    public string Name { get; set; }
    [MaxLength(350)]
    public string Description { get; set; }
    public bool IsLocal { get; set; }
    [Column(TypeName = "jsonb")] public List<DeliveryZoneRate> ZoneRates { get; set; }
    public int TenantId { get; set; }
}