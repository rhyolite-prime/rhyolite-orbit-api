using System;
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

namespace RhyoliteOrbit.DomainServices.Pos.InventoryCategories;

public class InventoryCategoryManager: DomainService, IInventoryCategoryManager
{
    private readonly IRepository<InventoryCategory, Guid> _repositoryInventoryCategory;
    private readonly IActorRef _dataSupportEntitySyncActorProvider;

    public InventoryCategoryManager(IRepository<InventoryCategory, Guid> repositoryInventoryCategory, TopLevelActors.DataSupportEntitySyncActorProvider dataSupportEntitySyncActorProvider)
    {
        _repositoryInventoryCategory = repositoryInventoryCategory;
        _dataSupportEntitySyncActorProvider = dataSupportEntitySyncActorProvider();
    }


    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<InventoryCategory>();

        predicate.And(x => !x.IsDeleted);
        
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.ToLower().Contains(query.ToLower()));

        }
        
        var totalCount =
            
            await _repositoryInventoryCategory.CountAsync(predicate);
        
        var data = await (await _repositoryInventoryCategory.GetAllAsync()).Where(predicate).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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


    public async Task Create(InventoryCategory entity, string dataSupportBusinessAccountId)
    {
        var datta = await _repositoryInventoryCategory.FirstOrDefaultAsync(x => x.Name == entity.Name);
        if (datta != null)
        {
            throw new UserFriendlyException(400, "Duplicate records are not allowed.");
        }
        
        await _repositoryInventoryCategory.InsertAsync(entity);
        
        var syncDataModel = new
        {
            EntityName = "pos_inventory_category",
            Operation = "create",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);

    }


    public async Task Update(InventoryCategory entity, string dataSupportBusinessAccountId)
    {
        await _repositoryInventoryCategory.UpdateAsync(entity);
        
        var syncDataModel = new
        {
            EntityName = "pos_inventory_category",
            Operation = "update",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }

    public async Task Delete(Guid id, string dataSupportBusinessAccountId)
    {
        var inventoryCategory = await _repositoryInventoryCategory.FirstOrDefaultAsync(id);
        
        await _repositoryInventoryCategory.DeleteAsync(id);
        
        var syncDataModel = new
        {
            EntityName = "pos_inventory_category",
            Operation = "delete",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = inventoryCategory
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }
}