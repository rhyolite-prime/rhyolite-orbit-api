using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace RhyoliteOrbit.DomainServices.SchoolManager.SubjectCategories;

public class SubjectCategoryManager: DomainService, ISubjectCategoryManager
{
    private readonly IRepository<SubjectCategory, Guid> _repositorySubjectCategory;

    public SubjectCategoryManager(IRepository<SubjectCategory, Guid> repositorySubject)
    {
        _repositorySubjectCategory = repositorySubject;
    }

    public async Task Create(SubjectCategory entity)
    {
        var datta = await _repositorySubjectCategory.FirstOrDefaultAsync(x => x.Name == entity.Name);
        if (datta == null)
        {
            await _repositorySubjectCategory.InsertAsync(entity);

            return;
        }
        
        throw new Abp.UI.UserFriendlyException(
            400,
            "Duplicate records are not allowed.",
            "A Subject Category with the same name already exists."
        );

    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<SubjectCategory>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.Contains(query));
        }
            
        var totalCount =
                
            await _repositorySubjectCategory.CountAsync(predicate);
            
        var data = await (await _repositorySubjectCategory.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.Description }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositorySubjectCategory.DeleteAsync(id);
    }

    public async Task Update(SubjectCategory entity)
    {
        await _repositorySubjectCategory.UpdateAsync(entity);
    }
}