using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Pos;

public class PosSalesAgent: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(250)]
    public string Name { get; set; }
    public Guid BranchId { get; set; }
    [MaxLength(200)]
    public string BranchName { get; set; }
    [MaxLength(350)]
    public string LogoUrl { get; set; }
    [MaxLength(150)]
    public string Description { get; set; }
    public bool IsActive { get; set; }
    [MaxLength(15)]
    public Guid MerchantId { get; set; }
    [MaxLength(100)]
    public string MerchantName { get; set; }
    [MaxLength(50)]
    public string Status { get; set; } // awaiting deployment, learning mode, busy, experimental, limited functionality, unspecialized,
    public int TenantId { get; set; }
}