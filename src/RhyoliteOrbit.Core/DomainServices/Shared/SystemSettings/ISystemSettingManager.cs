using System.Threading.Tasks;

namespace RhyoliteOrbit.DomainServices.Shared.SystemSettings;

public interface ISystemSettingManager: Abp.Domain.Services.IDomainService
{
    Task<string> GetDefaultCurrencyCode();
}