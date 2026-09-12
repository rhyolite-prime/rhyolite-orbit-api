using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhyoliteOrbit.Models.Pos;

public class PosInventoryItem: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string Slug { get; set; }
        [MaxLength(500)]
        public string ShortDescription { get; set; }
        [Column(TypeName = "jsonb")] public List<string> FullDescription { get; set; }
        [MaxLength(250)]
        public string AdminComment { get; set; }
        public Guid MerchantId { get; set; }
        [MaxLength(150)]
        public string MerchantName { get; set; }
        [MaxLength(20)]
        public string Sku { get; set; }
        [Column(TypeName = "jsonb")] public List<string> Images { get; set; }
        public int StockQuantity { get; set; }
        public decimal OldSellingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal CostPrice { get; set; }
        
        //shipping
        
        public decimal Weight { get; set; } //kgs
        
        public bool IsFreeShipping { get; set; }
        
        public bool ShipSeparately { get; set; }
        
        public Guid UomCodeId { get; set; }
        
        [MaxLength(50)]
        public string UomName { get; set; }
    
        public Guid CategoryId { get; set; }
        [MaxLength(100)]
        public string CategoryName { get; set; }
        [MaxLength(150)]
        public string CategorySlug { get; set; }
        
        [Column(TypeName = "jsonb")] public List<string> Tags { get; set; }
        
        [Column(TypeName = "jsonb")] public List<PosInventoryItemVariant> Variants { get; set; }
        
        public int MinimumCartQuantity { get; set; }
        
        public int MaximumCartQuantity { get; set; }
        
        public Guid BranchId { get; set; }
        [MaxLength(50)]
        public string BranchName { get; set; }
        
        [MaxLength(150)]
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
        
        [Column(TypeName = "jsonb")] public List<ProductAttribute> ProductAttributes { get; set; }
        
        [Column(TypeName = "jsonb")] public List<PosInventoryItemSpecification> Specifications { get; set; }  
        
        [Column(TypeName = "jsonb")] public List<PosInventoryItemBulkPricing> BulkPricing { get; set; }  
        
        //seo
        
        [MaxLength(100)]
        public string MetaTitle { get; set; }
        [MaxLength(100)]
        public string MetaKeywords { get; set; }
        [MaxLength(150)]
        public string MetaDescription { get; set; }
        [MaxLength(100)]
        public string LeadTime { get; set; }
        public int TenantId { get; set; }
}