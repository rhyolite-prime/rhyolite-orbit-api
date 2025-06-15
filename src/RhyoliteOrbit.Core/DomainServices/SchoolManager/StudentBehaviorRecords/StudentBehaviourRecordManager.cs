using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.StudentBehaviorRecords;

public class StudentBehaviourRecordManager: Abp.Domain.Services.DomainService, IStudentBehaviourRecordManager
{
    private readonly IRepository<StudentBehaviourRecord, Guid> _repositoryStudentBehaviourRecord;

    public StudentBehaviourRecordManager(IRepository<StudentBehaviourRecord, Guid> repositoryStudentBehaviourRecord)
    {
        _repositoryStudentBehaviourRecord = repositoryStudentBehaviourRecord;
    }

    public async Task<object> Create(StudentBehaviourRecord entity)
    {
        var datta = await _repositoryStudentBehaviourRecord.FirstOrDefaultAsync(x => x.AcademicYearId == entity.AcademicYearId && x.TermId == entity.TermId && x.ClassId == entity.ClassId && x.StudentId == entity.StudentId);
        if (datta == null)
        {
            await _repositoryStudentBehaviourRecord.InsertAsync(entity);

            return new
            {
                code = 200,
                message = "successful"
            };
        }
        
        throw new Abp.UI.UserFriendlyException(400, "Duplicate records are not allowed.");

    }

    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<StudentBehaviourRecord>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.StudentIdentifier.Contains(query) || x.StudentName.Contains(query));
        }
            
        var totalCount =
                
            await _repositoryStudentBehaviourRecord.CountAsync(predicate);
            
        var data = await (await _repositoryStudentBehaviourRecord.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.StudentId,a.StudentIdentifier, a.StudentName, a.ClassName, a.AcademicYearName, a.TermName, a.ConductOrAttitudeRemarks, a.GeneralRemarks }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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

    
    
    public async Task<object> GetViaStudentId(Guid studentId)
    {
       return await _repositoryStudentBehaviourRecord.FirstOrDefaultAsync(x=> x.StudentId == studentId);
    }
    
    public async Task Delete(Guid id)
    {
        await _repositoryStudentBehaviourRecord.DeleteAsync(id);
    }

    public async Task Update(StudentBehaviourRecord entity)
    {
        await _repositoryStudentBehaviourRecord.UpdateAsync(entity);
    }
}