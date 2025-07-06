using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeDeductions;

public class EmployeeDeductionManager: Abp.Domain.Services.DomainService, IEmployeeDeductionManager
{
    private readonly IRepository<EmployeeDeduction, Guid> _repositoryEmployeeDeduction;

    public EmployeeDeductionManager(IRepository<EmployeeDeduction, Guid> repositoryEmployeeDeduction)
    {
        _repositoryEmployeeDeduction = repositoryEmployeeDeduction;
    }

    public async Task<object> Create(EmployeeDeduction entity)
    {
        var data = await _repositoryEmployeeDeduction.FirstOrDefaultAsync(x=>x.EmployeeId == entity.EmployeeId && x.DeductionTypeId == entity.DeductionTypeId);

        if (data == null)
        {
            await _repositoryEmployeeDeduction.InsertAsync(entity);
            return new
            {
                code = 200,
                message = "successful"
            };
        }

        throw new UserFriendlyException(
            400,
            "Duplicate Employee Deduction record.",
            "An Employee Deduction with the same EmployeeId and DeductionTypeId already exists."
        );

    }

    public async Task Update(EmployeeDeduction entity)
    {
        await _repositoryEmployeeDeduction.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryEmployeeDeduction.FirstOrDefaultAsync(id);
    }

    
    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<EmployeeDeduction>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.DeductionTypeName.Contains(query) || x.EmployeeName.Contains(query) || x.EmployeeIdentifier.Contains(query));
        }
            
        var totalCount =
                
            await _repositoryEmployeeDeduction.CountAsync(predicate);
            
        var data = await (await _repositoryEmployeeDeduction.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.EmployeeId, a.EmployeeIdentifier, a.Amount, a.EmployerAmount }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        return await _repositoryEmployeeDeduction.GetAllListAsync(x=>x.EmployeeId == employeeId);
    }

    public async Task Delete(Guid id)
    {
        await _repositoryEmployeeDeduction.DeleteAsync(id);

    }
}