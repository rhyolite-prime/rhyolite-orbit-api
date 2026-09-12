using System;

namespace RhyoliteOrbit.Services.Pos.Categories.Dto;

public class PosInventoryCategoryDto
{
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public bool IsService { get; set; }
    public Guid ParentCategoryId { get; set; } = Guid.Empty;
    public string ImageUrl { get; set; }
}