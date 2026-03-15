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
using RhyoliteOrbit.DomainServices.Subscription.Dto;
using RhyoliteOrbit.Models.Shared;
using RhyoliteOrbit.MultiTenancy;

namespace RhyoliteOrbit.DomainServices.Subscription;

public class SubscriptionManager: DomainService, ISubscriptionManager
{
    private readonly IRepository<SubscriptionPayment,Guid> _repositorySubscriptionPayment;
    private readonly IRepository<SubscriptionRenewalHistory,Guid> _repositorySubscriptionRenewalHistory;
    private readonly IHubtelPaymentApi  _hubtelPaymentApi;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly TenantManager _tenantManager;
    private readonly SubscriptionPricingOptions _pricing;


    public SubscriptionManager(IRepository<SubscriptionPayment, Guid> repositorySubscriptionPayment, IHubtelPaymentApi hubtelPaymentApi, IUnitOfWorkManager unitOfWorkManager, TenantManager tenantManager, IRepository<SubscriptionRenewalHistory, Guid> repositorySubscriptionRenewalHistory, IOptions<SubscriptionPricingOptions> pricingOptions)
    {
        _repositorySubscriptionPayment = repositorySubscriptionPayment;
        _hubtelPaymentApi = hubtelPaymentApi;
        _unitOfWorkManager = unitOfWorkManager;
        _tenantManager = tenantManager;
        _repositorySubscriptionRenewalHistory = repositorySubscriptionRenewalHistory;
        _pricing = pricingOptions.Value;

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
   
    public async Task<object> InitializeSubscriptionPayment(SubscriptionPaymentDto dto)
    {

        //retrieve tenant info to get tenant info
        var tenantInfo = await _tenantManager.FindByIdAsync(dto.TenantId);
        //compute cost for selected modules
        
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

        var totalUsd = selectedModuleIds.Sum(id => configuredById[id].PriceInUsd);
        var exchangeRate = _pricing.UsdExchangeRate <= 0 ? 1m : _pricing.UsdExchangeRate;

        var totalAmount = decimal.Round(totalUsd * exchangeRate, 2, MidpointRounding.AwayFromZero);

        dto.Amount = totalAmount;
        //dto.Amount = 1m;
        
        var selectedModuleNames = selectedModuleIds
            .Distinct()
            .Select(id => configuredById[id].Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList();

        const int maxNamesInDescription = 4;
        var shownNames = selectedModuleNames.Take(maxNamesInDescription).ToList();
        var remainingCount = selectedModuleNames.Count - shownNames.Count;

        var description = remainingCount > 0
            ? $"Rhyolite ERP Subscription - {string.Join(", ", shownNames)} (+{remainingCount} more)"
            : $"Rhyolite ERP Subscription - {string.Join(", ", shownNames)}";

        var renewalStartDate = DateTime.UtcNow;
        var renewalEndDate = CalculateRenewalEndDate(renewalStartDate, dto.Cycle);
        
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
            // insert into subscription renewal table

            var paymentId = Guid.NewGuid();
            await _repositorySubscriptionRenewalHistory.InsertAsync(new SubscriptionRenewalHistory
            {
                SubscriptionPaymentId = paymentId, 
                Country = tenantInfo.Country,
                Reference = paymentResponse.ClientReference, 
                Status = (int)RhyoliteOrbitConsts.SubscriptionRenewalStatus.Pending, 
                Amount = totalAmount, 
                PaymentMethod = string.Empty,
                Channel = string.Empty,
                CustomerMsisdn = string.Empty, 
                Combo = dto.Combo, 
                Cycle = dto.Cycle, 
                SelectedModules = dto.Modules,
                IsDirectDebitRenewal = dto.IsDirectDebitPayment, 
                Notes = description, 
                RenewalStartDate = DateTime.UtcNow, 
                RenewalEndDate = renewalEndDate,

            });
            
            await _repositorySubscriptionPayment.InsertAsync(new SubscriptionPayment
            {
                Id = paymentId,
                PaymentMethod = dto.PaymentMethod,
                PhoneNumber =  dto.PhoneNumber,
                Cycle = dto.Cycle,
                Amount = dto.Amount,
                Reference = paymentResponse.ClientReference,
                Status = (int)RhyoliteOrbitConsts.PaymentStatus.Pending,
                IsDirectDebitPayment =  dto.IsDirectDebitPayment,
                Description = description

            });
        }
        
        return paymentResponse;
    }
    
    public async Task<object> InitializeRecurringSubscriptionPayment(SubscriptionPaymentDto dto)
    {
        if (dto.Channel.Equals("mtn"))
        {
            dto.Channel = "mtn-gh-direct-debit";
        }
        
        if (dto.Channel.Equals("telecel"))
        {
            dto.Channel = "vodafone-gh-direct-debit";
        }
        
        var paymentResponse = await _hubtelPaymentApi.InitializeDirectDebit(new DirectDebitMobileMoneyDto 
            { 
                CustomerMsisdn = dto.PhoneNumber,
                CallbackUrl = "http://213.165.245.124:5004/api/services/app/HubtelCallbackProcessor/Preapproval", 
                Channel = dto.Channel,
            }, 
            dto.TenantId);
        
        return paymentResponse;
    }

    public async Task<PreapprovalStatusCheckResponse> PreapprovalStatusCheck(string clientReference, int tenantId)
    {
        return await _hubtelPaymentApi.PreapprovalStatusCheck(clientReference, tenantId);
    }

    public async Task<PreapprovalCancellationResponse> CancelPreapproval(string customerMsisdn, int tenantId)
    {
        return await _hubtelPaymentApi.CancelPreapproval(customerMsisdn, tenantId);
    }

    public async Task<DirectDebitChargeResponse> RenewSubscription(DirectDebitChargeDto dto, int tenantId)
    {
        
        //record debit in subscription renewal history table
        
        dto.PrimaryCallbackUrl =
            "http://213.165.245.124:5004/api/services/app/HubtelCallbackProcessor/DirectDebitCharge";
        
        return await _hubtelPaymentApi.DirectDebitCharge(dto, tenantId);
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
            var statusCheckResponse = await _hubtelPaymentApi.CheckoutStatusCheck(new BaseHubtelPaymentRequestDto { ClientReference = reference});

            if (statusCheckResponse is { Code: (int)RhyoliteOrbitConsts.PaymentStatus.Completed })
            {
                var subscriptionPayment = await _repositorySubscriptionPayment.FirstOrDefaultAsync(a => a.Reference == reference);
                subscriptionPayment.Status = (int)RhyoliteOrbitConsts.PaymentStatus.Completed;
                subscriptionPayment.PhoneNumber = phoneNumber;
                await _repositorySubscriptionPayment.UpdateAsync(subscriptionPayment);

                var subscriptionRenewalHistory = await _repositorySubscriptionRenewalHistory.FirstOrDefaultAsync(a => a.Reference == reference);
                if (subscriptionRenewalHistory is not null)
                {
                    subscriptionRenewalHistory.Status = (int)RhyoliteOrbitConsts.SubscriptionRenewalStatus.Renewed;
                    subscriptionRenewalHistory.CustomerMsisdn = phoneNumber;

                    await _repositorySubscriptionRenewalHistory.UpdateAsync(subscriptionRenewalHistory);
                }
                
                // Update tenant table with the required subscription details
                var tenant = await _tenantManager.FindByIdAsync(subscriptionPayment.TenantId);
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
                    tenant.PhoneNumber =  phoneNumber;

                    await _tenantManager.UpdateAsync(tenant);

                }


                //schedule a quartz job for subscription renewal reminder or direct charge based on the billing cycle chosen by the customer
                
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

    // Helper to get price safely (0 if not configured)
    decimal Price(int id) => byId.TryGetValue(id, out var m) ? m.PriceInUsd : 0m;
    
    var unique = selectedModules.Distinct().ToHashSet();

    // IMPORTANT: Adjust these ids to match your real module id configuration
    const int GlId = 3;
    const int PayrollId = 2;
    const int SchoolManagerId = 1;
    const int ApId = 4;
    const int ArId = 5;
    const int FixedAssetsId = 6;
    const int PropertyRentalsId = 12;

    if (unique.Contains(GlId))
    {
        tenant.HasSubscribedToGl = true;
        tenant.GlSubscriptionStartDate = renewalStartDate;
        tenant.GlSubscriptionEndDate = renewalEndDate;
        tenant.GlBillingCycle = cycle;
        tenant.GlNextRenewalDate = renewalEndDate;
        var gl = _pricing.Modules.FirstOrDefault(a=> a.Id == GlId);
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
}

    
}