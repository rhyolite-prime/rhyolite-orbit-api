using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.PastQuestions;

public class PastQuestionManager: Abp.Domain.Services.DomainService, IPastQuestionManager
{
    private readonly IRepository<PastQuestion, Guid> _repositoryPastQuestion;

    public PastQuestionManager(IRepository<PastQuestion, Guid> repositoryPastQuestion)
    {
        _repositoryPastQuestion = repositoryPastQuestion;
    }


    public async Task<object> Create(PastQuestion entity)
    {
        var datta = await _repositoryPastQuestion.FirstOrDefaultAsync(x => x.Caption == entity.Caption);

        if (datta == null)
        {
            await _repositoryPastQuestion.InsertAsync(entity);

            return new
            {
                code = 200,
                message = "successful"
            };
        }
        
        throw new UserFriendlyException(400, "Duplicate records are not allowed.");

    }

    public async Task Update(PastQuestion entity)
    {
        await _repositoryPastQuestion.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryPastQuestion.GetAsync(id);
    }

    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
         
        var predicate = PredicateBuilder.New<PastQuestion>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Caption.Contains(query));

        }
            
        var totalCount =
                
            await _repositoryPastQuestion.CountAsync(predicate);
            
        var data = await (await _repositoryPastQuestion.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.TermId, a.TermName, a.AcademicYearId, a.AcademicYearName, a.Caption, a.FileUrl }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryPastQuestion.DeleteAsync(id);

    }
}