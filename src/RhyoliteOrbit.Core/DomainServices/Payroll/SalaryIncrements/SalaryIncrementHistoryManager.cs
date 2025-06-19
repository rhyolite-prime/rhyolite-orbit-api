using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.SalaryIncrements;

public class SalaryIncrementHistoryManager: Abp.Domain.Services.DomainService, ISalaryIncrementHistoryManager
{
    private readonly IRepository<Models.Payroll.SalaryIncrementHistory, Guid> _repositorySalaryIncrementHistory;

    public SalaryIncrementHistoryManager(IRepository<Models.Payroll.SalaryIncrementHistory, Guid> repositorySalaryIncrementHistory)
    {
        _repositorySalaryIncrementHistory = repositorySalaryIncrementHistory;
    }

    public async Task Create(Models.Payroll.SalaryIncrementHistory entity)
    {
        await _repositorySalaryIncrementHistory.InsertAsync(entity);
    }

    public async Task<object> ListAll()
    {
        var data = await _repositorySalaryIncrementHistory.GetAllListAsync();
        return data.OrderBy(a => a.EmployeeName).ToList();
    }
    
    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<SalaryIncrementHistory>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.EmployeeName.Contains(query) || x.EmployeeIdentifier.Contains(query));
        }
            
        var totalCount =
                
            await _repositorySalaryIncrementHistory.CountAsync(predicate);
            
        var data = await (await _repositorySalaryIncrementHistory.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.EmployeeId, a.EmployeeIdentifier, a.EmployeeName, a.CurrentSalary, a.PreviousSalary, a.IncrementAmount  }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
    
}