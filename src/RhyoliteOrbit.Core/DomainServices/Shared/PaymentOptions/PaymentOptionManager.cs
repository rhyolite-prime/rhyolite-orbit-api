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
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.PaymentOptions;

public class PaymentOptionManager : DomainService, IPaymentOptionManager
{
    private readonly IRepository<PaymentOption, Guid> _repositoryPaymentOption;
    private readonly IActorRef _dataSupportEntitySyncActorProvider;
    
    public PaymentOptionManager(IRepository<PaymentOption, Guid> repositoryPaymentOption, TopLevelActors.DataSupportEntitySyncActorProvider dataSupportEntitySyncActorProvider)
    {
        _repositoryPaymentOption = repositoryPaymentOption;
        _dataSupportEntitySyncActorProvider = dataSupportEntitySyncActorProvider();
    }

    public async Task Create(PaymentOption entity, string dataSupportBusinessAccountId)
    {
        var datta = await _repositoryPaymentOption.FirstOrDefaultAsync(x => x.Name == entity.Name);

        if (datta != null)
        {
            throw new UserFriendlyException(400, "Duplicate records are not allowed.");
        }
        
        await _repositoryPaymentOption.InsertAsync(entity);
        
        var syncDataModel = new
        {
            EntityName = "erp_payment_option",
            Operation = "create",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }
    
    public async Task<object> GetDetails(Guid id)
    {
        return await _repositoryPaymentOption.FirstOrDefaultAsync(id);
    }
    
    public async Task Update(PaymentOption entity, string dataSupportBusinessAccountId)
    {
        await _repositoryPaymentOption.UpdateAsync(entity);
        
        var syncDataModel = new
        {
            EntityName = "erp_payment_option",
            Operation = "update",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<PaymentOption>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(a => a.Name.Contains(query));

        }
            
        var totalCount = await _repositoryPaymentOption.CountAsync(predicate);
            
        var data = await (await _repositoryPaymentOption.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name,a.Description, a.CreationTime, a.Exec, a.MetaData }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
    
    public async Task Delete(Guid id, string dataSupportBusinessAccountId)
    {
        var entity = await _repositoryPaymentOption.FirstOrDefaultAsync(id);
        await _repositoryPaymentOption.DeleteAsync(id);
        
        var syncDataModel = new
        {
            EntityName = "erp_payment_option",
            Operation = "delete",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }
    
    public async Task<object> GetAll()
    {
        return await _repositoryPaymentOption.GetAllListAsync();
    }
}