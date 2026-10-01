using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.PaymentOptions;

public interface IPaymentOptionManager: IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task<object> GetAll();
    Task Create(PaymentOption input, string dataSupportBusinessAccountId);
    Task Update(PaymentOption input, string dataSupportBusinessAccountId);
    Task Delete(Guid id, string dataSupportBusinessAccountId);
}