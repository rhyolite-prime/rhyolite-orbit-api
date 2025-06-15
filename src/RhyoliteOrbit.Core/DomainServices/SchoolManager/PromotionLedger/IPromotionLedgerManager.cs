using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.PromotionLedger;

public interface IPromotionLedgerManager: Abp.Domain.Services.IDomainService
{
    Task Create(PromotionHistory input);
    
    Task<object> ListAll(Guid academicYearId);
}