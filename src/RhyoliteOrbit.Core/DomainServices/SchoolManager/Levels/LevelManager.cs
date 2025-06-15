using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.Levels;

public class LevelManager: Abp.Domain.Services.DomainService, ILevelManager
{
    private readonly IRepository<Level, Guid> _repositoryLevel;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    public LevelManager(IRepository<Level, Guid> repositoryLevel, IUnitOfWorkManager unitOfWorkManager)
    {
        _repositoryLevel = repositoryLevel;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task<object> Create(Level entity)
    {
        var datta = await _repositoryLevel.FirstOrDefaultAsync(x => x.Name == entity.Name);
        
        if (datta == null)
        {
            await _repositoryLevel.InsertAsync(entity);

            return new
            {
                code = 200,
                message = "successful"
            };
        }

        throw new UserFriendlyException(400, "Duplicate records are not allowed.");
        
    }

    public async Task CreateBulk(List<Level> inputs, int tenantId)
    {

        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            foreach (var entity in inputs)
            {
                entity.TenantId = tenantId;
                await _repositoryLevel.InsertAsync(entity);

            }

        }

    }


    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<Level>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.Contains(query));

        }
            
        var totalCount =
                
            await _repositoryLevel.CountAsync(predicate);
            
        var data = await (await _repositoryLevel.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryLevel.DeleteAsync(id);

    }

    public async Task Update(Level entity)
    {
        await _repositoryLevel.UpdateAsync(entity);
    }
}