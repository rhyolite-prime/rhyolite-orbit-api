using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.SubjectRemarks;

public class SubjectRemarkManager: Abp.Domain.Services.DomainService, ISubjectRemarkManager
{
    private readonly IRepository<SubjectRemark, Guid> _repositorySubjectRemark;

    public SubjectRemarkManager(IRepository<SubjectRemark, Guid> repositorySubjectRemark)
    {
        _repositorySubjectRemark = repositorySubjectRemark;
    }

    public async Task Create(SubjectRemark entity)
    {
        await _repositorySubjectRemark.InsertAsync(entity);

    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<SubjectRemark>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Remarks.Contains(query));
        }
            
        var totalCount =
                
            await _repositorySubjectRemark.CountAsync(predicate);
            
        var data = await (await _repositorySubjectRemark.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Remarks, a.MaximumMarks, a.MinimumMarks,  }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositorySubjectRemark.DeleteAsync(id);
    }

    public async Task Update(SubjectRemark entity)
    {
        await _repositorySubjectRemark.UpdateAsync(entity);
    }
}