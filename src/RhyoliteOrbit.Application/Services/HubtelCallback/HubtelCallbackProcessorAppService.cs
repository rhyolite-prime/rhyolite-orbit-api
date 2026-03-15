using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RhyoliteOrbit.DomainServices.HubtelPayment.Dto;
using RhyoliteOrbit.DomainServices.Quartz;
using RhyoliteOrbit.DomainServices.Subscription;

namespace RhyoliteOrbit.Services.HubtelCallback;

public class HubtelCallbackProcessorAppService: RhyoliteOrbitAppServiceBase
{
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly IQuartzJobScheduler _jobScheduler;
    
    public HubtelCallbackProcessorAppService(ISubscriptionManager subscriptionManager, IQuartzJobScheduler jobScheduler)
    {
        _subscriptionManager = subscriptionManager;
        _jobScheduler = jobScheduler;
    }
    

    [HttpPost]
    public async Task WebCheckout(HubtelWebCheckoutCallbackResponse response)
    {
        Logger.Info($"web_checkout_callback => {JsonConvert.SerializeObject(response)}");

         
        var clientReference = response?.Data?.ClientReference;
        string phoneNumber = response?.Data?.CustomerPhoneNumber;

        if (string.IsNullOrWhiteSpace(clientReference))
        {
            Logger.Warn("web_checkout_callback => ClientReference is null/empty");
            return;
        }

        // Digits after the last underscore are the tenant id
        var lastUnderscoreIndex = clientReference.LastIndexOf('_');
        if (lastUnderscoreIndex < 0 || lastUnderscoreIndex == clientReference.Length - 1)
        {
            Logger.Warn($"web_checkout_callback => ClientReference has no tenant suffix: {clientReference}");
            return;
        }

        var tenantIdPart = clientReference[(lastUnderscoreIndex + 1)..];
        if (!int.TryParse(tenantIdPart, out var tenantId))
        {
            Logger.Warn($"web_checkout_callback => TenantId part is not numeric: {tenantIdPart} (ClientReference: {clientReference})");
            return;
        }

        // TODO: use tenantId (e.g., set actor/tenant context) before completing payment
        await _subscriptionManager.CompleteSubscriptionPayment(clientReference, phoneNumber, tenantId);
    }

    [HttpPost]
    public async Task Preapproval(HubtelPreapprovalCallbackResponse response)
    {
        await Task.Delay(1);
        
        Logger.Info($"preapproval_callback => {JsonConvert.SerializeObject(response)}");
       
        if (response.PreapprovalStatus.Equals("APPROVED"))
        {
            //if preapproval is successful we do a direct charge and schedule another debit based on the billing cycle
            
            var clientReference = response.ClientReferenceId;

            if (string.IsNullOrWhiteSpace(clientReference))
            {
                Logger.Warn("web_checkout_callback => ClientReference is null/empty");
                return;
            }
            
            var lastUnderscoreIndex = clientReference.LastIndexOf('_');
            if (lastUnderscoreIndex < 0 || lastUnderscoreIndex == clientReference.Length - 1)
            {
                Logger.Warn($"web_checkout_callback => ClientReference has no tenant suffix: {clientReference}");
                return;
            }
            
            var tenantIdPart = clientReference[(lastUnderscoreIndex + 1)..];
            if (!int.TryParse(tenantIdPart, out var tenantId))
            {
                Logger.Warn($"web_checkout_callback => TenantId part is not numeric: {tenantIdPart} (ClientReference: {clientReference})");
                return;
            }
            
            await _subscriptionManager.RenewSubscription(new DirectDebitChargeDto
            {
                CustomerMsisdn =  response.CustomerMsisdn, 
                Amount = 0, 
                PrimaryCallbackUrl = "", 
                Channel = "", 
                CustomerEmail = "",
                CustomerName = "", 
                Description = ""
                
                
            }, tenantId);
            
            
            
        }
        else
        {
            //send an email telling the customer somethinf
        }

    }
    
    [HttpPost]
    public async Task DirectDebitCharge(DirectDebitChargeCallbackResponse response)
    {
        await Task.Delay(1);
        
        Logger.Info($"direct_debit_charge_callback => {JsonConvert.SerializeObject(response)}");

    }
    
    
    
}