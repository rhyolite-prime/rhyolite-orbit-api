using System;
using System.Threading.Tasks;

namespace RhyoliteOrbit.DomainServices.Payroll.TaxTableHistories;

public interface ITaxTableHistoryManager : Abp.Domain.Services.IDomainService
{
    Task Create(string title);
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task Delete(Guid id);
    Task Restore(Guid id);
}