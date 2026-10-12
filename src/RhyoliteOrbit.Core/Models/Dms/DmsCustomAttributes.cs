using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>A custom metadata field defined by a <see cref="DmsDocumentTemplate"/>.</summary>
public class DmsCustomAttributes: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid TemplateId { get; set; }

    [MaxLength(100)]
    public string TemplateName { get; set; }

    [MaxLength(100)]
    public string AttributeName { get; set; }

    /// <summary>Text, Number, Date, Boolean, Dropdown</summary>
    [MaxLength(20)]
    public string DataType { get; set; }

    [Column(TypeName = "jsonb")]
    public List<DropdownOption> PossibleValues { get; set; }

    [MaxLength(100)]
    public string DefaultValue { get; set; }

    public int OrderIndex { get; set; }

    public bool IsRequired { get; set; }

    /// <summary>Attribute is included in full text search when true.</summary>
    public bool IsSearchable { get; set; }

    /// <summary>Value is treated as PII and masked for users without the View permission.</summary>
    public bool IsSensitive { get; set; }

    [MaxLength(200)]
    public string ValidationRegex { get; set; }

    public int TenantId { get; set; }
}
