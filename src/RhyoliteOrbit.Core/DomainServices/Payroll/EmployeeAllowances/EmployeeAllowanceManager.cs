using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeAllowances;

public class EmployeeAllowanceManager: Abp.Domain.Services.DomainService, IEmployeeAllowanceManager
{
    private readonly IRepository<EmployeeAllowance, Guid> _repositoryEmployeeAllowance;

    public EmployeeAllowanceManager(IRepository<EmployeeAllowance, Guid> repositoryEmployeeAllowance)
    {
        _repositoryEmployeeAllowance = repositoryEmployeeAllowance;
    }

    public async Task<object> Create(EmployeeAllowance entity)
    {
        var data = await _repositoryEmployeeAllowance.FirstOrDefaultAsync(x=> x.EmployeeId == entity.EmployeeId && x.AllowanceTypeId == entity.AllowanceTypeId);
            
        if (data == null)
        {
            await _repositoryEmployeeAllowance.InsertAsync(entity);

            return new
            {
                code = 200,
                message = "successful"
            };

        }

        throw new UserFriendlyException(400,"Employee Allowance","Duplicate records are not allowed.");


    }

    public async Task Update(EmployeeAllowance entity)
    {
        await _repositoryEmployeeAllowance.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryEmployeeAllowance.FirstOrDefaultAsync(id);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<EmployeeAllowance>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.AllowanceTypeName.Contains(query) || x.EmployeeName.Contains(query) || x.EmployeeIdentifier.Contains(query));
        }
            
        var totalCount =
                
            await _repositoryEmployeeAllowance.CountAsync(predicate);
            
        var data = await (await _repositoryEmployeeAllowance.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.IsTaxable, a.EmployeeId, a.EmployeeIdentifier, a.EmployeeName,  a.AllowanceTypeName, a.AllowanceTypeId, a.Amount,  a.AllowanceDays, a.Ssf, a.ProvidentFund   }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        return await _repositoryEmployeeAllowance.GetAllListAsync(x=> x.EmployeeId == employeeId);
    }
    
    public async Task Delete(Guid id)
    {
        await _repositoryEmployeeAllowance.DeleteAsync(id);

    }
}