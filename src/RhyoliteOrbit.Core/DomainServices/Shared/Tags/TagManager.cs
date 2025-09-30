using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.Tags;

public class TagManager: DomainService, ITagManager 
{
    private readonly IRepository<Tag, Guid> _repositoryTag;

    public TagManager(IRepository<Tag, Guid> repositoryReligion)
    {
        _repositoryTag = repositoryReligion;
    }

    public async Task Create(Tag entity)
    {
        var datta = await _repositoryTag.FirstOrDefaultAsync(x => x.Name.Trim().ToLower() == entity.Name.Trim().ToLower());

        if (datta == null)
        {
            await _repositoryTag.InsertAsync(entity);
            return;
        }
        
        throw new UserFriendlyException(
            400,
            "Duplicate records are not allowed.",
            "A duplicate tag with the same name already exists."
        );

    }

    public async Task Update(Tag entity)
    {
        await _repositoryTag.UpdateAsync(entity);
    }

    
    public async Task<object> ListAll(int pageNo, int pageSize, RhyoliteOrbitConsts.ErpModule erpModule, string query)
    {
        var predicate = PredicateBuilder.New<Tag>();
        
        predicate.And(x => !x.IsDeleted);
        
        predicate.And(x => x.ErpModule == (int)erpModule);
        
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(a => a.Name.Contains(query));
        }
        
        
        var totalCount =
            await _repositoryTag.CountAsync(predicate);
            
        var data = await (await _repositoryTag.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.ErpModule, a.CreationTime, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryTag.DeleteAsync(id);

    }
}