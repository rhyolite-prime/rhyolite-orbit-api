using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.ClassStreams;

public class ClassStreamManager: Abp.Domain.Services.DomainService, IClassStreamManager
{
    private readonly IRepository<ClassStream, Guid> _repositoryClassStream;

    public ClassStreamManager(IRepository<ClassStream, Guid> repositoryClassStream)
    {
        _repositoryClassStream = repositoryClassStream;
    }

    public async Task Create(ClassStream entity)
    {
        var datta = await _repositoryClassStream.FirstOrDefaultAsync(x => x.Name == entity.Name);
        if (datta == null)
        {
            await _repositoryClassStream.InsertAsync(entity);
            return;
        }

        throw new UserFriendlyException(400, "Duplicate records are not allowed.");

    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<ClassStream>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.ToLower().Contains(query.ToLower()));
        }
            
        var totalCount =
                
            await _repositoryClassStream.CountAsync(predicate);
            
        var data = await (await _repositoryClassStream.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name,}).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryClassStream.DeleteAsync(id);
    }

    public async Task Update(ClassStream entity)
    {
        await _repositoryClassStream.UpdateAsync(entity);
    }
}