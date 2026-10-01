using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;

namespace RhyoliteOrbit.Models.Pos;

public class BannerSlider: FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(200)]
    public string Image { get; set; }  
    [MaxLength(200)]
    public string ImageMobile { get; set; }
    [MaxLength(100)]
    public string ImageAlt { get; set; }
    
    [MaxLength(200)]
    public string Link { get; set; }
    public int TenantId { get; set; }
}