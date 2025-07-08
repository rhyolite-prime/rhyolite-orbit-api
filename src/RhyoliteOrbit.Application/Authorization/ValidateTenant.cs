using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.UI;
using HashidsNet;
using Microsoft.AspNetCore.Http;

namespace RhyoliteOrbit.Authorization;

public class ValidateTenant: RhyoliteOrbitAppServiceBase, IValidateTenant
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHashids _hashids;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    /// <param name="hashids"></param>
    public ValidateTenant(IHttpContextAccessor httpContextAccessor, IHashids hashids)
    {
        _httpContextAccessor = httpContextAccessor;
        _hashids = hashids;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="UserFriendlyException"></exception>
    public Task<int> Validate()
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            if (!_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("AccountSecret", out var accountSecret))
            {
                throw new Exception("Account Secret is required");
            }
            
            return Task.FromResult(_hashids.Decode(accountSecret).FirstOrDefault());
        }

        throw new UserFriendlyException("Request Failed !");
    }
}