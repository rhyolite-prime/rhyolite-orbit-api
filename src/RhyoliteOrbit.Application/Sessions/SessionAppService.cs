using Abp.Auditing;
using RhyoliteOrbit.Sessions.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using HashidsNet;
using Microsoft.AspNetCore.Mvc;

namespace RhyoliteOrbit.Sessions;

public class SessionAppService : RhyoliteOrbitAppServiceBase, ISessionAppService
{
    
    private readonly IHashids _hashids;

    public SessionAppService(IHashids hashids)
    {
        _hashids = hashids;
    }
    
    [HttpGet]
    [DisableAuditing]
    public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
    {
        var output = new GetCurrentLoginInformationsOutput
        {
            Application = new ApplicationInfoDto
            {
                Version = AppVersionHelper.Version,
                ReleaseDate = AppVersionHelper.ReleaseDate,
                Features = new Dictionary<string, bool>()
            }
        };

        if (AbpSession.TenantId.HasValue)
        {
            output.Tenant = ObjectMapper.Map<TenantLoginInfoDto>(await GetCurrentTenantAsync());
        }

        if (AbpSession.UserId.HasValue)
        {
            output.User = ObjectMapper.Map<UserLoginInfoDto>(await GetCurrentUserAsync());
        }

        return output;
    }
    
    [HttpGet]
    [DisableAuditing]
    public object GetAccountSecret()
    {
        var tenantId = AbpSession.TenantId ?? 0;
            
        var userId = AbpSession.UserId ?? 0;
            
        var accountSecret = _hashids.Encode(tenantId);
            
        var accountKey = _hashids.EncodeLong(userId);
            
        return new { AccountSecret = accountSecret, AccountKey = accountKey};
    }
    
    
}
