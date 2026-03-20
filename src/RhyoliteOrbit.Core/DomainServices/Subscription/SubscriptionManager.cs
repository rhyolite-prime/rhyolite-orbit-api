using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RhyoliteOrbit.DomainServices.HubtelPayment;
using RhyoliteOrbit.DomainServices.HubtelPayment.Dto;
using RhyoliteOrbit.DomainServices.Quartz;
using RhyoliteOrbit.DomainServices.Quartz.Dto;
using RhyoliteOrbit.DomainServices.Subscription.Dto;
using RhyoliteOrbit.Models.Shared;
using RhyoliteOrbit.MultiTenancy;

namespace RhyoliteOrbit.DomainServices.Subscription;

public class SubscriptionManager: DomainService, ISubscriptionManager
{
    private readonly IRepository<SubscriptionPayment,Guid> _repositorySubscriptionPayment;
    private readonly IRepository<SubscriptionRenewalHistory,Guid> _repositorySubscriptionRenewalHistory;
    private readonly IRepository<Tenant> _repositoryTenant;
    private readonly IHubtelPaymentApi  _hubtelPaymentApi;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly TenantManager _tenantManager;
    private readonly IQuartzJobScheduler _quartzJobScheduler;
    private readonly SubscriptionPricingOptions _pricing;


    public SubscriptionManager(IRepository<SubscriptionPayment, Guid> repositorySubscriptionPayment, IHubtelPaymentApi hubtelPaymentApi, IUnitOfWorkManager unitOfWorkManager, TenantManager tenantManager, IRepository<SubscriptionRenewalHistory, Guid> repositorySubscriptionRenewalHistory, IOptions<SubscriptionPricingOptions> pricingOptions, IQuartzJobScheduler quartzJobScheduler, IRepository<Tenant> repositoryTenant)
    {
        _repositorySubscriptionPayment = repositorySubscriptionPayment;
        _hubtelPaymentApi = hubtelPaymentApi;
        _unitOfWorkManager = unitOfWorkManager;
        _tenantManager = tenantManager;
        _repositorySubscriptionRenewalHistory = repositorySubscriptionRenewalHistory;
        _quartzJobScheduler = quartzJobScheduler;
        _pricing = pricingOptions.Value;
        _repositoryTenant = repositoryTenant;
    }

    public async Task<object> ListPayments(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<SubscriptionPayment>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Description.ToLower().Contains(query.ToLower()) || x.Reference == query || x.PhoneNumber == query);
        }
            
        var totalCount =
                
            await _repositorySubscriptionPayment.CountAsync(predicate);
            
        var data = await (await _repositorySubscriptionPayment.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.PhoneNumber, CreationTime = a.CreationTime.ToString("dd-MMM-yyyy"), a.Cycle, a.Description, Reference = ToDisplayReference(a.Reference), a.IsDirectDebitPayment, a.Amount, Status = ((RhyoliteOrbitConsts.PaymentStatus)a.Status).ToString(),}).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

        var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

        var paginatedResult = new
        {
            pageNo,
            pageSize,
            totalCount,
            totalPages,
            data,
            lowerBound = pageSize * (pageNo - 1) + 1,
            upperBound = totalPages == pageNo ? totalCount : pageNo * pageSize,
        };

        return paginatedResult;
    }
   
    public async Task<SubscriptionInitializationResponse> InitializeSubscriptionPayment(SubscriptionPaymentDto dto)
    {
        var tenantInfo = await _tenantManager.FindByIdAsync(dto.TenantId);
        var (totalAmount, description, renewalEndDate) = CalculateSubscriptionDetails(dto);
        dto.Amount = totalAmount;
        
        var paymentResponse = await _hubtelPaymentApi.InitializeWebCheckout(new WebCheckoutDto 
            { 
                TotalAmount = dto.Amount, 
                Description = "Rhyolite ERP Subscription", 
                CallbackUrl = "http://213.165.245.124:5004/api/services/app/HubtelCallbackProcessor/WebCheckout", 
                CancellationUrl = "https://erp.rhyoliteprime.com/Account/ModuleManager", 
                ReturnUrl = "https://erp.rhyoliteprime.com"
            }, 
            
            dto.TenantId);

        if (paymentResponse is { Code: (int)RhyoliteOrbitConsts.PaymentStatus.Processing })
        {
            await CreateSubscriptionRecords(dto, paymentResponse.ClientReference, tenantInfo.Country, description, renewalEndDate);
        }
        
        return new SubscriptionInitializationResponse
        {
            PaymentMode = "WebCheckout",
            Status = "Processing",
            Message = "Redirecting to payment gateway...",
            CheckoutUrl = paymentResponse.CheckoutUrl,
            ClientReference = paymentResponse.ClientReference
        };
    }
    
    public async Task<SubscriptionInitializationResponse> InitializeRecurringSubscriptionPayment(SubscriptionPaymentDto dto)
    {
        var tenantInfo = await _tenantManager.FindByIdAsync(dto.TenantId);
        var (totalAmount, description, renewalEndDate) = CalculateSubscriptionDetails(dto);
        dto.Amount = totalAmount;

        // Normalise channel name regardless of which path we take
        if (dto.Channel.Equals("mtn", StringComparison.OrdinalIgnoreCase))
        {
            dto.Channel = "mtn-gh-direct-debit";
            dto.Network = "mtn";
        }
        else if (dto.Channel.Equals("telecel", StringComparison.OrdinalIgnoreCase))
        {
            dto.Channel = "vodafone-gh-direct-debit";
            dto.Network = "telecel";
        }

        // Fast-path: tenant already has an approved direct-debit mandate — skip pre-approval
        if (tenantInfo.IsDirectDebitEnabled)
        {
            var chargeResponse = await _hubtelPaymentApi.DirectDebitCharge(new DirectDebitChargeDto
            {
                CustomerMsisdn = dto.PhoneNumber,
                Amount = dto.Amount,
                PrimaryCallbackUrl = "http://213.165.245.124:5004/api/services/app/HubtelCallbackProcessor/Acknowledgement",
                Channel = dto.Channel,
                CustomerEmail = $"{tenantInfo.Name}@rhyoliteprime.com",
                CustomerName = tenantInfo.Name,
                Description = description,
            }, dto.TenantId);

            await CreateSubscriptionRecords(dto, chargeResponse.Data?.TransactionId, tenantInfo.Country, description, renewalEndDate);
            
            return new SubscriptionInitializationResponse
            {
                PaymentMode = "DirectDebit",
                Status = "Processing",
                Title = "Subscription Auto-Charge Initiated",
                Message = $"As you have authorized pre-approval on this account ({tenantInfo.PhoneNumber}), your account will be debited without the need for PIN authorization.",
                CheckoutUrl = string.Empty,
                ClientReference = chargeResponse.Data?.TransactionId
            };
        }

        // Default path: request user pre-approval first
        var paymentResponse = await _hubtelPaymentApi.InitializePreapproval(new MobileMoneyPreapprovalDto
            {
                CustomerMsisdn = dto.PhoneNumber,
                CallbackUrl = "http://213.165.245.124:5004/api/services/app/HubtelCallbackProcessor/Preapproval",
                Channel = dto.Channel,
            },
            dto.TenantId);

        if (paymentResponse is { ResponseCode: "0000" })
        {
            await CreateSubscriptionRecords(dto, paymentResponse.Data.ClientReferenceId, tenantInfo.Country, description, renewalEndDate);
        }

        return new SubscriptionInitializationResponse
        {
            PaymentMode = "PreapprovalRequested",
            Status = paymentResponse.Data?.PreapprovalStatus ?? "Pending",
            Title = "Preapproval Prompt Sent!",
            Message = "Please check your phone for a preapproval prompt to authorize automatic billing",
            HubtelPreApprovalId = paymentResponse.Data?.HubtelPreApprovalId ?? string.Empty,
            ClientReferenceId = paymentResponse.Data?.ClientReferenceId ?? string.Empty,
            OtpPrefix = paymentResponse.Data?.OtpPrefix ?? string.Empty,
            PreapprovalStatus = paymentResponse.Data?.PreapprovalStatus ?? "Pending"
        };
    }

    public async Task<object> CompleteSubscriptionPaymentViaDirectCharge(string reference, int tenantId)
    {
        await Task.Delay(TimeSpan.FromSeconds(3));
        
        var statusResponse = await _hubtelPaymentApi.TransactionStatusCheck(reference, tenantId);

        if (statusResponse is { Code: (int)RhyoliteOrbitConsts.PaymentStatus.Completed })
        {
            await CompleteRecurringSubscriptionPayment(reference, tenantId);
            
            return new SubscriptionInitializationResponse
            {
                PaymentMode = "DirectDebit",
                Status = "Completed",
                Title = "Subscription Auto-Charge Completed",
                Message = "Your subscription has been successfully renewed.",
                CheckoutUrl = string.Empty,
                ClientReference = reference
            };
        }
 
        return new SubscriptionInitializationResponse
        {
            PaymentMode = "DirectDebit",
            Status = "Failed",
            Title = "Subscription Auto-Charge Failed",
            Message = "Your subscription renewal failed. Please try again.",
            CheckoutUrl = string.Empty,
            ClientReference = reference
        };
    }

    public async Task<PreapprovalStatusCheckResponse> PreapprovalStatusCheck(string clientReference, int tenantId)
    {
        return await _hubtelPaymentApi.PreapprovalStatusCheck(clientReference, tenantId);
    }
    
    public async Task<PreapprovalCancellationResponse> CancelPreapproval(string customerMsisdn, int tenantId)
    {
        return await _hubtelPaymentApi.CancelPreapproval(customerMsisdn, tenantId);
    }

    public async Task<VerifyPreapprovalResponse> VerifyPreapproval(VerifyPreapprovalDto dto, int tenantId)
    {
        return await _hubtelPaymentApi.VerifyPreapproval(dto, tenantId);
    }

    public async Task<DirectDebitChargeResponse> RenewSubscription(DirectDebitChargeDto dto, int tenantId)
    {
        dto.PrimaryCallbackUrl = "http://213.165.245.124:5004/api/services/app/HubtelCallbackProcessor/DirectDebitCharge";
        
        return await _hubtelPaymentApi.DirectDebitCharge(dto, tenantId);
    }

    public async Task CompleteRecurringSubscriptionPayment(string reference, int tenantId)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            var subscriptionPayment = await _repositorySubscriptionPayment.FirstOrDefaultAsync(a => a.Reference == reference);
            if (subscriptionPayment != null)
            {
                subscriptionPayment.Status = (int)RhyoliteOrbitConsts.PaymentStatus.Completed;
                await _repositorySubscriptionPayment.UpdateAsync(subscriptionPayment);
            }

            var subscriptionRenewalHistory = await _repositorySubscriptionRenewalHistory.FirstOrDefaultAsync(a => a.Reference == reference);

            if (subscriptionRenewalHistory != null)
            {
                subscriptionRenewalHistory.Status = (int)RhyoliteOrbitConsts.SubscriptionRenewalStatus.Renewed;
                await _repositorySubscriptionRenewalHistory.UpdateAsync(subscriptionRenewalHistory);

                // Update tenant table — must disable tenant filters because Tenant lives in the host DB
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant))
                {
                    var tenant = await _repositoryTenant.FirstOrDefaultAsync(t => t.Id == tenantId);
                    if (tenant != null)
                    {
                        ApplySelectedModulesToTenant(
                            tenant,
                            subscriptionRenewalHistory.SelectedModules,
                            subscriptionRenewalHistory.RenewalStartDate,
                            subscriptionRenewalHistory.RenewalEndDate,
                            subscriptionRenewalHistory.Cycle);

                        tenant.IsDirectDebitEnabled = subscriptionRenewalHistory.IsDirectDebitRenewal;
                        if (subscriptionRenewalHistory.IsDirectDebitRenewal)
                        {
                            tenant.PhoneNumber = subscriptionRenewalHistory.CustomerMsisdn;
                            tenant.Network = subscriptionRenewalHistory.Network;
                        }
                        

                        await _repositoryTenant.UpdateAsync(tenant);
                    }
                }
            }
        }
    }
     
    
    
    public async Task<object> CompleteSubscriptionPayment(string reference)
    {
        if (string.IsNullOrWhiteSpace(reference))
        {
            throw new Abp.UI.UserFriendlyException("Reference is required.");
        }

        // small delay to allow provider to finalize processing
        await Task.Delay(TimeSpan.FromSeconds(3));

        var statusCheckResponse = await _hubtelPaymentApi.CheckoutStatusCheck(
            new BaseHubtelPaymentRequestDto { ClientReference = reference });

        var displayReference = ToDisplayReference(reference);

        if (statusCheckResponse is not { Code: (int)RhyoliteOrbitConsts.PaymentStatus.Completed })
        {
            return new
            {
                Status = nameof(RhyoliteOrbitConsts.PaymentStatus.Failed),
                Code = RhyoliteOrbitConsts.PaymentStatus.Failed,
                reference = displayReference,
            };
        }

        // Idempotent update: only move Pending -> Completed
        var subscriptionPayment = await _repositorySubscriptionPayment.FirstOrDefaultAsync(a => a.Reference == reference);

        if (subscriptionPayment == null)
        {
            // Payment completed at provider but we can't find our record
            // (still return success to user; optionally log/alert elsewhere)
            return new
            {
                Status = nameof(RhyoliteOrbitConsts.PaymentStatus.Completed),
                Code = RhyoliteOrbitConsts.PaymentStatus.Completed,
                reference = displayReference,
            };
        }

        if (subscriptionPayment.Status != (int)RhyoliteOrbitConsts.PaymentStatus.Completed)
        {
            subscriptionPayment.Status = (int)RhyoliteOrbitConsts.PaymentStatus.Completed;
            await _repositorySubscriptionPayment.UpdateAsync(subscriptionPayment);
        }

        return new
        {
            Status = nameof(RhyoliteOrbitConsts.PaymentStatus.Completed),
            Code = RhyoliteOrbitConsts.PaymentStatus.Completed,
            reference = displayReference,
        };
    }
    // for anonymous call backs
    public async Task<object> CompleteSubscriptionPayment(string reference, string phoneNumber, int tenantId)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            
            var statusCheckResponse = await _hubtelPaymentApi.CheckoutStatusCheck(new BaseHubtelPaymentRequestDto { ClientReference = reference});

            if (statusCheckResponse is { Code: (int)RhyoliteOrbitConsts.PaymentStatus.Completed })
            {
                var subscriptionPayment = await _repositorySubscriptionPayment.FirstOrDefaultAsync(a => a.Reference == reference);
                subscriptionPayment.Status = (int)RhyoliteOrbitConsts.PaymentStatus.Completed;
                subscriptionPayment.PhoneNumber = phoneNumber;
                await _repositorySubscriptionPayment.UpdateAsync(subscriptionPayment);

                var subscriptionRenewalHistory = await _repositorySubscriptionRenewalHistory.FirstOrDefaultAsync(a => a.Reference == reference);

                if (subscriptionRenewalHistory == null)
                {
                    throw new Abp.UI.UserFriendlyException("Subscription renewal history not found.");
                }

                subscriptionRenewalHistory.Status = (int)RhyoliteOrbitConsts.SubscriptionRenewalStatus.Renewed;
                subscriptionRenewalHistory.CustomerMsisdn = phoneNumber;

                await _repositorySubscriptionRenewalHistory.UpdateAsync(subscriptionRenewalHistory);

                // Update tenant table — must disable tenant filters because Tenant lives in the host DB
                using (_unitOfWorkManager.Current.DisableFilter(Abp.Domain.Uow.AbpDataFilters.MayHaveTenant))
                using (_unitOfWorkManager.Current.DisableFilter(Abp.Domain.Uow.AbpDataFilters.MustHaveTenant))
                {
                    var tenant = await _repositoryTenant.FirstOrDefaultAsync(t => t.Id == subscriptionPayment.TenantId);
                    if (tenant != null)
                    {
                        ApplySelectedModulesToTenant(
                            tenant,
                            subscriptionRenewalHistory.SelectedModules,
                            subscriptionRenewalHistory.RenewalStartDate,
                            subscriptionRenewalHistory.RenewalEndDate,
                            subscriptionRenewalHistory.Cycle);

                        // keep existing behavior for direct debit flag (if applicable)
                        tenant.IsDirectDebitEnabled = subscriptionRenewalHistory.IsDirectDebitRenewal;
                        if (subscriptionRenewalHistory.IsDirectDebitRenewal)
                        {
                            tenant.PhoneNumber = phoneNumber;
                            tenant.Network = subscriptionRenewalHistory.Network;
                        }
                        
                        await _repositoryTenant.UpdateAsync(tenant);
                    }
                }


                //schedule a quartz job for subscription renewal reminder or direct charge based on the billing cycle chosen by the customer
                
                var scheduleCron = $"0 {subscriptionRenewalHistory.RenewalEndDate.Minute} " +
                                   $"{subscriptionRenewalHistory.RenewalEndDate.Hour} " +
                                   $"{subscriptionRenewalHistory.RenewalEndDate.Day} " +
                                   $"{subscriptionRenewalHistory.RenewalEndDate:MMM} " +
                                   $"? " +
                                   $"{subscriptionRenewalHistory.RenewalEndDate.Year}";
                
                
                var payload = new JobModel
                {
                    Name = $"SUBSCRIPTION_RENEWAL_{tenantId}_{subscriptionRenewalHistory?.Id}",
                    Schedule = scheduleCron,
                    StartDate = subscriptionRenewalHistory.RenewalEndDate.AddMinutes(-5),
                    EndDate = subscriptionRenewalHistory.RenewalEndDate.AddMinutes(5),
                    Description = $"Subscription renewal processing for tenant {tenantId}",
                    CustomData = new CustomData
                    {
                        CallbackUrl = $"http://213.165.245.124:5004/api/services/app/Subscription/RunScheduledRenewal",
                        UniqueId = $"{subscriptionRenewalHistory.Id:N}_{tenantId}"
                    }
                };
                
                await _quartzJobScheduler.CreateJob(payload);
                
                return new
                {
                    PaymentStatus = nameof(RhyoliteOrbitConsts.PaymentStatus.Completed),
                    reference,
                };
            }
            
            return new
            {
                PaymentStatus = nameof(RhyoliteOrbitConsts.PaymentStatus.Failed),
                reference,
            };
        }

         
    }
    
    
    private static DateTime CalculateRenewalEndDate(DateTime startUtc, string cycle)
    {
        if (string.IsNullOrWhiteSpace(cycle))
        {
            throw new Abp.UI.UserFriendlyException("Billing cycle is required.");
        }

        switch (cycle.Trim().ToLowerInvariant())
        {
            case "quarterly":
                return startUtc.AddMonths(3);

            case "half-yearly":
            case "half yearly":
            case "semi-annual":
            case "semiannual":
                return startUtc.AddMonths(6);

            case "annual":
            case "yearly":
                return startUtc.AddYears(1);

            default:
                throw new Abp.UI.UserFriendlyException($"Unsupported billing cycle: {cycle}");
        }
    }
    
    private static string ToDisplayReference(string reference)
    {
        if (string.IsNullOrWhiteSpace(reference))
        {
            return string.Empty;
        }

        var first = reference.IndexOf('_');
        var last = reference.LastIndexOf('_');

        // Need at least two underscores, and something between them
        if (first < 0 || last <= first + 1)
        {
            return reference; // fallback: return as-is
        }

        return reference.Substring(first + 1, last - first - 1);
    }

    
    private void ApplySelectedModulesToTenant(
    Tenant tenant,
    IReadOnlyList<int> selectedModules,
    DateTime renewalStartDate,
    DateTime renewalEndDate,
    string cycle)
{
    if (tenant == null) throw new ArgumentNullException(nameof(tenant));
    if (selectedModules == null || selectedModules.Count == 0) return;

    var modules = _pricing.Modules ?? new List<ModulePriceOptions>();
    var byId = modules.ToDictionary(m => m.Id);

    var exchangeRate = _pricing.UsdExchangeRate <= 0 ? 1m : _pricing.UsdExchangeRate;
    var multiplier = GetCycleMultiplier(cycle);

    // Final (cycle-adjusted) local price for a module id; 0 if not configured.
    decimal Price(int id)
    {
        if (!byId.TryGetValue(id, out var m)) return 0m;

        var amount = m.PriceInUsd * exchangeRate * multiplier;
        return decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    var unique = selectedModules.Distinct().ToHashSet();

    // IMPORTANT: Adjust these ids to match your real module id configuration
    const int GlId = 3;
    const int PayrollId = 2;
    const int SchoolManagerId = 1;
    const int ApId = 4;
    const int ArId = 5;
    const int FixedAssetsId = 6;
    const int PropertyRentalsId = 12;
    const int PointOfSaleId = 7;
    const int HumanResourceId = 8;
    const int InventoryId = 9;
    const int SupplyChainId = 10;
    const int ManufacturingId = 11;
    const int DocumentManagementId = 13;
    const int CoreBankingId = 14;

    if (unique.Contains(GlId))
    {
        tenant.HasSubscribedToGl = true;
        tenant.GlSubscriptionStartDate = renewalStartDate;
        tenant.GlSubscriptionEndDate = renewalEndDate;
        tenant.GlBillingCycle = cycle;
        tenant.GlNextRenewalDate = renewalEndDate;

        tenant.GlSubscriptionFee = Price(GlId);
    }

    if (unique.Contains(PayrollId))
    {
        tenant.HasSubscribedToPayroll = true;
        tenant.PayrollSubscriptionStartDate = renewalStartDate;
        tenant.PayrollSubscriptionEndDate = renewalEndDate;
        tenant.PayrollBillingCycle = cycle;
        tenant.PayrollNextRenewalDate = renewalEndDate;

        tenant.PayrollSubscriptionFee = Price(PayrollId);
    }

    if (unique.Contains(SchoolManagerId))
    {
        tenant.HasSubscribedToSchoolManager = true;
        tenant.SchoolManagerSubscriptionStartDate = renewalStartDate;
        tenant.SchoolManagerSubscriptionEndDate = renewalEndDate;
        tenant.SchoolManagerBillingCycle = cycle;
        tenant.SchoolManagerNextRenewalDate = renewalEndDate;

        tenant.SchoolManagerSubscriptionFee = Price(SchoolManagerId);
    }

    if (unique.Contains(ApId))
    {
        tenant.HasSubscribedToAp = true;
        tenant.ApSubscriptionStartDate = renewalStartDate;
        tenant.ApSubscriptionEndDate = renewalEndDate;
        tenant.ApBillingCycle = cycle;
        tenant.ApNextRenewalDate = renewalEndDate;

        tenant.ApSubscriptionFee = Price(ApId);
    }

    if (unique.Contains(ArId))
    {
        tenant.HasSubscribedToAr = true;
        tenant.ArSubscriptionStartDate = renewalStartDate;
        tenant.ArSubscriptionEndDate = renewalEndDate;
        tenant.ArBillingCycle = cycle;
        tenant.ArNextRenewalDate = renewalEndDate;

        tenant.ArSubscriptionFee = Price(ArId);
    }

    if (unique.Contains(PropertyRentalsId))
    {
        tenant.HasSubscribedToPropertyRentals = true;
        tenant.PropertyRentalsSubscriptionStartDate = renewalStartDate;
        tenant.PropertyRentalsSubscriptionEndDate = renewalEndDate;
        tenant.PropertyRentalsBillingCycle = cycle;
        tenant.PropertyRentalsNextRenewalDate = renewalEndDate;

        tenant.PropertyRentalsSubscriptionFee = Price(PropertyRentalsId);
    }

    if (unique.Contains(FixedAssetsId))
    {
        tenant.HasSubscribedToFixedAssets = true;
        tenant.FixedAssetsSubscriptionStartDate = renewalStartDate;
        tenant.FixedAssetsSubscriptionEndDate = renewalEndDate;
        tenant.FixedAssetsBillingCycle = cycle;
        tenant.FixedAssetsNextRenewalDate = renewalEndDate;

        tenant.FixedAssetsSubscriptionFee = Price(FixedAssetsId);
    }
    
    if (unique.Contains(PointOfSaleId))
    {
        tenant.HasSubscribedToPointOfSale = true;
        tenant.PointOfSaleSubscriptionStartDate = renewalStartDate;
        tenant.PointOfSaleSubscriptionEndDate = renewalEndDate;
        tenant.PointOfSaleBillingCycle = cycle;
        tenant.PointOfSaleNextRenewalDate = renewalEndDate;

        tenant.PointOfSaleSubscriptionFee = Price(PointOfSaleId);
    }

    if (unique.Contains(HumanResourceId))
    {
        tenant.HasSubscribedToHumanResource = true;
        tenant.HumanResourceSubscriptionStartDate = renewalStartDate;
        tenant.HumanResourceSubscriptionEndDate = renewalEndDate;
        tenant.HumanResourceBillingCycle = cycle;
        tenant.HumanResourceNextRenewalDate = renewalEndDate;

        tenant.HumanResourceSubscriptionFee = Price(HumanResourceId);
    }

    if (unique.Contains(InventoryId))
    {
        tenant.HasSubscribedToInventory = true;
        tenant.InventorySubscriptionStartDate = renewalStartDate;
        tenant.InventorySubscriptionEndDate = renewalEndDate;
        tenant.InventoryBillingCycle = cycle;
        tenant.InventoryNextRenewalDate = renewalEndDate;

        tenant.InventorySubscriptionFee = Price(InventoryId);
    }

    if (unique.Contains(SupplyChainId))
    {
        tenant.HasSubscribedToSupplyChain = true;
        tenant.SupplyChainSubscriptionStartDate = renewalStartDate;
        tenant.SupplyChainSubscriptionEndDate = renewalEndDate;
        tenant.SupplyChainBillingCycle = cycle;
        tenant.SupplyChainNextRenewalDate = renewalEndDate;

        tenant.SupplyChainSubscriptionFee = Price(SupplyChainId);
    }

    if (unique.Contains(ManufacturingId))
    {
        tenant.HasSubscribedToManufacturing = true;
        tenant.ManufacturingSubscriptionStartDate = renewalStartDate;
        tenant.ManufacturingSubscriptionEndDate = renewalEndDate;
        tenant.ManufacturingBillingCycle = cycle;
        tenant.ManufacturingNextRenewalDate = renewalEndDate;

        tenant.ManufacturingSubscriptionFee = Price(ManufacturingId);
    }

    if (unique.Contains(DocumentManagementId))
    {
        tenant.HasSubscribedToDocumentManagement = true;
        tenant.DocumentManagementSubscriptionStartDate = renewalStartDate;
        tenant.DocumentManagementSubscriptionEndDate = renewalEndDate;
        tenant.DocumentManagementBillingCycle = cycle;
        tenant.DocumentManagementNextRenewalDate = renewalEndDate;

        tenant.DocumentManagementSubscriptionFee = Price(DocumentManagementId);
    }

    if (unique.Contains(CoreBankingId))
    {
        tenant.HasSubscribedToCoreBanking = true;
        tenant.CoreBankingSubscriptionStartDate = renewalStartDate;
        tenant.CoreBankingSubscriptionEndDate = renewalEndDate;
        tenant.CoreBankingBillingCycle = cycle;
        tenant.CoreBankingNextRenewalDate = renewalEndDate;

        tenant.CoreBankingSubscriptionFee = Price(CoreBankingId);
    }
}
    
    

    private decimal GetCycleMultiplier(string? cycle)
    {
        if (string.IsNullOrWhiteSpace(cycle))
        {
            throw new Abp.UI.UserFriendlyException("Billing cycle is required.");
        }

        var c = cycle.Trim().ToLowerInvariant();
        var m = _pricing.BillingCycleMultipliers ?? new BillingCycleMultipliers();

        return c switch
        {
            "quarterly" => m.Quarterly,
            "half-yearly" => m.HalfYearly,
            "half yearly" => m.HalfYearly,
            "annual" => m.Annual,
            "yearly" => m.Annual,
            _ => throw new Abp.UI.UserFriendlyException($"Unsupported billing cycle: {cycle}")
        };
    }
    
    

    public async Task<SubscriptionRenewalHistory> GetSubscriptionRenewalHistory(string reference, int tenantId)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            return await _repositorySubscriptionRenewalHistory.FirstOrDefaultAsync(a => a.Reference == reference);
        }
    }

    private (decimal totalAmount, string description, DateTime renewalEndDate) CalculateSubscriptionDetails(SubscriptionPaymentDto dto)
    {
        var selectedModuleIds = dto.Modules ?? new List<int>();
        if (selectedModuleIds.Count == 0)
        {
            throw new Abp.UI.UserFriendlyException("Please select at least one module.");
        }

        var configuredModules = _pricing.Modules ?? new List<ModulePriceOptions>();
        var configuredById = configuredModules.ToDictionary(m => m.Id);

        var unknownIds = selectedModuleIds.Where(id => !configuredById.ContainsKey(id)).Distinct().ToList();
        if (unknownIds.Count > 0)
        {
            throw new Abp.UI.UserFriendlyException($"Unknown module id(s): {string.Join(", ", unknownIds)}");
        }

        var exchangeRate = _pricing.UsdExchangeRate <= 0 ? 1m : _pricing.UsdExchangeRate;
        
        var multiplier = GetCycleMultiplier(dto.Cycle);

        var totalAmount = selectedModuleIds
            .Distinct()
            .Sum(id => decimal.Round(configuredById[id].PriceInUsd * exchangeRate * multiplier, 2, MidpointRounding.AwayFromZero));

        var selectedModuleNames = selectedModuleIds
            .Distinct()
            .Select(id => configuredById[id].Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList();

        const int maxNamesInDescription = 4;
        var shownNames = selectedModuleNames.Take(maxNamesInDescription).ToList();
        var remainingCount = selectedModuleNames.Count - shownNames.Count;

        var description = remainingCount > 0
            ? $"Rhyolite ERP - {string.Join(", ", shownNames)} (+{remainingCount} more)"
            : $"Rhyolite ERP - {string.Join(", ", shownNames)}";

        var renewalEndDate = CalculateRenewalEndDate(DateTime.UtcNow, dto.Cycle);

        return (totalAmount, description, renewalEndDate);
    }

    private async Task CreateSubscriptionRecords(SubscriptionPaymentDto dto, string reference, string country, string description, DateTime renewalEndDate)
    {
        var paymentId = Guid.NewGuid();
        await _repositorySubscriptionRenewalHistory.InsertAsync(new SubscriptionRenewalHistory
        {
            SubscriptionPaymentId = paymentId,
            Country = country,
            Reference = reference,
            Status = (int)RhyoliteOrbitConsts.SubscriptionRenewalStatus.Pending,
            Amount = dto.Amount,
            PaymentMethod = dto.PaymentMethod,
            Channel = dto.Channel,
            Network = dto.Network,
            CustomerMsisdn = dto.PhoneNumber,
            Combo = dto.Combo,
            Cycle = dto.Cycle,
            SelectedModules = dto.Modules,
            IsDirectDebitRenewal = dto.IsDirectDebitPayment,
            Notes = description,
            RenewalStartDate = DateTime.UtcNow,
            RenewalEndDate = renewalEndDate,
            TenantId = dto.TenantId
        });

        await _repositorySubscriptionPayment.InsertAsync(new SubscriptionPayment
        {
            Id = paymentId,
            PaymentMethod = dto.PaymentMethod,
            PhoneNumber = dto.PhoneNumber,
            Cycle = dto.Cycle,
            Amount = dto.Amount,
            Reference = reference,
            Status = (int)RhyoliteOrbitConsts.PaymentStatus.Pending,
            IsDirectDebitPayment = dto.IsDirectDebitPayment,
            Description = description,
            Channel = dto.Channel,
            Network = dto.Network,
            TenantId = dto.TenantId
        });
    }
}
