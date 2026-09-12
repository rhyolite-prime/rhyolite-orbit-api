using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Abp.UI;
using HashidsNet;
using Microsoft.AspNetCore.Http;

namespace RhyoliteOrbit.Authorization;

public class ValidateAuth: RhyoliteOrbitAppServiceBase, IValidateAuth
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHashids _hashids;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    /// <param name="hashids"></param>
    public ValidateAuth(IHttpContextAccessor httpContextAccessor, IHashids hashids)
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
    public Task<int> ValidateTenant()
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            if (!_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("AccountId", out var accountId))
            {
                throw new Exception("Account Id is required");
            }
            
            if (!_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("AccountSecret", out var accountSecret))
            {
                throw new Exception("A Pinch of salt is required");
            }
            
            var hashids = new Hashids(accountSecret, 10);
            
            var decodedAccountId = hashids.Decode(accountId).FirstOrDefault();
                
            return Task.FromResult(decodedAccountId);
        }

        throw new UserFriendlyException("Request Failed !");
    }
    
    
    public Task<Guid> ValidateCustomer()
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            if (!_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("X-Customer", out var customerToken))
            {
                throw new Exception("X-Customer Header is required");
            }
            
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(customerToken);

            var customerIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "customerId");
            if (customerIdClaim == null)
            {
                throw new Exception("Invalid token");
            }

            return Task.FromResult(Guid.Parse(customerIdClaim.Value));
            
        }

        throw new UserFriendlyException("Request Failed !");
    }
    
    
}