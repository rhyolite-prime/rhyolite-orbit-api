using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.CostCenters;

public interface ICostCenterManager : Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    
    Task Create(CostCenter input);
    
    Task Update(CostCenter input);
    
    Task Delete(Guid id);
}