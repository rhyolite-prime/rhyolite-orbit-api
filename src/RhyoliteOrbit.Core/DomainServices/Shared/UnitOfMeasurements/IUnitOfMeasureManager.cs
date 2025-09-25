using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.UnitOfMeasurements;

public interface IUnitOfMeasureManager: IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task Create(UnitOfMeasure input);
    Task Update(UnitOfMeasure input);
    Task Delete(Guid id);
}