using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;

namespace RhyoliteOrbit.Models.Pos;

public class PosCustomerDeliveryAddress: FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(200)]
    public string FullName { get; set; }
    
    [MaxLength(20)]
    public string PhoneNo { get; set; }
    
    [MaxLength(20)]
    public string AdditionalPhoneNo { get; set; }
    
    [MaxLength(150)]
    public string Region { get; set; }
    
    [MaxLength(150)]
    public string City { get; set; }
    
    [MaxLength(250)]
    public string Landmark { get; set; }
    
    [MaxLength(250)]
    public string PreciseLocation { get; set; }
    
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
    
    public Guid CustomerId { get; set; }
    
    public int TenantId { get; set; }
}