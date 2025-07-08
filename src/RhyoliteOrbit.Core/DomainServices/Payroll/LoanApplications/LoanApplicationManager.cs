using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.LoanApplications;

public class LoanApplicationManager: Abp.Domain.Services.DomainService, ILoanApplicationManager
{
    private readonly IRepository<LoanApplication, Guid> _repositoryLoanApplication;

    public LoanApplicationManager(IRepository<LoanApplication, Guid> repositoryLoanApplication)
    {
        _repositoryLoanApplication = repositoryLoanApplication;
    }

    public async Task Create(LoanApplication entity)
    {
        await _repositoryLoanApplication.InsertAsync(entity);
    }

    public async Task Update(LoanApplication entity)
    {
        await _repositoryLoanApplication.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryLoanApplication.FirstOrDefaultAsync(id);
    }

    public async Task<object> ListAll()
    {
        var data = await _repositoryLoanApplication.GetAllListAsync();
        return data.OrderByDescending(a => a.CreationTime).ToList();
    }
    
    
    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<LoanApplication>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.LoanTypeName.Contains(query) || x.EmployeeName.Contains(query) || x.EmployeeIdentifier.Contains(query));
        }
            
        var totalCount =
                
            await _repositoryLoanApplication.CountAsync(predicate);
            
        var data = await (await _repositoryLoanApplication.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.EmployeeId, a.EmployeeIdentifier, a.EmployeeName, a.LoanTypeId, a.LoanTypeName, a.ApplyDate, a.ApprovedDate, a.DeductionStarts, a.LoanAmount, a.LoanDuration  }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryLoanApplication.DeleteAsync(id);

    }
}