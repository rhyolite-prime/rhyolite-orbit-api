using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Shared;

public class Country: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>
{
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(20)]
    public string Code { get; set; }
    public int NumericIsoCode { get; set; }
    [MaxLength(100)]
    public string Nationality { get; set; }
}