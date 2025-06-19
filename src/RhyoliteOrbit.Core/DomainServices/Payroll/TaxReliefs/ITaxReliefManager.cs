using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.TaxReliefs;

public interface ITaxReliefManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> Create(TaxRelief input);
    Task Update(TaxRelief input);
    Task Delete(Guid id);
}