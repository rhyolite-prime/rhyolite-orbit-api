using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Pos;

public class InventoryCategory: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(150)]
    public string Name { get; set; }
    [MaxLength(150)]
    public string Slug { get; set; }
    [MaxLength(350)]
    public string ImageUrl { get; set; }
    [MaxLength(1000)]
    public string Description { get; set; }
    public bool IsService { get; set; }
    public Guid ParentCategoryId { get; set; }
    public int ProductCount { get; set; }
    public int TenantId { get; set; }
}