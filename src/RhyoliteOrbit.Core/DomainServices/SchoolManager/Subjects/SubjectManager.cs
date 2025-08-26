using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.Subjects;

public class SubjectManager: Abp.Domain.Services.DomainService, ISubjectManager
{
    private readonly IRepository<Subject, Guid> _repositorySubject;

    public SubjectManager(IRepository<Subject, Guid> repositorySubject)
    {
        _repositorySubject = repositorySubject;
    }

    public async Task Create(Subject entity)
    {
        var datta = await _repositorySubject.FirstOrDefaultAsync(x => x.Name == entity.Name);
        if (datta == null)
        {
            await _repositorySubject.InsertAsync(entity);

            return;
        }
        
        throw new Abp.UI.UserFriendlyException(
            400,
            "Duplicate records are not allowed.",
            "A Subject with the same name already exists."
        );

    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<Subject>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.Contains(query));
        }
            
        var totalCount =
                
            await _repositorySubject.CountAsync(predicate);
            
        var data = await (await _repositorySubject.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.SubjectCategoryId, a.SubjectCategoryName }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

        var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

        var paginatedResult = new
        {
            pageNo,
            pageSize,
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
        await _repositorySubject.DeleteAsync(id);
    }

    public async Task Update(Subject entity)
    {
        await _repositorySubject.UpdateAsync(entity);
    }
}