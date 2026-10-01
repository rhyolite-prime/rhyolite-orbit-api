using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Shared;

public class Country: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>
{
    [MaxLength(150)]
    public string Name { get; set; }
    [MaxLength(20)]
    public string IsoCode { get; set; }
    public int NumericIsoCode { get; set; }
    [MaxLength(150)]
    public string Nationality { get; set; }
}