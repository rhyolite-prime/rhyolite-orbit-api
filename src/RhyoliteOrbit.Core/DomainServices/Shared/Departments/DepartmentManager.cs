using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.Departments;

public class DepartmentManager: Abp.Domain.Services.DomainService, IDepartmentManager
{
    private readonly IRepository<Department, Guid> _repositoryDepartment;

    public DepartmentManager(IRepository<Department, Guid> repositoryDepartment)
    {
        _repositoryDepartment = repositoryDepartment;
    }

    public async Task Create(Department entity)
    {
        var datta = await _repositoryDepartment.FirstOrDefaultAsync(x => x.Name == entity.Name);

        if (datta == null)
        {
            await _repositoryDepartment.InsertAsync(entity);

        }
        
        throw new UserFriendlyException(400, "Duplicate records are not allowed.");
    }
    
    public async Task<Department> GetAsync(string name)
    {
        return await _repositoryDepartment.FirstOrDefaultAsync(x => x.Name == name);
    }
    
    public async Task Update(Department entity)
    {
        await _repositoryDepartment.UpdateAsync(entity);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<Department>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(a => a.Name.Contains(query));

        }
            
        var totalCount =
                
            await _repositoryDepartment.CountAsync(predicate);
            
        var data = await (await _repositoryDepartment.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.CreationTime, }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryDepartment.DeleteAsync(id);

    }
}