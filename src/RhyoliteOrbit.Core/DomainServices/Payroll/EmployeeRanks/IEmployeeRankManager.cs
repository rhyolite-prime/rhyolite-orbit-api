using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeRanks;

public interface IEmployeeRankManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task Create(EmployeeRank input);
    Task Update(EmployeeRank input);
    Task Delete(Guid id);
}