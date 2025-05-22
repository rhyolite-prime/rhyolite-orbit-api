using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Shared;
 
namespace RhyoliteOrbit.DomainServices.Shared.Currencies;

public interface ICurrencyManager : Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);

    Task Create(Currency input);


    Task Update(Currency input);

    Task Delete(Guid id);
}