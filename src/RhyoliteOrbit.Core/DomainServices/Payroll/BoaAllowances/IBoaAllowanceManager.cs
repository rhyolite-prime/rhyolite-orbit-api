using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.BoaAllowances;

public interface IBoaAllowanceManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> ListAll(int month, int year);
    Task Create(BonusAndOnetimeAllowance input);
    Task Update(BonusAndOnetimeAllowance input);
    Task Delete(Guid id);
}