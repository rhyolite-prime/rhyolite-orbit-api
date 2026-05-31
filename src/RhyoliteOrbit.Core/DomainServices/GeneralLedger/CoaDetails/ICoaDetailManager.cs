using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.GeneralLedger;

namespace RhyoliteOrbit.DomainServices.GeneralLedger.CoaDetails;

public interface ICoaDetailManager : Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query, int status);
    Task<object> ListActiveAccounts();
    Task Create(CoaDetail entity);
    
    Task Update(CoaDetail entity);
    Task Delete(Guid id);
}