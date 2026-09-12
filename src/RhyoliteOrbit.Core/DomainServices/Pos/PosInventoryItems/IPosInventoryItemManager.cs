using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.PosInventoryItems;

public interface IPosInventoryItemManager: IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query, string shelfLocation, string branchId, string categoryId, string merchantId,string itemType);
    Task Create(PosInventoryItem input, string dataSupportBusinessAccountId);
    Task Update(PosInventoryItem input, string dataSupportBusinessAccountId);
    Task Delete(Guid id, string dataSupportBusinessAccountId);
    Task UpdateProductSpecification(Guid id, List<PosInventoryItemSpecification> specifications, string dataSupportBusinessAccountId);
    Task AddProductAttributes(Guid itemId, List<ProductAttribute> attributes, string dataSupportBusinessAccountId);
    Task UpdateProductAttributes(Guid itemId, List<ProductAttribute> attributes, string dataSupportBusinessAccountId);
    Task<object> GetProductSpecifications(Guid itemId);
    Task<List<ProductAttribute>> GetProductAttributes(Guid itemId);
    
    Task<object> GetDetails(Guid id);
    
}