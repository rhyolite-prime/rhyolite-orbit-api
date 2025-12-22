using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Shared;

public class MessageTemplate: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(350)]
    public string MessageContent { get; set; }
    [MaxLength(100)]
    public string MessageSubject { get; set; }
    [MaxLength(50)]
    public string Alias { get; set; }
    public int TenantId { get; set; }
}   