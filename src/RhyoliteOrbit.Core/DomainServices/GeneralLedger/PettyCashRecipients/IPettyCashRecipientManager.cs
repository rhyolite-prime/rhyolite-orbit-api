using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.GeneralLedger;

namespace RhyoliteOrbit.DomainServices.GeneralLedger.PettyCashRecipients;

public interface IPettyCashRecipientManager: IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);
    Task Create(PettyCashRecipient input);
    Task Update(PettyCashRecipient input);
    Task Delete(Guid id);
}