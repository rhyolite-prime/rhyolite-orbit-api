using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.TerminalReports;

public class TerminalReportManager:  Abp.Domain.Services.DomainService, ITerminalReportManager
{
    private readonly IRepository<TerminalReport, Guid> _repositoryTerminalReport;

    public TerminalReportManager(IRepository<TerminalReport, Guid> repositoryTerminalReport)
    {
        _repositoryTerminalReport = repositoryTerminalReport;
    }

    public async Task<object> Create(TerminalReport entity)
    {
        var datta = await _repositoryTerminalReport.FirstOrDefaultAsync(x => x.AcademicYearId == entity.AcademicYearId && x.TermId == entity.TermId && x.ClassId == entity.ClassId);

        if (datta == null)
        {
            await _repositoryTerminalReport.InsertAsync(entity);

            return new
            {
                code = 200,
                message = "successful"
            };
        }

        throw new UserFriendlyException(400, "Duplicate terminal reports are not allowed.");
    }
    
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<TerminalReport>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.StudentIdentifier.Contains(query) || x.StudentName.Contains(query));
        }
            
        var totalCount =
                
            await _repositoryTerminalReport.CountAsync(predicate);
            
        var data = await (await _repositoryTerminalReport.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.AcademicYearName, a.ClassName, a.TermName, a.StudentIdentifier, a.StudentName, a.StudentId, a.PromotedTo, a.TermAverage, a.NumberOnRoll, a.NextTermBegins, a.Position, a.Attendance, a.TotalAttendanceDays, a.ClassTeacher, a.ClassTeacherRemarks, a.SubjectResults }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryTerminalReport.FirstOrDefaultAsync(id);
    }

    public async Task Delete(Guid id)
    {
        await _repositoryTerminalReport.DeleteAsync(id);
    }

    public async Task Update(TerminalReport entity)
    {
        await _repositoryTerminalReport.UpdateAsync(entity);
    }

}