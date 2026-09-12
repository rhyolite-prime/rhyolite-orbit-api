using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.InventoryCategories;

public interface IInventoryCategoryManager:IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query);
    Task Create(InventoryCategory input, string dataSupportBusinessAccountId);
    Task Update(InventoryCategory input, string dataSupportBusinessAccountId);
    Task Delete(Guid id, string dataSupportBusinessAccountId);
}