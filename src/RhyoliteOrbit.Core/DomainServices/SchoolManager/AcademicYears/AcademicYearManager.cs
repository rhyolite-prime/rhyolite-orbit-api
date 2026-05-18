using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.SchoolManager.AcademicYears.Dto;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.AcademicYears;

public class AcademicYearManager : Abp.Domain.Services.DomainService, IAcademicYearManager
{
    private readonly IRepository<AcademicYear, Guid> _repositoryAcademicYear;

    public AcademicYearManager(IRepository<AcademicYear, Guid> repositoryAcademicYear)
    {
        _repositoryAcademicYear = repositoryAcademicYear;
    }

    public async Task Create(AcademicYear entity)
    {
        var datta = await _repositoryAcademicYear.FirstOrDefaultAsync(x => x.Name == entity.Name);

        if (datta == null)
        {
            var maxPosition = await (await _repositoryAcademicYear.GetAllAsync()).MaxAsync(x => (int?)x.Position) ?? 0;
            entity.Position = maxPosition + 1;

            await _repositoryAcademicYear.InsertAsync(entity);
            return;
        }

        throw new UserFriendlyException(
            400,
            "Duplicate records are not allowed.",
            "An Academic Year with the same name already exists."
        );

    }

    public async Task CreateTerm(TermDto input)
    {
        var academicYearData = await _repositoryAcademicYear.FirstOrDefaultAsync(input.AcademicYearId);
        if (academicYearData.Terms == null)
        {
            academicYearData.Terms = new List<Term>();
        }
        var termList = academicYearData.Terms;
        var termInfo = termList.FirstOrDefault(x => x.Id == input.Id);

        if (termInfo == null)
        {
            int newPosition = input.Position > 0
                ? input.Position
                : (academicYearData.Terms.Any() ? academicYearData.Terms.Max(t => t.Position) + 1 : 1);

            academicYearData.Terms.Add(new Term
            {
                Id = Guid.NewGuid(),
                Name = input.Name,
                Position = newPosition,
                NoOfDays = input.NoOfDays,
                StartDate = input.StartDate,
                EndDate = input.EndDate

            });

            await _repositoryAcademicYear.UpdateAsync(academicYearData);

            return;
        }

        throw new UserFriendlyException(
            400,
            "Duplicate records are not allowed.",
            "A Term with the same reference already exists."
        );


    }

    public async Task Update(AcademicYear entity)
    {
        var academicYearData = await _repositoryAcademicYear.GetAsync(entity.Id);

        if (academicYearData == null)
        {
            throw new UserFriendlyException(
                400,
                "Academic Year Not Found",
                "The specified Academic Year could not be found. Please verify the provided ID and try again."
            );

        }

        academicYearData.BeginDate = entity.BeginDate;
        academicYearData.EndDate = entity.EndDate;
        academicYearData.Name = entity.Name;
        academicYearData.Position = entity.Position;


        await _repositoryAcademicYear.UpdateAsync(academicYearData);
    }

    public async Task UpdateTerm(TermDto input)
    {
        var academicYearData = await _repositoryAcademicYear.FirstOrDefaultAsync(input.AcademicYearId);
        var termList = academicYearData.Terms;
        var termInfo = termList.FirstOrDefault(x => x.Id == input.Id);
        termList.Remove(termInfo);

        termInfo.Name = input.Name;
        termInfo.StartDate = input.StartDate;
        termInfo.EndDate = input.EndDate;
        termInfo.NoOfDays = input.NoOfDays;
        termInfo.Position = input.Position;


        termList.Add(termInfo);

        academicYearData.Terms = termList;

        await _repositoryAcademicYear.UpdateAsync(academicYearData);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryAcademicYear.GetAsync(id);
    }


    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<AcademicYear>();

        predicate.And(x => !x.IsDeleted);

        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.Contains(query));
        }

        var totalCount =

            await _repositoryAcademicYear.CountAsync(predicate);

        var data = await (await _repositoryAcademicYear.GetAllAsync()).Where(predicate).Select(a => new { a.Id, a.Name, a.BeginDate, a.EndDate, a.Position, a.Terms }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryAcademicYear.DeleteAsync(id);
    }

    public async Task DeleteTerm(Guid academicYearId, Guid id)
    {
        var academicYearData = await _repositoryAcademicYear.FirstOrDefaultAsync(academicYearId);
        var termList = academicYearData.Terms;
        var termInfo = termList.FirstOrDefault(x => x.Id == id);
        termList.Remove(termInfo);

        academicYearData.Terms = termList;

        await _repositoryAcademicYear.UpdateAsync(academicYearData);

    }
}