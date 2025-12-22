using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.GeneralLedger;

namespace RhyoliteOrbit.DomainServices.GeneralLedger.CoaControls;

public class CoaControlManager: DomainService, ICoaControlManager
{
    private readonly IRepository<CoaControl, Guid> _repositoryCoaControl;

    public CoaControlManager(IRepository<CoaControl, Guid> repositoryCoaControl)
    {
        _repositoryCoaControl = repositoryCoaControl;
    }

    public async Task Create(CoaControl entity)
    {
        var coaControl =
            await _repositoryCoaControl.FirstOrDefaultAsync(x => x.Id == entity.AccountHeadId && x.MinAccount == entity.MinAccount && x.MaxAccount == entity.MaxAccount);

        if (coaControl == null)
        {
            //use recursion to construct the parent sub group hierachy in the format : parent/child/child1/child2/child3
            // this is needed to set the entity.ParentSubGroupName
            
            entity.ParentSubGroupName = await BuildParentHierarchy(entity.ParentSubGroupId);
            
            await _repositoryCoaControl.InsertAsync(entity);
            return;
        }
        
        throw new UserFriendlyException(
            400,
            "Duplicate records are not allowed.",
            "An Account Head with the same name already exists."
        );

         
    }

    public async Task Update(CoaControl entity)
    {
        await _repositoryCoaControl.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryCoaControl.FirstOrDefaultAsync(id);
    }

    public async Task<object> ListAll(Guid id)
    {
        return await _repositoryCoaControl.GetAllListAsync(x=> x.AccountHeadId == id);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query , Guid accountHeadId)
    {
        var predicate = PredicateBuilder.New<CoaControl>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.SubGroupName.ToLower().Contains(query.ToLower()));
        }

        if (accountHeadId != Guid.Empty)
        {
            predicate.And(x => x.AccountHeadId == accountHeadId);
        }
        
        var totalCount =
                
            await _repositoryCoaControl.CountAsync(predicate);
            
        var data = await (await _repositoryCoaControl.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.SubGroupName, a.MinAccount, a.MaxAccount, a.AccountHeadId, a.ParentSubGroupId, a.ParentSubGroupName }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

        var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

        var paginatedResult = new
        {
            pageNo,
            pageSize,
            totalCount,
            totalPages,
            data,
            lowerBound = pageSize * (pageNo - 1) + 1,
            upperBound = totalPages == pageNo ? totalCount : pageNo * pageSize,
        };

        return paginatedResult;
    }

    public async Task Delete(Guid id)
    {
        await _repositoryCoaControl.DeleteAsync(id);

    }

    private async Task<string> BuildParentHierarchy(Guid? id)
    {
        if (id == null || id == Guid.Empty)
            return string.Empty;

        var parent = await _repositoryCoaControl.FirstOrDefaultAsync(id.Value);

        if (parent == null)
            return string.Empty;

        var parentHierarchy = await BuildParentHierarchy(parent.ParentSubGroupId);

        return string.IsNullOrWhiteSpace(parentHierarchy)
            ? parent.SubGroupName
            : $"{parentHierarchy}/{parent.SubGroupName}";
    }
}