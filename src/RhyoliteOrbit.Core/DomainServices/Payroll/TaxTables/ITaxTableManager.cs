using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.TaxTables;

public interface ITaxTableManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize);
    Task Create(TaxTable input);
    Task Update(TaxTable input);
    Task Delete(Guid id);
}