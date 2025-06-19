using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeOnetimeDeductions;

public class EmployeeOnetimeDeductionManager: Abp.Domain.Services.DomainService, IEmployeeOnetimeDeductionManager
{
    private readonly IRepository<EmployeeOnetimeDeduction, Guid> _repositoryEmployeeOnetimeDeduction;

    public EmployeeOnetimeDeductionManager(IRepository<EmployeeOnetimeDeduction, Guid> repositoryEmployeeOnetimeDeductionAllowanceType)
    {
        _repositoryEmployeeOnetimeDeduction = repositoryEmployeeOnetimeDeductionAllowanceType;
    }

    public async Task Create(EmployeeOnetimeDeduction entity)
    {
        await _repositoryEmployeeOnetimeDeduction.InsertAsync(entity);
    }

    public async Task Update(EmployeeOnetimeDeduction entity)
    {
        await _repositoryEmployeeOnetimeDeduction.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryEmployeeOnetimeDeduction.FirstOrDefaultAsync(id);
    }

    
    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<EmployeeOnetimeDeduction>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.DeductionTypeName.Contains(query) || x.EmployeeName.Contains(query) || x.EmployeeIdentifier.Contains(query));
        }
            
        var totalCount =
                
            await _repositoryEmployeeOnetimeDeduction.CountAsync(predicate);
            
        var data = await (await _repositoryEmployeeOnetimeDeduction.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.EmployeeId, a.EmployeeIdentifier, a.EmployeeName, a.Amount, a.IsFixedAmount, a.Percentage, a.Month, a.Year, a.DeductionTypeId, a.DeductionTypeName   }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        return await _repositoryEmployeeOnetimeDeduction.GetAllListAsync(x=> x.EmployeeId == employeeId);
    }

    public async Task<object> ListAll(int month,int year)
    {
        return await _repositoryEmployeeOnetimeDeduction.GetAllListAsync(x => x.Month == month && x.Year == year);
    }

    public async Task Delete(Guid id)
    {
        await _repositoryEmployeeOnetimeDeduction.DeleteAsync(id);
    }
}