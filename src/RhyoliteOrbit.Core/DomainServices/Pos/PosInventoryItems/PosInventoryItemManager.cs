using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Akka.Actor;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Actors;
using RhyoliteOrbit.Models.Pos;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Pos.PosInventoryItems;

public class PosInventoryItemManager: DomainService, IPosInventoryItemManager
{
    private readonly IRepository<PosInventoryItem, Guid> _repositoryItem;
    private readonly IRepository<Merchant, Guid> _repositoryMerchant;
    private readonly IRepository<InventoryCategory, Guid> _repositoryCategory;
    private readonly IRepository<PosBranch, Guid> _repositoryBranch;
    private readonly IRepository<UomCode, Guid> _repositoryUom;
    private readonly IRepository<BusinessProfile, Guid> _repositoryBusinessProfile;
    private readonly IActorRef _dataSupportEntitySyncActorProvider;

    public PosInventoryItemManager(IRepository<PosInventoryItem, Guid> repositoryItem, TopLevelActors.DataSupportEntitySyncActorProvider dataSupportEntitySyncActorProvider, IRepository<Merchant, Guid> repositoryMerchant, IRepository<InventoryCategory, Guid> repositoryCategory, IRepository<PosBranch, Guid> repositoryBranch, IRepository<UomCode, Guid> repositoryUom, IRepository<BusinessProfile, Guid> repositoryBusinessProfile)
    {
        _repositoryItem = repositoryItem;
        _repositoryMerchant = repositoryMerchant;
        _repositoryCategory = repositoryCategory;
        _repositoryBranch = repositoryBranch;
        _repositoryUom = repositoryUom;
        _repositoryBusinessProfile = repositoryBusinessProfile;
        _dataSupportEntitySyncActorProvider = dataSupportEntitySyncActorProvider();
    }

    public async Task<object> GetAll(int pageNo, int pageSize, string query, string shelfLocation, string branchId, string categoryId, string merchantId, string itemType)
    {
        
        var predicate = PredicateBuilder.New<PosInventoryItem>();

        predicate.And(x => !x.IsDeleted);
        
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.ToLower().Contains(query.ToLower()) || x.Sku.ToLower().Contains(query.ToLower()));
        }
        
        if (!string.IsNullOrEmpty(itemType))
        {
            if (itemType.Equals("product"))
            {
                predicate.And(x => !x.IsService);
            }
            else
            {
                predicate.And(x => x.IsService);
            }
            
        }
        
        if (!string.IsNullOrEmpty(shelfLocation))
        {
            predicate.And(x => x.ShelfLocation.Contains(shelfLocation));
        }
        
        if (!string.IsNullOrEmpty(branchId))
        {
            predicate.And(x => x.BranchId == Guid.Parse(branchId));
        }
        
        if (!string.IsNullOrEmpty(categoryId))
        {
            predicate.And(x => x.CategoryId == Guid.Parse(categoryId));
        }
        
        if (!string.IsNullOrEmpty(merchantId))
        {
            predicate.And(x => x.MerchantId == Guid.Parse(merchantId));
        }
        
        var totalCount =
            
            await _repositoryItem.CountAsync(predicate);
        
        var data = await (await _repositoryItem.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Images, ImageUrl = a.Images.FirstOrDefault(), a.Name, a.ShortDescription, a.CostPrice, a.SellingPrice, a.MerchantId, a.MerchantName, a.CategoryId, a.CategoryName, a.Sku, a.BranchId, a.BranchName, a.ShelfLocation, a.StockQuantity, a.IsFreeShipping, a.IsService }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

        var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

        var paginatedResult = new
        {
            pageNo,
            totalCount,
            totalPages,
            data,
            lowerBound = pageSize * (pageNo - 1) + 1,
            upperBound = totalPages == pageNo ? totalCount : pageNo * pageSize,
        };

        return paginatedResult;
        
    }

    public async Task Create(PosInventoryItem entity, string dataSupportBusinessAccountId)
    {
        var datta = await _repositoryItem.FirstOrDefaultAsync(x => x.Sku == entity.Sku);
        if (datta != null)
        {
            throw new UserFriendlyException(400, "Duplicate sku records are not allowed.");
        }
            
        await _repositoryItem.InsertAsync(entity);
        //update count of products in selected product category

        var categoryInfo = await _repositoryCategory.FirstOrDefaultAsync(entity.CategoryId);
        if (categoryInfo != null)
        {
            var countOfCategoryItems = await _repositoryItem.CountAsync(x => x.Id == entity.CategoryId);
            categoryInfo.ProductCount = countOfCategoryItems + 1;
            await _repositoryCategory.UpdateAsync(categoryInfo);
        }
        
        var syncDataModel = new
        {
            EntityName = "pos_inventory_items",
            Operation = "create",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
        

    }

    public async Task Update(PosInventoryItem entity, string dataSupportBusinessAccountId)
    {
        await _repositoryItem.UpdateAsync(entity);
        
        //update count of products in selected product category

        var categoryInfo = await _repositoryCategory.FirstOrDefaultAsync(entity.CategoryId);
        if (categoryInfo != null)
        {
            var countOfCategoryItems = await _repositoryItem.CountAsync(x => x.Id == entity.CategoryId);
            categoryInfo.ProductCount = countOfCategoryItems + 1;
            await _repositoryCategory.UpdateAsync(categoryInfo);
        }
        
        var syncDataModel = new
        {
            EntityName = "pos_inventory_items",
            Operation = "update",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }

    public async Task UpdateProductSpecification(Guid id, List<PosInventoryItemSpecification> productSpecifications, string dataSupportBusinessAccountId)
    {
        var item = await _repositoryItem.FirstOrDefaultAsync(id);
        if (item == null)
        {
            throw new UserFriendlyException(404, "Item not found.");
        }

        item.Specifications = productSpecifications;
        await _repositoryItem.UpdateAsync(item);
        
        var syncDataModel = new
        {
            EntityName = "pos_inventory_items",
            Operation = "update",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = item
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }
    
    public async Task AddProductAttributes(Guid itemId, List<ProductAttribute> attributes,string dataSupportBusinessAccountId)
    {
        var item = await _repositoryItem.FirstOrDefaultAsync(itemId);
        if (item == null)
            throw new UserFriendlyException(404, "Item not found.");
    
        item.ProductAttributes ??= new List<ProductAttribute>();
    
        // Add all attributes from the list
        item.ProductAttributes.AddRange(attributes);
        await _repositoryItem.UpdateAsync(item);
        
        var syncDataModel = new
        {
            EntityName = "pos_inventory_items",
            Operation = "update",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = item
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
        
    }
    
    public async Task UpdateProductAttributes(Guid itemId, List<ProductAttribute> attributes, string dataSupportBusinessAccountId)
    {
        var item = await _repositoryItem.FirstOrDefaultAsync(itemId);
        if (item == null)
            throw new UserFriendlyException(404, "Item not found.");
    
        if (item.ProductAttributes == null)
            throw new UserFriendlyException(400, "No product attributes found.");
    
        // Update each attribute if it exists
        foreach (var attribute in attributes)
        {
            var index = item.ProductAttributes.FindIndex(a => a.Id == attribute.Id);
            if (index == -1)
                throw new UserFriendlyException(404, $"Product attribute with ID {attribute.Id} not found.");
    
            item.ProductAttributes[index] = attribute;
        }
        
        await _repositoryItem.UpdateAsync(item);
        
        var syncDataModel = new
        {
            EntityName = "pos_inventory_items",
            Operation = "update",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = item
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }
    
    public async Task<object> GetProductSpecifications(Guid itemId)
    {
        var item = await _repositoryItem.FirstOrDefaultAsync(itemId);
        if (item == null)
        {
            throw new UserFriendlyException(404, "Item not found.");
        }
    
        return item.Specifications;
    }
    
    public async Task<List<ProductAttribute>> GetProductAttributes(Guid itemId)
    {
        var item = await _repositoryItem.FirstOrDefaultAsync(itemId);
        if (item == null)
        {
            throw new UserFriendlyException(404, "Item not found.");
        }
    
        return item.ProductAttributes ?? new List<ProductAttribute>();
    }
    
    public async Task Delete(Guid id, string dataSupportBusinessAccountId)
    {
        var itemInfo = await _repositoryItem.FirstOrDefaultAsync(id);
        
        await _repositoryItem.DeleteAsync(id);
        
        var syncDataModel = new
        {
            EntityName = "pos_inventory_items",
            Operation = "delete",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = itemInfo
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }
    
    public async Task<object> GetDetails(Guid id)
    {
       return await _repositoryItem.FirstOrDefaultAsync(id);
    }
    
    
}