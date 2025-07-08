using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeBenefitInKinds;

public class EmployeeBenefitInKindManager : Abp.Domain.Services.DomainService, IEmployeeBenefitInKindManager
{
    private readonly IRepository<EmployeeBenefitInKind, Guid> _repositoryEmployeeBenefitInKind;

    public EmployeeBenefitInKindManager(IRepository<EmployeeBenefitInKind, Guid> repositoryEmployeeBenefitInKind)
    {
        _repositoryEmployeeBenefitInKind = repositoryEmployeeBenefitInKind;
    }

    public async Task<object> Create(EmployeeBenefitInKind entity)
    {
        var data = await _repositoryEmployeeBenefitInKind.FirstOrDefaultAsync(x=> x.EmployeeId == entity.EmployeeId && x.BenefitInKindTypeId == entity.BenefitInKindTypeId);
        if (data == null)
        {
            await _repositoryEmployeeBenefitInKind.InsertAsync(entity);

            return new
            {
                code = 200,
                message = "successful"
            };
        }
        
        throw new UserFriendlyException(
            400,
            "Duplicate Employee Benefit In Kind record.",
            "An Employee Benefit In Kind with the same EmployeeId and BenefitInKindTypeId already exists."
        );
        
    }

    public async Task Update(EmployeeBenefitInKind entity)
    {
        await _repositoryEmployeeBenefitInKind.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryEmployeeBenefitInKind.FirstOrDefaultAsync(id);
    }

    
    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<EmployeeBenefitInKind>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.BenefitInKindTypeName.Contains(query) || x.EmployeeName.Contains(query) || x.EmployeeIdentifier.Contains(query));
        }
            
        var totalCount =
                
            await _repositoryEmployeeBenefitInKind.CountAsync(predicate);
            
        var data = await (await _repositoryEmployeeBenefitInKind.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.EmployeeId, a.EmployeeIdentifier, a.EmployeeName, a.Amount, a.BenefitInKindTypeId, a.BenefitInKindTypeName   }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        return await _repositoryEmployeeBenefitInKind.GetAllListAsync(x=> x.EmployeeId == employeeId);
    }
    
    public async Task Delete(Guid id)
    {
        await _repositoryEmployeeBenefitInKind.DeleteAsync(id);

    }
}