using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Dms;

public class DmsCustomIdentifier: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }
    public string Prefix { get; set; }
    public string Suffix { get; set; }
    public bool IncludeYear { get; set; }
    public bool IncludeMonth { get; set; }
    public int PaddingLength { get; set; } // e.g., 4 => 0001
    public int CurrentSequence { get; set; }
    public Guid TemplateId { get; set; }
    public string TemplateName { get; set; }
    public int TenantId { get; set; }
}