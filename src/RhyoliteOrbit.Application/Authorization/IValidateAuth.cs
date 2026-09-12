using System;
using System.Threading.Tasks;

namespace RhyoliteOrbit.Authorization;

public interface IValidateAuth:  Abp.Application.Services.IApplicationService
{
    Task<int> ValidateTenant();
    Task<Guid> ValidateCustomer();
}