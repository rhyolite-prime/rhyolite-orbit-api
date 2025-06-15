using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.Shared.MetaData;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.GradingSchemes;

public class GradingSchemeManager: Abp.Domain.Services.DomainService, IGradingSchemeManager
{
    private readonly IRepository<GradingScheme, Guid> _repositoryGradingScheme;
    private readonly IAcademicPeriodMetaDataManager _academicPeriodMetaDataManager;
    
    public GradingSchemeManager(IRepository<GradingScheme, Guid> repositoryGradingScheme, IAcademicPeriodMetaDataManager academicPeriodMetaDataManager)
    {
        _repositoryGradingScheme = repositoryGradingScheme;
        _academicPeriodMetaDataManager = academicPeriodMetaDataManager;
    }

    public async Task<object> ListAll(Guid academicYearId, Guid termId, Guid levelId)
    {
        return await _repositoryGradingScheme.GetAllListAsync(x=>x.AcademicYearId == academicYearId && x.TermId == termId && x.LevelId == levelId);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, Guid academicYearId, Guid termId, Guid levelId)
    {
        var predicate = PredicateBuilder.New<GradingScheme>();

        predicate.And(x => !x.IsDeleted);
            
        if (academicYearId != Guid.Empty)
        {
            predicate.And(x => x.AcademicYearId == academicYearId);
        }
         
        if (termId != Guid.Empty)
        {
            predicate.And(x => x.TermId == termId);
        }
        if (levelId != Guid.Empty)
        {
            predicate.And(x => x.LevelId == levelId);
        }
            
        var totalCount =
                
            await _repositoryGradingScheme.CountAsync(predicate);
            
        var data = await (await _repositoryGradingScheme.GetAllAsync()).Where(predicate).Select(a=> new { a.Id,  }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
    public async Task Create(GradingScheme entity)
    {
        var basicInfo = await _academicPeriodMetaDataManager.GetAcademicPeriodMeta(entity.AcademicYearId, entity.TermId);
        entity.AcademicYearName = basicInfo.AcademicYearName;
        entity.TermName = basicInfo.TermName;
        await _repositoryGradingScheme.InsertAsync(entity);
    }

    public async Task Update(GradingScheme entity)
    {
        var basicInfo = await _academicPeriodMetaDataManager.GetAcademicPeriodMeta(entity.AcademicYearId, entity.TermId);
        entity.AcademicYearName = basicInfo.AcademicYearName;
        entity.TermName = basicInfo.TermName;
        await _repositoryGradingScheme.UpdateAsync(entity);

    }

    public async Task Delete(Guid id)
    {
        await _repositoryGradingScheme.DeleteAsync(id);
    }
}