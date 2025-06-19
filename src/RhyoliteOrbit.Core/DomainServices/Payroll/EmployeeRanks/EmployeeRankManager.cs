using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeRanks;

public class EmployeeRankManager: Abp.Domain.Services.DomainService, IEmployeeRankManager
{
    private readonly IRepository<EmployeeRank, Guid> _repositoryEmployeeRank;

    public EmployeeRankManager(IRepository<EmployeeRank, Guid> repositoryEmployeeRank)
    {
        _repositoryEmployeeRank = repositoryEmployeeRank;
    }

    public async Task<object> Create(EmployeeRank entity)
    {
        var datta = await _repositoryEmployeeRank.FirstOrDefaultAsync(x => x.Name.Trim().ToLower() == entity.Name.Trim().ToLower());

        if (datta == null)
        {
            await _repositoryEmployeeRank.InsertAsync(entity);

            return new
            {
                code = 200,
                message = "successful"
            };
        }
        
        throw new Abp.UI.UserFriendlyException(
            400,
            "Duplicate records are not allowed.",
            "An Employee Rank with the same Name already exists."
        );

    }

    public async Task Update(EmployeeRank entity)
    {
        await _repositoryEmployeeRank.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryEmployeeRank.FirstOrDefaultAsync(id);
    }

    
    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<EmployeeRank>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.Contains(query));
        }
            
        var totalCount =
                
            await _repositoryEmployeeRank.CountAsync(predicate);
            
        var data = await (await _repositoryEmployeeRank.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryEmployeeRank.DeleteAsync(id);

    }
}