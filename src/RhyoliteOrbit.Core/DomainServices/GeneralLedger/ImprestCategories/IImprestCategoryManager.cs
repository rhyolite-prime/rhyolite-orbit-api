using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.GeneralLedger;

namespace RhyoliteOrbit.DomainServices.GeneralLedger.ImprestCategories;

public interface IImprestCategoryManager : Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> GetAsync(Guid id);
    Task Create(ImprestCategory input);
    Task Update(ImprestCategory input);
    Task Delete(Guid id);
}