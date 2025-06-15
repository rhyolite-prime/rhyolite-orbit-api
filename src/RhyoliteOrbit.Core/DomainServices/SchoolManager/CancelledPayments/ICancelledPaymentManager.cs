using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.CancelledPayments;

public interface ICancelledPaymentManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(Guid academicYearId, Guid termId, Guid classId);
    Task CreateBatch(List<CancelledPayment> payments);
}