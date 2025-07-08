using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.CustomerGroups;

public interface IPartnerCategoryManager : Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    
    Task Create(PartnerCategory input);
    
    Task Update(PartnerCategory input);
    
    Task Delete(Guid id);
}