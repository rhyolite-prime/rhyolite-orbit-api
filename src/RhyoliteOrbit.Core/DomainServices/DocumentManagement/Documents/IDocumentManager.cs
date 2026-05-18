using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.DocumentManagement;

namespace RhyoliteOrbit.DomainServices.DocumentManagement.Documents;

public interface IDocumentManager : Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task Create(Document entity);
    Task Update(Document entity);
    Task Delete(Guid id);
}
