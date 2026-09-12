using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Akka.Actor;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Actors;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.Coupons;

public class CouponManager
{
    private readonly IRepository<Coupon, Guid> _repositoryCoupon;
    private readonly IRepository<PosBranch, Guid> _repositoryBranch;
    private readonly IActorRef _dataSupportEntitySyncActorProvider;

    public CouponManager(IRepository<Coupon, Guid> repositoryCoupon, IRepository<PosBranch, Guid> repositoryBranch, TopLevelActors.DataSupportEntitySyncActorProvider dataSupportEntitySyncActorProvider)
    {
        _repositoryCoupon = repositoryCoupon;
        _repositoryBranch = repositoryBranch;
        _dataSupportEntitySyncActorProvider = dataSupportEntitySyncActorProvider();
    }


    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<Coupon>();

        predicate.And(x => !x.IsDeleted);
        
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.CouponIdentifier.Contains(query));

        }
        
        var totalCount =
            
            await _repositoryCoupon.CountAsync(predicate);
        
        var data = await (await _repositoryCoupon.GetAllAsync()).Where(predicate).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

        var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

        var paginatedResult = new
        {
            pageNo,
            totalCount,
            totalPages,
            data,
            lowerBound = pageSize * (pageNo - 1) + 1,
            upperBound = totalPages == pageNo ? totalCount : pageNo * pageSize,
        };

        return paginatedResult;
        
    }

    public async Task Create(Coupon entity, string dataSupportBusinessAccountId)
    {
        var existingCoupon = await _repositoryCoupon.FirstOrDefaultAsync(x => x.CouponIdentifier == entity.CouponIdentifier);
        if (existingCoupon != null)
        {
            throw new UserFriendlyException(400, "Duplicate records are not allowed.");
        }

        var branchInfo = await _repositoryBranch.GetAsync(entity.BranchId);
        if (branchInfo == null)
        {
            throw new UserFriendlyException(400, "Selected branch not found.");
        }

        entity.BranchName = branchInfo.Name;
        await _repositoryCoupon.InsertAsync(entity);
        
        var syncDataModel = new
        {
            EntityName = "pos_coupon",
            Operation = "create",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }


    public async Task Update(Coupon entity, string dataSupportBusinessAccountId)
    {
        var branchInfo = await _repositoryBranch.GetAsync(entity.BranchId);

        if (branchInfo == null)
        {
            throw new UserFriendlyException(400, "Selected branch not found.");  
        }
        
        entity.BranchName = branchInfo.Name;
        await _repositoryCoupon.UpdateAsync(entity);
        
        var syncDataModel = new
        {
            EntityName = "pos_coupon",
            Operation = "update",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
        
    }

    public async Task Delete(Guid id, string dataSupportBusinessAccountId)
    {
        var couponInfo = await _repositoryCoupon.FirstOrDefaultAsync(id);
        
        await _repositoryCoupon.DeleteAsync(id);
        
        var syncDataModel = new
        {
            EntityName = "pos_coupon",
            Operation = "delete",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = couponInfo
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }
}