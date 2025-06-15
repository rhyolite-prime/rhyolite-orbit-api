using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.Parents;

public interface IParentManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(Guid id);
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> ListParentWards();
    Task<object> GetParentInfo(Guid id);
    Task<object> Create(Parent input);
    Task<object> Update(Parent input);
    Task Delete(Guid id);
    Task<object> ListParents();

    Task<object> VerifyParent(string phoneNo, string pin, int tenantId);
}