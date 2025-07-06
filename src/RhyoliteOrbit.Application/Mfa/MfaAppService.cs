using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;

namespace RhyoliteOrbit.Mfa;

public class MfaAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IMfaManager _mfaManager;
    private readonly IValidateTenant _validateTenant;
    
    public MfaAppService(IMfaManager mfaManager, IValidateTenant validateTenant)
    {
        _mfaManager = mfaManager;
        _validateTenant = validateTenant;
    }
    
    /// <summary>
    /// SendEmailOtp
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<object> SendEmailOtp(string subject)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        var userInfo = await GetCurrentUserAsync();
        
        return await _mfaManager.SendEmailOtp(userInfo.EmailAddress, subject, tenantInfo.Id);
    }
    
    /// <summary>
    /// VerifyEmailOtp
    /// </summary>
    /// <param name="otp"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<object> VerifyOtp(string otp)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        var userInfo = await GetCurrentUserAsync();
        
        return await _mfaManager.VerifyOtp(userInfo.EmailAddress, otp, tenantInfo.Id);
    }
}