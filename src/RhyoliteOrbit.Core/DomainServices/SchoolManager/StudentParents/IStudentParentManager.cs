using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.StudentParents;

public interface IStudentParentManager : Abp.Domain.Services.IDomainService
{
    Task<object> GetStudentParent(Guid id);
    Task Create(StudentParent entity);
    Task Update(StudentParent entity);
    Task Delete(Guid id);

    //list parent's children
    Task<List<Guid>> ListChildren(Guid parentId);
}