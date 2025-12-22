using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.ProductionPlanning;

namespace RhyoliteOrbit.DomainServices.ProductionPlanning.RawMaterials;

public interface IRawMaterialManager: IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> GetAsync(Guid id);
    Task Create(RawMaterial entity);
    Task Update(RawMaterial entity);
    Task Delete(Guid id);
}