using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.ResultTypes;

public interface IResultTypeManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(Guid levelId);
    Task<object> Create(ResultType input);
    Task Update(ResultType input);
    Task Delete(Guid id);
    Task<object> ListByClass(Guid classId);
    Task<object> ListAll(int tenantId);
    
}