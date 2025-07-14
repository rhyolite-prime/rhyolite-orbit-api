using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.GeneralLedger;

namespace RhyoliteOrbit.DomainServices.GeneralLedger.BankAccounts;

public interface IBankAccountManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task Create(BankAccount input);
    Task Update(BankAccount input);
    Task Delete(Guid id);
}