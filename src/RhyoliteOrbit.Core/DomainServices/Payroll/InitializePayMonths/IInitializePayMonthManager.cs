using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.InitializePayMonths;

public interface IInitializePayMonthManager: Abp.Domain.Services.IDomainService
{
    Task<object> GetData();
    
    Task Create(InitializePayMonth entity);
}