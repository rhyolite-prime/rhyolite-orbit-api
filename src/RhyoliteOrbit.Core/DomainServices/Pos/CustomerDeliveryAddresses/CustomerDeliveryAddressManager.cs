using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Linq.Expressions;
using Akka.Actor;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Actors;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.CustomerDeliveryAddresses;

public class CustomerDeliveryAddressManager: DomainService, ICustomerDeliveryAddressManager
{
    private readonly IRepository<PosCustomerDeliveryAddress, Guid> _posCustomerDeliveryAddressRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IActorRef _dataSupportEntitySyncActorProvider;
    
    public CustomerDeliveryAddressManager(IRepository<PosCustomerDeliveryAddress, Guid> posCustomerDeliveryAddressRepository, TopLevelActors.DataSupportEntitySyncActorProvider dataSupportEntitySyncActorProvider, IUnitOfWorkManager unitOfWorkManager)
    {
        _posCustomerDeliveryAddressRepository = posCustomerDeliveryAddressRepository;
        _dataSupportEntitySyncActorProvider = dataSupportEntitySyncActorProvider();
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<PosCustomerDeliveryAddress>();

        predicate.And(x => !x.IsDeleted);
        
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.FullName.ToLower().Contains(query.ToLower()) || x.Landmark.ToLower().Contains(query.ToLower()) || x.PreciseLocation.ToLower().Contains(query.ToLower()));

        }
        
        var totalCount =  await _posCustomerDeliveryAddressRepository.CountAsync(predicate);
        
        var data = await (await _posCustomerDeliveryAddressRepository.GetAllAsync()).Where(predicate).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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

    public async Task<object> GetCustomerAddress(Guid customerId, int tenantId)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            return await _posCustomerDeliveryAddressRepository.FirstOrDefaultAsync(x=> x.CustomerId == customerId);
        }

    }

    public async Task CreateByCustomer(PosCustomerDeliveryAddress entity, int tenantId, string dataSupportBusinessAccountId)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            
            var existingAddress = await _posCustomerDeliveryAddressRepository.FirstOrDefaultAsync(x=> x.CustomerId == entity.CustomerId);
            if (existingAddress != null)
            {
                await _posCustomerDeliveryAddressRepository.UpdateAsync(existingAddress);
            }
            else
            {
                entity.TenantId = tenantId;
                await _posCustomerDeliveryAddressRepository.InsertAsync(entity);
            }
            
            var syncDataModel = new
            {
                EntityName = "pos_customer_delivery_address",
                Operation = "create",
                BusinessAccountId = dataSupportBusinessAccountId,
                Data = entity
            };
        
            _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
            
        }

    }
    
    public async Task Create(PosCustomerDeliveryAddress entity, string dataSupportBusinessAccountId)
    {
        await _posCustomerDeliveryAddressRepository.InsertAsync(entity);
        
        var syncDataModel = new
        {
            EntityName = "pos_customer_delivery_address",
            Operation = "create",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }

    public async Task Update(PosCustomerDeliveryAddress entity, string dataSupportBusinessAccountId)
    {
        await _posCustomerDeliveryAddressRepository.UpdateAsync(entity);
        
        var syncDataModel = new
        {
            EntityName = "pos_customer_delivery_address",
            Operation = "update",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }

    public async Task Delete(Guid id, string dataSupportBusinessAccountId)
    {
        var entity = await _posCustomerDeliveryAddressRepository.FirstOrDefaultAsync(id);
        await _posCustomerDeliveryAddressRepository.HardDeleteAsync(entity);
        
        var syncDataModel = new
        {
            EntityName = "pos_customer_delivery_address",
            Operation = "delete",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }
}