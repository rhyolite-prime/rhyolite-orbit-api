using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>
/// Auto numbering scheme producing human readable document identifiers
/// (e.g. INV-2026-000123).
/// </summary>
public class DmsCustomIdentifier: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }

    public string Prefix { get; set; }

    public string Suffix { get; set; }

    public bool IncludeYear { get; set; }

    public bool IncludeMonth { get; set; }

    public int PaddingLength { get; set; }

    public int CurrentSequence { get; set; }

    public Guid TemplateId { get; set; }

    public string TemplateName { get; set; }

    public bool IsActive { get; set; }

    /// <summary>0 = Never, 1 = Yearly, 2 = Monthly (see <see cref="RhyoliteOrbitConsts.SequenceResetPolicy"/>).</summary>
    public int ResetPolicy { get; set; }

    public System.DateTime? LastGeneratedAt { get; set; }

    public int TenantId { get; set; }
}
