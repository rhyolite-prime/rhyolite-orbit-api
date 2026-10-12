using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Shared;
 
namespace RhyoliteOrbit.DomainServices.Shared.Currencies;

public interface ICurrencyManager : Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> ListAll(int tenantId);
    Task<object> GetDefaultCurrency(int tenantId);
    Task Create(Currency input, string dataSupportBusinessAccountId);
    Task<Currency> GetAsync(string currencyName);
    Task Update(Currency input, string dataSupportBusinessAccountId);
    Task Delete(Guid id, string dataSupportBusinessAccountId);
}