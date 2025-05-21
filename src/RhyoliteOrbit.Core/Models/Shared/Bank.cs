using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Shared;

public class Bank: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(150)]
    public string Name { get; set; }
    [MaxLength(150)]
    public string ContactPerson { get; set; }
    [MaxLength(50)]
    public string ContactNo { get; set; }
    [MaxLength(50)]
    public string BankCode { get; set; }
    [MaxLength(150)]
    public string Email { get; set; }
    [Column(TypeName = "jsonb")] public List<BankBranch> BankBranches { get; set; }
    public int TenantId { get; set; }
}