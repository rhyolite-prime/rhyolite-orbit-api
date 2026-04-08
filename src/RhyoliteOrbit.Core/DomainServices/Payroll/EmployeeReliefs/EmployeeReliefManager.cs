using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeReliefs;

public class EmployeeReliefManager: Abp.Domain.Services.DomainService, IEmployeeReliefManager
{
    private readonly IRepository<EmployeeRelief, Guid> _repositoryEmployeeRelief;

    public EmployeeReliefManager(IRepository<EmployeeRelief, Guid> repositoryEmployeeRelief)
    {
        _repositoryEmployeeRelief = repositoryEmployeeRelief;
    }

    public async Task Create(EmployeeRelief entity)
    {
        var data = await _repositoryEmployeeRelief.FirstOrDefaultAsync(x=> x.EmployeeId == entity.EmployeeId && x.ReliefTypeId == entity.ReliefTypeId);
            
        if (data == null)
        {
            await _repositoryEmployeeRelief.InsertAsync(entity);

            return;
        }
        
        throw new UserFriendlyException(
            400,
            "Duplicate Employee Relief record.",
            "An Employee Relief with the same EmployeeId and ReliefTypeId already exists."
        );


    }

    public async Task Update(EmployeeRelief entity)
    {
        await _repositoryEmployeeRelief.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryEmployeeRelief.FirstOrDefaultAsync(id);
    }

    
    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<EmployeeRelief>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.ReliefTypeName.Contains(query) || x.EmployeeName.Contains(query) || x.EmployeeIdentifier.Contains(query));
        }
            
        var totalCount =
                
            await _repositoryEmployeeRelief.CountAsync(predicate);
            
        var data = await (await _repositoryEmployeeRelief.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.EmployeeId, a.EmployeeIdentifier, a.EmployeeName, a.Amount, a.ReliefTypeId, a.ReliefTypeName   }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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

    public async Task<object> ListAll(Guid employeeId)
    {
        return await _repositoryEmployeeRelief.GetAllListAsync(x=>x.EmployeeId == employeeId);
    }

    public async Task Delete(Guid id)
    {
        await _repositoryEmployeeRelief.DeleteAsync(id);

    }
}