using System;

namespace RhyoliteOrbit.Models.Shared;

public class MessageTemplate: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public string MessageContent { get; set; }
    public string MessageSubject { get; set; }
    public string Alias { get; set; }
    public int TenantId { get; set; }
}