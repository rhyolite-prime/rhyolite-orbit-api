using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Shared;

public class SystemNumber: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public int ModuleId { get; set; }
    
    [MaxLength(100)]
    public string ItemName { get; set; }
    [MaxLength(100)]
    public string Key { get; set; }
    [MaxLength(100)]
    public string Prefix { get; set; }
    
    [MaxLength(100)]
    public string Suffix { get; set; }
    
    [MaxLength(50)]
    public string FormatPattern { get; set; } // eg. {PREFIX}{NO}{SUFFIX}
    
    public int Padding { get; set; }
    public int LastNo { get; set; }
    public int TenantId { get; set; }
}