using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.OvertimeTimeSheets;

public class OvertimeTimeSheetManager: Abp.Domain.Services.DomainService, IOvertimeTimeSheetManager
{
    private readonly IRepository<OvertimeTimeSheet, Guid> _repositoryOvertimeTimeSheet;

    public OvertimeTimeSheetManager(IRepository<OvertimeTimeSheet, Guid> repositoryOvertimeTimeSheet)
    {
        _repositoryOvertimeTimeSheet = repositoryOvertimeTimeSheet;
    }

    public async Task Create(OvertimeTimeSheet entity)
    {
        await _repositoryOvertimeTimeSheet.InsertAsync(entity);
    }

    public async Task Update(OvertimeTimeSheet entity)
    {
        await _repositoryOvertimeTimeSheet.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryOvertimeTimeSheet.FirstOrDefaultAsync(id);
    }

    
    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<OvertimeTimeSheet>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.OvertimeTypeName.Contains(query) || x.EmployeeName.Contains(query) || x.EmployeeIdentifier.Contains(query));
        }
            
        var totalCount =
                
            await _repositoryOvertimeTimeSheet.CountAsync(predicate);
            
        var data = await (await _repositoryOvertimeTimeSheet.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.EmployeeId, a.EmployeeIdentifier, a.EmployeeName, a.Month, a.Year, a.JobDescription, a.OvertimeTypeId, a.OvertimeHours,a.OvertimeMinutes,a.IsTaxable,   }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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

    public async Task<object> ListAll(int month,int year)
    {
        return await _repositoryOvertimeTimeSheet.GetAllListAsync(x=> x.Month == month && x.Year == year);
    }
    public async Task Delete(Guid id)
    {
        await _repositoryOvertimeTimeSheet.DeleteAsync(id);
    }
}