using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.CustomerGroups;

public interface ICustomerGroupManager : Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int id, int creationTime, string name);
    
    Task Create(CustomerGroup input);
    
    Task Update(CustomerGroup input);
    
    Task Delete(Guid id);
}