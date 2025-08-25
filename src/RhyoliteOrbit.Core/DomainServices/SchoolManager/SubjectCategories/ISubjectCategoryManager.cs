using System;
using System.Threading.Tasks;

namespace RhyoliteOrbit.DomainServices.SchoolManager.SubjectCategories;

public interface ISubjectCategoryManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task Create(SubjectCategory entity);
    Task Update(SubjectCategory entity);
    Task Delete(Guid id);
}