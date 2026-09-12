using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.DeliveryChannels;

public interface IDeliveryChannelManager: IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query);
    Task Create(DeliveryChannel input, string dataSupportBusinessAccountId);
    Task Update(DeliveryChannel input, string dataSupportBusinessAccountId);
    Task Delete(Guid id, string dataSupportBusinessAccountId);
}