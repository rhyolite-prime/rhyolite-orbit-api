using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Pos;

public class PosBranch: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(200)]
    public string Name { get; set; }
    [MaxLength(200)]
    public string Location { get; set; }
    [MaxLength(200)]

    public string CountryName { get; set; }
    [MaxLength(20)]
    public string CountryCode { get; set; }
    [MaxLength(100)]
    public string RegionOrState { get; set; }
    [MaxLength(100)]
    public string City { get; set; }
    [MaxLength(250)]
    public string Address { get; set; }
    [MaxLength(15)]
    public string PhoneNo { get; set; }
    [MaxLength(200)]
    public string BranchManager { get; set; }
        
    public Guid RevenueAccountId { get; set; }
    [MaxLength(50)]
    public string RevenueAccountNo { get; set; }
    [MaxLength(100)]
    public string GeoCoordinates { get; set; }
    public bool IsDefault { get; set; }
    public int TenantId { get; set; }
}