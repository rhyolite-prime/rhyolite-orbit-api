using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.CustomerDeliveryAddresses;

public interface ICustomerDeliveryAddressManager: IDomainService
{
    Task<object> GetCustomerAddress(Guid customerId, int tenantId);
    Task<object> GetAll(int pageNo, int pageSize, string query);
    Task CreateByCustomer(PosCustomerDeliveryAddress entity, int tenantId, string dataSupportBusinessAccountId);
    Task Create(PosCustomerDeliveryAddress entity, string dataSupportBusinessAccountId);
    Task Update(PosCustomerDeliveryAddress entity, string dataSupportBusinessAccountId);
    Task Delete(Guid id, string dataSupportBusinessAccountId);
}