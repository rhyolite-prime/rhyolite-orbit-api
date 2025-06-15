using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.StudentBehaviorRecords;

public interface IStudentBehaviourRecordManager: Abp.Domain.Services.IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query);
    Task<object> GetViaStudentId(Guid studentId);
    Task<object> Create(StudentBehaviourRecord entity);
    Task Update(StudentBehaviourRecord entity);
    Task Delete(Guid id);
}