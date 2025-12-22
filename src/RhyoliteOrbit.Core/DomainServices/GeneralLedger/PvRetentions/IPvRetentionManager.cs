using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.GeneralLedger;

namespace RhyoliteOrbit.DomainServices.GeneralLedger.PvRetentions;

public interface IPvRetentionManager: IDomainService
{
    Task<object> ListAll(Guid paymentVoucherId);
    Task Create(PvRetention input);
    Task Update(PvRetention input);
    Task Delete(Guid id);
}