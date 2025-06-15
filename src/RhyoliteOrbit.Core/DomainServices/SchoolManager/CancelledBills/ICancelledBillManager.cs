using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.CancelledBills;

public interface ICancelledBillManager: Abp.Domain.Services.IDomainService
{
    Task CreateBatch(List<CancelledBill> bills);
    Task<object> ListAll(Guid academicYearId, Guid termId, Guid classId, Guid billTypeId);
}