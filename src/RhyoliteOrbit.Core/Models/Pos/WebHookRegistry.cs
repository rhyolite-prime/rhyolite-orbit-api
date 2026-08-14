using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Pos;

public class WebHookRegistry: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(150)]
    public string Name { get; set; }
    
    [MaxLength(350)]
    public string EndpointUrl { get; set; }  // httsp://webhook.rhyoliteprime.com/
    
    public int TenantId { get; set; }
}