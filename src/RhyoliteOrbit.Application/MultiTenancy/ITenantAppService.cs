using Abp.Application.Services;
using RhyoliteOrbit.MultiTenancy.Dto;

namespace RhyoliteOrbit.MultiTenancy;

public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
{
}

