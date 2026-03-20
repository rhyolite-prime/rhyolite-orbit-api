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
            tenantInfo.Network,
            tenantInfo.PhoneNumber,
            tenantInfo.BusinessLogo,
            tenantInfo.Country,
            tenantInfo.HasSubscribedToAp, 
            tenantInfo.HasSubscribedToAr, 
            tenantInfo.HasSubscribedToGl,
            tenantInfo.HasSubscribedToPayroll,
            tenantInfo.HasSubscribedToPropertyRentals,
            tenantInfo.HasSubscribedToSchoolManager,
            tenantInfo.HasSubscribedToDocumentManagement,
            tenantInfo.HasSubscribedToFixedAssets,
            tenantInfo.HasSubscribedToHumanResource,
            tenantInfo.HasSubscribedToManufacturing,
            tenantInfo.HasSubscribedToSupplyChain,
            tenantInfo.HasSubscribedToPointOfSale,
            tenantInfo.HasSubscribedToCoreBanking,
            tenantInfo.HasSubscribedToInventory
            
        };
    }
    
    [HttpPost]
    public async Task<SubscriptionInitializationResponse> InitializeSubscriptionPayment(SubscriptionPaymentDto dto)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        dto.TenantId = tenantInfo.Id;
        
        if (dto.IsDirectDebitPayment)
        {
            return await _subscriptionManager.InitializeRecurringSubscriptionPayment(dto);
        }
        
        return await _subscriptionManager.InitializeSubscriptionPayment(dto);
    }
    
    [HttpPost]
    public async Task<VerifyPreapprovalResponse> VerifyPreapproval(VerifyPreapprovalDto dto)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        return await _subscriptionManager.VerifyPreapproval(dto, tenantInfo.Id);
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
    
    
    [HttpGet]
    public async Task<object> CompleteSubscriptionPaymentViaDirectCharge(string reference)
    {
        var tenantInfo = await GetCurrentTenantAsync();
        
        return await _subscriptionManager.CompleteSubscriptionPaymentViaDirectCharge(reference, tenantInfo.Id);
    }
    
    [HttpPost]
    public async Task<object> RunScheduledRenewal(string uniqueId)
    {
         //extract subscription renewal id and tenantid-> format: UniqueId = $"{subscriptionRenewalHistory?.Id:N}_{tenantId}"
         // proceed to do a direct charge if preapproval is set
         // if not send a reminder with a payment link
         await Task.Delay(1);
         
         return new
         {
            success = true,
            message = "Renewal scheduled"
         };
         
         //return await _subscriptionManager.RenewSubscription(dto, tenantInfo.Id);
    }
        
     
}