using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.Gratuities;

public interface IGratuityManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize);
    Task Create(Gratuity input);
    Task Update(Gratuity input);
    Task Delete(Guid id);
}