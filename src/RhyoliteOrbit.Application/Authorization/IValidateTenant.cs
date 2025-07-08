using System.Threading.Tasks;

namespace RhyoliteOrbit.Authorization;

public interface IValidateTenant:  Abp.Application.Services.IApplicationService
{
    Task<int> Validate();
}