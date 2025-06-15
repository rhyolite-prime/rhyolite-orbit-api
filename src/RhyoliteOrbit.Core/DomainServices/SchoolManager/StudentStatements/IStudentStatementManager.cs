using System;
using System.Threading.Tasks;

namespace RhyoliteOrbit.DomainServices.SchoolManager.StudentStatements;

public interface IStudentStatementManager: Abp.Domain.Services.IDomainService
{
    Task<object> GetStatement(Guid studentId);
}