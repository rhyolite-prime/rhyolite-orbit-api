using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.AssignedClasses;

public class AssignedClassManager: DomainService, IAssignedClassManager
{
    private readonly IRepository<AssignedClass, Guid> _repositoryAssignedClass;

    public AssignedClassManager(IRepository<AssignedClass, Guid> repositoryAssignedClass)
    {
        _repositoryAssignedClass = repositoryAssignedClass;
    }

    public async Task<object> Create(AssignedClass entity)
    {
        var datta = await _repositoryAssignedClass.FirstOrDefaultAsync(x => x.AcademicYearId == entity.AcademicYearId && x.TermId == entity.TermId && entity.StaffId == x.StaffId);

        if (datta == null)
        {
            await _repositoryAssignedClass.InsertAsync(entity);

            return new
            {
                code = 200,
                message = "successful"
            };
        }

        throw new UserFriendlyException(400,"Duplicate records are not allowed.");
    }


    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<AssignedClass>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.StaffName.Contains(query));

        }
            
        var totalCount =
                
            await _repositoryAssignedClass.CountAsync(predicate);
            
        var data = await (await _repositoryAssignedClass.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.AcademicYearId, a.TermId, a.StaffId, a.AcademicYearName, a.TermName, a.StaffName }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryAssignedClass.DeleteAsync(id);
    }
}