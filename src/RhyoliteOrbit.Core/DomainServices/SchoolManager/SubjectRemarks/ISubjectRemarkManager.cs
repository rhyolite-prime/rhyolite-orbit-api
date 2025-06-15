using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.SubjectRemarks;

public interface ISubjectRemarkManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task Create(SubjectRemark entity);
    Task Update(SubjectRemark entity);
    Task Delete(Guid id);
}