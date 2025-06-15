using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.PaymentTypes;

public interface IPaymentTypeManager : Abp.Domain.Services.IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query);
    Task<object> Create(PaymentType entity);
    Task Update(PaymentType entity);
    Task Delete(Guid id);
}