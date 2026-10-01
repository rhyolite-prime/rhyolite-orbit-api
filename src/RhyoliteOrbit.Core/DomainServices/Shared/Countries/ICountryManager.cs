using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.Countries;

public interface ICountryManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    
    Task Create(Country input);
    Task<Country> GetNationality(string nationality);
    
    Task Update(Country input);
    
    Task Delete(Guid id);
    
    //region

    Task<object> ListRegions(string defaultRegion);
    Task CreateRegion(Region entity);
    Task UpdateRegion(Region entity);
    Task DeleteRegion(Guid id);
    

}