using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.GeneralLedger;

namespace RhyoliteOrbit.DomainServices.GeneralLedger.CoaControls;

public interface ICoaControlManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query, Guid accountHeadId);
    Task<object> ListAll(Guid id);
    Task Create(CoaControl input);
    Task Update(CoaControl input);
    Task Delete(Guid id);
}