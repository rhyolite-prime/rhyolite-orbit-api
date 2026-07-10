using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.CostCenters;

public class CostCenterManager: Abp.Domain.Services.DomainService, ICostCenterManager
{
    private readonly IRepository<CostCenter, Guid> _repositoryCostCenter;

    public CostCenterManager(IRepository<CostCenter, Guid> repositoryCostCenter)
    {
        _repositoryCostCenter = repositoryCostCenter;
    }

    public async Task Create(CostCenter entity)
    {
        var datta = await _repositoryCostCenter.FirstOrDefaultAsync(x => x.Name == entity.Name && x.ParentId == entity.ParentId);

        if (datta == null)
        {
            await _repositoryCostCenter.InsertAsync(entity);
            return;
        }
        
        throw new UserFriendlyException(400, "Duplicate records are not allowed.");
    }
    
    public async Task Update(CostCenter entity)
    {
        await _repositoryCostCenter.UpdateAsync(entity);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<CostCenter>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(a => a.Name.Contains(query));

        }
            
        var totalCount =
                
            await _repositoryCostCenter.CountAsync(predicate);
            
        var data = await (await _repositoryCostCenter.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.ParentId, a.CreationTime, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
    public async Task Delete(Guid id)
    {
        await _repositoryCostCenter.DeleteAsync(id);

    }
}