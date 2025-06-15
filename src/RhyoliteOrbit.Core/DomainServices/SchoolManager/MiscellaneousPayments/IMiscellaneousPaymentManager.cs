using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.MiscellaneousPayments;

public interface IMiscellaneousPaymentManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll();
    Task<object> ListAll(Guid paymentTypeId);
    Task<object> ListAll(DateTime startDate, DateTime endDate, Guid paymentTypeId);
    Task<object> ListAllUnposted();
    Task Create(MiscellaneousPayment entity);
    Task Delete(Guid id);
    Task BulkDelete(List<Guid> paymentList);
}