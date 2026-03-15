using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.HubtelPayment.Dto;
using RhyoliteOrbit.DomainServices.Subscription;
using RhyoliteOrbit.DomainServices.Subscription.Dto;

namespace RhyoliteOrbit.Services.Subscription;

public class SubscriptionAppService : RhyoliteOrbitAppServiceBase
{
    private readonly ISubscriptionManager  _subscriptionManager;

    public SubscriptionAppService(ISubscriptionManager subscriptionManager)
    {
        _subscriptionManager = subscriptionManager;
    }
    
    
    [HttpGet]
    public async Task<object> GetPayments(int pageNo, int pageSize, string query)
    {
        return await _subscriptionManager.ListPayments(pageNo, pageSize, query);
    }
    
    [HttpGet]
    public async Task<object> GetSubscriptionInfo()
    {
        var tenantInfo = await GetCurrentTenantAsync();
        return new
        {
            tenantInfo.BusinessId,
            tenantInfo.Name,
            tenantInfo.BusinessLogo,
            tenantInfo.Country,
            tenantInfo.HasSubscribedToAp, 
            tenantInfo.HasSubscribedToAr, 
            tenantInfo.HasSubscribedToGl,
            tenantInfo.HasSubscribedToPayroll,
            tenantInfo.HasSubscribedToPropertyRentals,
            tenantInfo.HasSubscribedToSchoolManager,
            
        };
    }
    
    [HttpPost]
    public async Task<object> InitializeSubscriptionPayment(SubscriptionPaymentDto dto)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        dto.TenantId = tenantInfo.Id;
        
        if (dto.IsDirectDebitPayment)
        {
            return await _subscriptionManager.InitializeRecurringSubscriptionPayment(dto);
        }
        
        return await _subscriptionManager.InitializeSubscriptionPayment(dto);
    }
    
    
    
    [HttpGet]
    public async Task<object> CompleteSubscriptionPayment(string reference)
    {
        return await _subscriptionManager.CompleteSubscriptionPayment(reference);
    }
    
    [HttpPost]
    public async Task<object> RenewSubscription(DirectDebitChargeDto dto)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        
        return await _subscriptionManager.RenewSubscription(dto, tenantInfo.Id);
    }
    
    [HttpGet]
    public async Task<object> PreapprovalStatusCheck(string reference)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        
        return await _subscriptionManager.PreapprovalStatusCheck(reference, tenantInfo.Id);
    }
    
    [HttpGet]
    public async Task<object> CancelPreapproval(string phoneNumber)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        
        return await _subscriptionManager.CancelPreapproval(phoneNumber, tenantInfo.Id);
    }
    
    
    
        
     
}