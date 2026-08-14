using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Pos;

public class PickupPoint: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string FriendlyName { get; set; }
    [MaxLength(100)]
    public string SystemName { get; set; }
    [MaxLength(350)]
    public string ImageUrl { get; set; }
    [MaxLength(50)]
    public string GhanaPostGps { get; set; }
    [MaxLength(100)]
    public string GpsCoordinates { get; set; }
    public int IsActive { get; set; }
    public int TenantId { get; set; }
}