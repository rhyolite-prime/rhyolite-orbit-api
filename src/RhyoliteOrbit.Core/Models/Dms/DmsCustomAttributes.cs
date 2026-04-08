using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Dms;

public class DmsCustomAttributes: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public Guid TemplateId { get; set; }
    [MaxLength(100)]
    public string TemplateName { get; set; }
    [MaxLength(100)]
    public string AttributeName { get; set; }
    [MaxLength(20)]
    public string DataType { get; set; } // Text, Number, Date, Boolean, Dropdown options
    [Column(TypeName = "jsonb")] public List<DropdownOption> PossibleValues { get; set; } // Stores dropdown options if DataType is Dropdown
    [MaxLength(100)]
    public string DefaultValue { get; set; }  
    public int OrderIndex { get; set; }  
    public bool IsRequired { get; set; }  
    public int TenantId { get; set; }
}