using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.Services.Pos.Inventory.Dto;

public class PosInventoryItemDto
{
        public string Name { get; set; }
        public string Slug { get; set; }
        public string ShortDescription { get; set; }
        public List<string> FullDescription { get; set; }
        public string AdminComment { get; set; }
        public Guid MerchantId { get; set; }
        public string MerchantName { get; set; }
        public string Sku { get; set; }
        public List<string> Images { get; set; }
        public int StockQuantity { get; set; }
        public decimal OldSellingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal CostPrice { get; set; }
        
        //shipping
        
        public decimal Weight { get; set; } //kgs
        
        public bool IsFreeShipping { get; set; }
        
        public bool ShipSeparately { get; set; }
        
        public Guid UomCodeId { get; set; }
        
        public string UomName { get; set; }
    
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategorySlug { get; set; }
        
        public List<string> Tags { get; set; }
        
        public List<PosInventoryItemVariant> Variants { get; set; }
        
        public int MinimumCartQuantity { get; set; }
        
        public int MaximumCartQuantity { get; set; }
        
        public Guid BranchId { get; set; }
        public string BranchName { get; set; }
        
        public string ShelfLocation { get; set; }
        public bool IsPublished { get; set; }
        public bool AllowCustomerReviews { get; set; }
        
        public DateTime AvailableStartDate { get; set; }
        
        public DateTime AvailableEndDate { get; set; }
        public bool MarkAsNew { get; set; }
        public bool DisableBuyButton { get; set; }
        public bool DisableWishlistButton { get; set; }
        public bool AvailableForPreOrder { get; set; }
        public bool IsTaxExempt { get; set; }
        
        public bool IsNotReturnable { get; set; }
        
        public bool IsRental { get; set; }
        
        public bool IsService { get; set; }
        public int Rating { get; set; }
        public int ReviewsCount { get; set; }
        
        public List<ProductAttribute> ProductAttributes { get; set; }
        
        public List<PosInventoryItemSpecification> Specifications { get; set; }  
        
        public List<PosInventoryItemBulkPricing> BulkPricing { get; set; }  
        
        //seo
        
        public string MetaTitle { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string LeadTime { get; set; }
}

public class ProductSpecificationDto : EntityDto<Guid>
{
   public List<PosInventoryItemSpecification> Specifications { get; set; }
}

public class ProductAttributeDto 
{
    public Guid ItemId { get; set; }
    
    public List<ProductAttribute> Attributes { get; set; }
}