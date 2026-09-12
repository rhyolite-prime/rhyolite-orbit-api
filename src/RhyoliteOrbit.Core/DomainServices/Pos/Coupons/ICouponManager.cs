using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.Coupons;

public interface ICouponManager: IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query);
    Task Create(Coupon input, string dataSupportBusinessAccountId);
    Task Update(Coupon input, string dataSupportBusinessAccountId);
    Task Delete(Guid id, string dataSupportBusinessAccountId);
}