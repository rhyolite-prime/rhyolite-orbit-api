using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.PickupPoints;

public interface IPickupPointManager: IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query);
        
    Task Create(PickupPoint input, string dataSupportBusinessAccountId);
        
    Task Update(PickupPoint input, string dataSupportBusinessAccountId);
        
    Task Delete(Guid id, string dataSupportBusinessAccountId);
}