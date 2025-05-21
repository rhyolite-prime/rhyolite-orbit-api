using Abp.Application.Services;
using RhyoliteOrbit.Sessions.Dto;
using System.Threading.Tasks;

namespace RhyoliteOrbit.Sessions;

public interface ISessionAppService : IApplicationService
{
    Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
}
