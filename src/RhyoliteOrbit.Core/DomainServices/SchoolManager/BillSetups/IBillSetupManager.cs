using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.BillSetups;

public interface IBillSetupManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(Guid academicYearId, Guid termId, Guid classId, Guid billTypeId);
    Task<object> GetAsync(Guid academicYearId, Guid termId, Guid classId, Guid billTypeId);
    Task<object> Create(BillSetup entity);
    Task DeleteDetail(Guid id, Guid headerId);
    Task Delete(Guid id);
}