using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.Countries;

public interface ICountryManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    
    Task<object> Create(Country input);
    Task<Country> GetNationality(string nationality);
    
    Task Update(Country input);
    
    Task Delete(Guid id);
}