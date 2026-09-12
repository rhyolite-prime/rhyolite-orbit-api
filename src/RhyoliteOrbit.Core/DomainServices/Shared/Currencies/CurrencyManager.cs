using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Akka.Actor;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Actors;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.Currencies;

public class CurrencyManager : Abp.Domain.Services.DomainService, ICurrencyManager
{
    private readonly IRepository<Currency, Guid> _repositoryCurrency;
    private readonly IActorRef _dataSupportEntitySyncActorProvider;

    public CurrencyManager(IRepository<Currency, Guid> repositoryCurrency, TopLevelActors.DataSupportEntitySyncActorProvider dataSupportEntitySyncActorProvider)
    {
        _repositoryCurrency = repositoryCurrency;
        _dataSupportEntitySyncActorProvider = dataSupportEntitySyncActorProvider();
    }

    public async Task Create(Currency entity, string dataSupportBusinessAccountId)
    {
        var datta = await _repositoryCurrency.FirstOrDefaultAsync(x => x.Name == entity.Name );

        if (datta != null)
        {
            throw new UserFriendlyException(400, "Duplicate records are not allowed.");
        }
        
        await _repositoryCurrency.InsertAsync(entity);
        
        var syncDataModel = new
        {
            EntityName = "erp_currency",
            Operation = "create",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }
    
    public async Task Update(Currency entity, string dataSupportBusinessAccountId)
    {
        await _repositoryCurrency.UpdateAsync(entity);
        
        var syncDataModel = new
        {
            EntityName = "erp_currency",
            Operation = "update",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }
    
    public async Task<Currency> GetAsync(string currencyName)
    {
        return await _repositoryCurrency.FirstOrDefaultAsync(x=> x.Name.ToLower() == currencyName.Trim().ToLower() );
    }


    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<Currency>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(a => a.Name.Contains(query));

        }
            
        var totalCount =
                
            await _repositoryCurrency.CountAsync(predicate);
            
        var data = await (await _repositoryCurrency.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.Code,a.IsDefault, a.BuyRate, a.SellRate, a.MinorName }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        var currencyInfo = await _repositoryCurrency.FirstOrDefaultAsync(id);
        
        await _repositoryCurrency.DeleteAsync(id);
        
        var syncDataModel = new
        {
            EntityName = "erp_currency",
            Operation = "delete",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = currencyInfo
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }
}