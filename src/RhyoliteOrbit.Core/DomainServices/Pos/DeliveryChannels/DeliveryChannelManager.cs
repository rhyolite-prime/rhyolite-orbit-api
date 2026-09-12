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

namespace RhyoliteOrbit.DomainServices.Pos.DeliveryChannels;

public class DeliveryChannelManager: DomainService, IDeliveryChannelManager
{
    private readonly IRepository<DeliveryChannel, Guid> _repositoryDeliveryChannel;
    private readonly IActorRef _dataSupportEntitySyncActorProvider;

    public DeliveryChannelManager(IRepository<DeliveryChannel, Guid> repositoryDeliveryChannel, TopLevelActors.DataSupportEntitySyncActorProvider dataSupportEntitySyncActorProvider)
    {
        _repositoryDeliveryChannel = repositoryDeliveryChannel;
        _dataSupportEntitySyncActorProvider = dataSupportEntitySyncActorProvider();
    }


    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<DeliveryChannel>();

        predicate.And(x => !x.IsDeleted);
        
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.ToLower().Contains(query.ToLower()));
        }
        
        var totalCount =
            
            await _repositoryDeliveryChannel.CountAsync(predicate);
        
        var data = await (await _repositoryDeliveryChannel.GetAllAsync()).Where(predicate).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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


    public async Task Create(DeliveryChannel entity, string dataSupportBusinessAccountId)
    {
        var datta = await _repositoryDeliveryChannel.FirstOrDefaultAsync(x => x.Name == entity.Name);
        if (datta != null)
        {
            throw new UserFriendlyException(400, "Duplicate records are not allowed.");
             
        }
        
        await _repositoryDeliveryChannel.InsertAsync(entity);
        
        var syncDataModel = new
        {
            EntityName = "pos_delivery_channel",
            Operation = "create",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);

    }


    public async Task Update(DeliveryChannel entity, string dataSupportBusinessAccountId)
    {
        await _repositoryDeliveryChannel.UpdateAsync(entity);
        
        var syncDataModel = new
        {
            EntityName = "pos_delivery_channel",
            Operation = "update",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }

    public async Task Delete(Guid id, string dataSupportBusinessAccountId)
    {
        var deliveryChannelInfo = await _repositoryDeliveryChannel.FirstOrDefaultAsync(id);
        
        await _repositoryDeliveryChannel.DeleteAsync(id);
        
        var syncDataModel = new
        {
            EntityName = "pos_delivery_channel",
            Operation = "update",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = deliveryChannelInfo
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }
}