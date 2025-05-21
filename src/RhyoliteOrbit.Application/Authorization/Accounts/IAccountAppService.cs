using Abp.Application.Services;
using RhyoliteOrbit.Authorization.Accounts.Dto;
using System.Threading.Tasks;

namespace RhyoliteOrbit.Authorization.Accounts;

public interface IAccountAppService : IApplicationService
{
    Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

    Task<RegisterOutput> Register(RegisterInput input);
}
