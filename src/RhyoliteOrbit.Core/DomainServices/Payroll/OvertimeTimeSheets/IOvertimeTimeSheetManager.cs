using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.OvertimeTimeSheets;

public interface IOvertimeTimeSheetManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> ListAll(int month,int year);
    Task Create(OvertimeTimeSheet input);
    Task Update(OvertimeTimeSheet input);
    Task Delete(Guid id);   
}