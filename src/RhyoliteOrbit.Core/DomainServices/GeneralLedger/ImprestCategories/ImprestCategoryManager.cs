using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.GeneralLedger;

namespace RhyoliteOrbit.DomainServices.GeneralLedger.ImprestCategories;

public class ImprestCategoryManager: DomainService, IImprestCategoryManager
{
    private readonly IRepository<ImprestCategory, Guid> _repositoryImprestCategory;

    public ImprestCategoryManager(IRepository<ImprestCategory, Guid> repositoryImprestCategory)
    {
        _repositoryImprestCategory = repositoryImprestCategory;
    }

    public async Task Create(ImprestCategory entity)
    {
        var datta = await _repositoryImprestCategory.FirstOrDefaultAsync(x => x.Name == entity.Name);

        if (datta == null)
        {
            await _repositoryImprestCategory.InsertAsync(entity);

            return;
        }
        
        throw new UserFriendlyException(
            400,
            "Duplicate records are not allowed.",
            "An Imprest Category with the same name already exists."
        );
        
    }

    public async Task Update(ImprestCategory entity)
    {
        await _repositoryImprestCategory.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryImprestCategory.FirstOrDefaultAsync(id);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<ImprestCategory>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.ToLower().Contains(query));
        }
            
        var totalCount =
                
            await _repositoryImprestCategory.CountAsync(predicate);
            
        var data = await (await _repositoryImprestCategory.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryImprestCategory.DeleteAsync(id);

    }
}