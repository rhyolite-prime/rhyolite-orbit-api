using System.Threading.Tasks;
using Abp.Application.Services;
using RhyoliteOrbit.MultiTenancy.Dto;

namespace RhyoliteOrbit.MultiTenancy;

public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
{
    Task<object> GenerateApiKeys();
}

