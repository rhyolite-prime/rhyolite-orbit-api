using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.PersonnelStatuses;

public interface IPersonnelStatusManager : Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);

    Task<PersonnelStatus> GetAsync(string statusName);
    
    Task Create(PersonnelStatus input);
    
    Task Update(PersonnelStatus input);
    
    Task Delete(Guid id);
}