using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Akka.Actor;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Actors;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.Branches;

public class PosBranchManager : DomainService, IPosBranchManager
{
    private readonly IRepository<PosBranch, Guid> _repositoryBranch;
    private readonly IActorRef _dataSupportEntitySyncActorProvider;
    
    public PosBranchManager(IRepository<PosBranch, Guid> repositoryBranch, TopLevelActors.DataSupportEntitySyncActorProvider dataSupportEntitySyncActorProvider)
    {
        _repositoryBranch = repositoryBranch;
        _dataSupportEntitySyncActorProvider = dataSupportEntitySyncActorProvider();
    }

    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<PosBranch>();

        predicate.And(x => !x.IsDeleted);
        
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.Contains(query) || x.BranchManager.Contains(query) || x.PhoneNo.Contains(query));

        }
        
        var totalCount =
            
            await _repositoryBranch.CountAsync(predicate);
        
        var data = await (await _repositoryBranch.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.CountryCode, a.CountryName, a.Address, a.City, a.Location, a.RegionOrState, a.PhoneNo, a.BranchManager }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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


    public async Task Create(PosBranch entity, string dataSupportBusinessAccountId)
    {
        var datta = await _repositoryBranch.FirstOrDefaultAsync(x => x.Name == entity.Name);
        if (datta != null)
        {
            throw new UserFriendlyException(400, "Duplicate records are not allowed.");
        }
        
        await _repositoryBranch.InsertAsync(entity);
        
        // sync
        var syncDataModel = new
        {
            EntityName = "pos_branch",
            Operation = "create",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }

    public async Task Update(PosBranch entity, string dataSupportBusinessAccountId)
    {
        await _repositoryBranch.UpdateAsync(entity);
        
        var syncDataModel = new
        {
            EntityName = "pos_branch",
            Operation = "update",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = entity
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }
        
    public async Task SetDefaultBranch(Guid id)
    {
        var branchInfo = await _repositoryBranch.FirstOrDefaultAsync(id);

        if (branchInfo != null)
        {
            branchInfo.IsDefault = true;
        }
        
        //set the rest to false
         var branches = await _repositoryBranch.GetAllListAsync();

         if (branches.Any())
         {
             foreach (var branch in branches)
             {
                 branch.IsDefault = false;
                 await _repositoryBranch.UpdateAsync(branch);
             }
         }


    }

    public async Task Delete(Guid id, string dataSupportBusinessAccountId)
    {
        var branchInfo = await _repositoryBranch.FirstOrDefaultAsync(id);
        
        await _repositoryBranch.DeleteAsync(id);
        
        var syncDataModel = new
        {
            EntityName = "pos_branch",
            Operation = "delete",
            BusinessAccountId = dataSupportBusinessAccountId,
            Data = branchInfo
        };
        
        _dataSupportEntitySyncActorProvider.Tell(syncDataModel);
    }
}