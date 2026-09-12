using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.GiftCards;

public interface IGiftCardManager:IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query);
    Task Create(GiftCard input);
    Task Update(GiftCard input);
    Task Delete(Guid id);
}