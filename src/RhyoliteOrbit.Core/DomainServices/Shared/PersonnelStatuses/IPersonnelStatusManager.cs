using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.PersonnelStatuses;

public interface IPersonnelStatusManager : Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int id, int creationTime, string name);
    
    Task Create(PersonnelStatus input);
    
    Task Update(PersonnelStatus input);
    
    Task Delete(Guid id);
}