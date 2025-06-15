using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.StudentStatements;

public class StudentStatementManager: Abp.Domain.Services.DomainService, IStudentStatementManager
{
    private readonly IRepository<StudentStatement, Guid> _repositoryStudentStatement;

    public StudentStatementManager(IRepository<StudentStatement, Guid> repositoryStudentStatement)
    {
        _repositoryStudentStatement = repositoryStudentStatement;
    }

    public async Task<object> GetStatement(Guid studentId)
    {
        return await _repositoryStudentStatement.FirstOrDefaultAsync(x => x.StudentId == studentId);
    }
}