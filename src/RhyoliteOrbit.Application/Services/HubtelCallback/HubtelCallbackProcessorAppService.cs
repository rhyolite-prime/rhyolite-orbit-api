using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RhyoliteOrbit.DomainServices.HubtelPayment.Dto;
using RhyoliteOrbit.DomainServices.Quartz;
using RhyoliteOrbit.DomainServices.Quartz.Dto;
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

        // may use tenantId (e.g., set actor/tenant context) before completing payment
        await _subscriptionManager.CompleteSubscriptionPayment(clientReference, phoneNumber, tenantId);
    }

    [HttpPost]
    public async Task Preapproval(HubtelPreapprovalCallbackResponse response)
    {
        Logger.Info($"preapproval_callback => {JsonConvert.SerializeObject(response)}");
       
        if (response.PreapprovalStatus.Equals("APPROVED"))
        {
            var clientReference = response.ClientReferenceId;

            if (string.IsNullOrWhiteSpace(clientReference))
            {
                Logger.Warn("preapproval_callback => ClientReferenceId is null/empty");
                return;
            }
            
            var lastUnderscoreIndex = clientReference.LastIndexOf('_');
            if (lastUnderscoreIndex < 0 || lastUnderscoreIndex == clientReference.Length - 1)
            {
                Logger.Warn($"preapproval_callback => ClientReference has no tenant suffix: {clientReference}");
                return;
            }
            
            var tenantIdPart = clientReference[(lastUnderscoreIndex + 1)..];
            if (!int.TryParse(tenantIdPart, out var tenantId))
            {
                Logger.Warn($"preapproval_callback => TenantId part is not numeric: {tenantIdPart} (ClientReference: {clientReference})");
                return;
            }

            // do a preapproval status check before proceeding
            
            await _subscriptionManager.PreapprovalStatusCheck(clientReference,tenantId);
           // if (preapprovalStatusCheckResponse == null || preapprovalStatusCheckResponse.Data.PreapprovalStatus != "APPROVED")
           // {
           //     
           // }
            
            var renewalHistory = await _subscriptionManager.GetSubscriptionRenewalHistory(clientReference, tenantId);
            if (renewalHistory == null)
            {
                Logger.Warn($"preapproval_callback => Renewal history not found for reference: {clientReference}, tenantId: {tenantId}");
                return;
            }

            // 1. Schedule Quartz job for subsequent renewals
            var scheduleCron = $"0 {renewalHistory.RenewalEndDate.Minute} " +
                               $"{renewalHistory.RenewalEndDate.Hour} " +
                               $"{renewalHistory.RenewalEndDate.Day} " +
                               $"{renewalHistory.RenewalEndDate:MMM} " +
                               $"? " +
                               $"{renewalHistory.RenewalEndDate.Year}";

            var payload = new JobModel
            {
                Name = $"SUBSCRIPTION_RENEWAL_{tenantId}_{renewalHistory.Id}",
                Schedule = scheduleCron,
                StartDate = renewalHistory.RenewalEndDate.AddMinutes(-5),
                EndDate = renewalHistory.RenewalEndDate.AddMinutes(5),
                Description = $"Subscription renewal processing for tenant {tenantId}",
                CustomData = new CustomData
                {
                    CallbackUrl = $"http://213.165.245.124:5004/api/services/app/Subscription/RunScheduledRenewal",
                    UniqueId = $"{renewalHistory.Id:N}_{tenantId}"
                }
            };

            await _jobScheduler.CreateJob(payload);

            // 2. Trigger Direct Debit Charge for the current cycle
            await _subscriptionManager.RenewSubscription(new DirectDebitChargeDto
            {
                CustomerMsisdn = response.CustomerMsisdn,
                Amount = renewalHistory.Amount,
                PrimaryCallbackUrl = "http://213.165.245.124:5004/api/services/app/HubtelCallbackProcessor/DirectDebitCharge",
                Channel = renewalHistory.Channel,
                CustomerEmail = string.Empty,
                CustomerName = string.Empty,
                Description = renewalHistory.Notes
            }, tenantId);
        }
        else
        {
            Logger.Warn($"preapproval_callback => Preapproval not approved: {response.PreapprovalStatus}");
        }
    }
    
    [HttpPost]
    public async Task DirectDebitCharge(DirectDebitChargeCallbackResponse response)
    {
        Logger.Info($"direct_debit_charge_callback => {JsonConvert.SerializeObject(response)}");

        if (!response.ResponseCode.Equals("0000"))
        {
            Logger.Warn($"direct_debit_charge_callback => Charge failed: {response.Message}");
            return;
        }

        var clientReference = response.Data?.ClientReference;
        if (string.IsNullOrWhiteSpace(clientReference))
        {
            Logger.Warn("direct_debit_charge_callback => ClientReference is null/empty");
            return;
        }

        try
        {
            var lastUnderscoreIndex = clientReference.LastIndexOf('_');
            if (lastUnderscoreIndex < 0 || lastUnderscoreIndex == clientReference.Length - 1)
            {
                Logger.Warn($"direct_debit_charge_callback => ClientReference has no tenant suffix: {clientReference}");
                return;
            }

            var tenantIdPart = clientReference[(lastUnderscoreIndex + 1)..];
            if (!int.TryParse(tenantIdPart, out var tenantId))
            {
                Logger.Warn($"direct_debit_charge_callback => TenantId part is not numeric: {tenantIdPart} (ClientReference: {clientReference})");
                return;
            }

            await _subscriptionManager.CompleteRecurringSubscriptionPayment(clientReference, tenantId);
        }
        catch (Exception ex)
        {
            Logger.Error($"direct_debit_charge_callback => Error processing charge for reference: {clientReference}", ex);
            throw;
        }
    }
    
    
    [HttpPost]
    public async Task Acknowledgement(object response)
    {
        await Task.Delay(1);
        
        Logger.Info($"acknowledged_callback => {JsonConvert.SerializeObject(response)}");

    }
    
    
    
    
}