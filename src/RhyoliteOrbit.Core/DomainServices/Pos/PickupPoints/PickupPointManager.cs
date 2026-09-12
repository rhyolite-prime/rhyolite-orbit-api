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

namespace RhyoliteOrbit.DomainServices.Pos.PickupPoints;

public class PickupPointManager: DomainService, IPickupPointManager
{
    private readonly IRepository<PickupPoint, Guid> _repositoryPickupPoint;
    private readonly IActorRef _dataSupportEntitySyncActorProvider;

    public PickupPointManager(IRepository<PickupPoint, Guid> repositoryPickupPoint, TopLevelActors.DataSupportEntitySyncActorProvider dataSupportEntitySyncActorProvider)
    {
        _repositoryPickupPoint = repositoryPickupPoint;
        _dataSupportEntitySyncActorProvider = dataSupportEntitySyncActorProvider();
    }


    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<PickupPoint>();

        predicate.And(x => !x.IsDeleted);
        
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.SystemName.ToLower().Contains(query.ToLower()) || x.FriendlyName.ToLower().Contains(query.ToLower()));

        }
        
        var totalCount =
            
            await _repositoryPickupPoint.CountAsync(predicate);
        
        var data = await (await _repositoryPickupPoint.GetAllAsync()).Where(predicate).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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


    public async Task Create(PickupPoint entity, string dataSupportBusinessAccountId)
    {
        var datta = await _repositoryPickupPoint.FirstOrDefaultAsync(x => x.SystemName == entity.SystemName);
        if (datta != null)
        {
            throw new UserFriendlyException(400, "Duplicate records are not allowed.");
        }
        
        await _repositoryPickupPoint.InsertAsync(entity);
        
        var syncDataModel = new
        {
            EntityName = "pos_pickup_points",
            Operation = "create",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);

    }


    public async Task Update(PickupPoint entity, string dataSupportBusinessAccountId)
    {
        await _repositoryPickupPoint.UpdateAsync(entity);
        
        var syncDataModel = new
        {
            EntityName = "pos_pickup_points",
            Operation = "update",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }

    public async Task Delete(Guid id, string dataSupportBusinessAccountId)
    {
        var pickupPointInfo = await _repositoryPickupPoint.FirstOrDefaultAsync(id);
        
        await _repositoryPickupPoint.DeleteAsync(id);
        
        var syncDataModel = new
        {
            EntityName = "pos_pickup_points",
            Operation = "delete",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = pickupPointInfo
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }
}