using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Akka.Pattern;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.Religions;

public class ReligionManager: Abp.Domain.Services.DomainService, IReligionManager
{
    private readonly IRepository<Religion, Guid> _repositoryReligion;

    public ReligionManager(IRepository<Religion, Guid> repositoryReligion)
    {
        _repositoryReligion = repositoryReligion;
    }

    public async Task Create(Religion entity)
    {
        var datta = await _repositoryReligion.FirstOrDefaultAsync(x => x.Name.Trim().ToLower() == entity.Name.Trim().ToLower());

        if (datta == null)
        {
            await _repositoryReligion.InsertAsync(entity);
            return;
        }
        
        throw new UserFriendlyException(
            400,
            "Duplicate records are not allowed.",
            "A duplicate religion with the same name already exists."
        );

    }

    public async Task Update(Religion entity)
    {
        await _repositoryReligion.UpdateAsync(entity);
    }

    public async Task<Religion> GetAsync(string name)
    {
        return await _repositoryReligion.FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<Religion>();
        
        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(a => a.Name.Contains(query));
        }
        
        var totalCount =
            await _repositoryReligion.CountAsync(predicate);
            
        var data = await (await _repositoryReligion.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.CreationTime, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryReligion.DeleteAsync(id);

    }
}