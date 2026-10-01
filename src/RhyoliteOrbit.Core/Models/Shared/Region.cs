using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Shared;

public class Region: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>
{
    public Guid CountryId { get; set; }
    
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    [Column(TypeName = "jsonb")] public List<City> Cities { get; set; } = new();
}