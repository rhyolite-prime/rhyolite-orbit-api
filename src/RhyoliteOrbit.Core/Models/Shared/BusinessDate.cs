using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Shared;

public class BusinessDate:Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>
{
    [MaxLength(100)]
    public string Type { get; set; }
    public DateTime ActualDate { get; set; }
}