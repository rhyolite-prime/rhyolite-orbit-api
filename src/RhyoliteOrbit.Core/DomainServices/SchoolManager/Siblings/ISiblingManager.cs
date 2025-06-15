using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.Siblings;

public interface ISiblingManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(Guid studentId);
    Task Create(Sibling input);
    Task Update(Sibling input);
    Task Delete(Guid id);
}