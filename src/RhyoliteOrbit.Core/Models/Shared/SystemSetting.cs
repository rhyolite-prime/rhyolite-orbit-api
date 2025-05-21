using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Shared;

public class SystemSetting: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    //email settings
    [MaxLength(100)]
    public string MailHostName { get; set; }
    public bool IsSslEnabled { get; set; }
    public int PortNumber { get; set; }
    [MaxLength(150)]
    public string PrimaryEmailAddress { get; set; }
    [MaxLength(100)]
    public string EmailPassword { get; set; }
    public int TenantId { get; set; }
}