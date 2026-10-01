using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace RhyoliteOrbit.Models.Shared;

public class PaymentOption:Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(150)]
    public string Description { get; set; }
    
    public bool IsDefault { get; set; }
    [Column(TypeName = "jsonb")] public JsonDocument Exec { get; set; } // an executable plugin template that runs on sapoDSL
    [Column(TypeName = "jsonb")] public JsonDocument MetaData { get; set; }
    public int TenantId { get; set; }
}