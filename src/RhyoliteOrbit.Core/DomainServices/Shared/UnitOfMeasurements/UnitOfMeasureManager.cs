using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.UnitOfMeasurements;

public class UnitOfMeasureManager: DomainService, IUnitOfMeasureManager
{
    private readonly IRepository<UnitOfMeasure, Guid> _repositoryUnitOfMeasure;

    public UnitOfMeasureManager(IRepository<UnitOfMeasure, Guid> repositoryUnitOfMeasure)
    {
        _repositoryUnitOfMeasure = repositoryUnitOfMeasure;
    }

    public async Task Create(UnitOfMeasure entity)
    {
        var datta = await _repositoryUnitOfMeasure.FirstOrDefaultAsync(x => x.Name.Trim().ToLower() == entity.Name.Trim().ToLower());

        if (datta == null)
        {
            await _repositoryUnitOfMeasure.InsertAsync(entity);
            return;
        }
        
        throw new UserFriendlyException(
            400,
            "Duplicate records are not allowed.",
            "A duplicate religion with the same name already exists."
        );

    }

    public async Task Update(UnitOfMeasure entity)
    {
        await _repositoryUnitOfMeasure.UpdateAsync(entity);
    }

    

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<UnitOfMeasure>();
        
        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(a => a.Name.ToLower().Contains(query.ToLower()));
        }
        
        var totalCount =
            await _repositoryUnitOfMeasure.CountAsync(predicate);
            
        var data = await (await _repositoryUnitOfMeasure.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.Description, a.CreationTime, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryUnitOfMeasure.DeleteAsync(id);

    }
}