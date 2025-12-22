using System.Threading.Tasks;
using RhyoliteOrbit.Models.GeneralLedger;

namespace RhyoliteOrbit.DomainServices.GeneralLedger.DefaultGlNumbers;

public interface IDefaultGlNumberManager: Abp.Domain.Services.IDomainService
{
    Task<object> GetDefaultSetup();
    Task Create(DefaultGlNumber input);
    Task Update(DefaultGlNumber input);
}