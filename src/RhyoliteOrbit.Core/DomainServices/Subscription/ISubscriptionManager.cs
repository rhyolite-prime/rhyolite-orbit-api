using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.DomainServices.HubtelPayment.Dto;
using RhyoliteOrbit.DomainServices.Subscription.Dto;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Subscription;

public interface ISubscriptionManager : IDomainService
{
    Task<object> ListPayments(int pageNo, int pageSize, string query);
    Task<SubscriptionInitializationResponse> InitializeSubscriptionPayment(SubscriptionPaymentDto dto);
    Task<SubscriptionInitializationResponse> InitializeRecurringSubscriptionPayment(SubscriptionPaymentDto dto);

    Task<PreapprovalStatusCheckResponse> PreapprovalStatusCheck(string clientReference, int tenantId);
    Task<VerifyPreapprovalResponse> VerifyPreapproval(VerifyPreapprovalDto dto, int tenantId);
    Task<DirectDebitChargeResponse> RenewSubscription(DirectDebitChargeDto dto, int tenantId);
    Task<PreapprovalCancellationResponse> CancelPreapproval(string customerMsisdn, int tenantId);
    Task<object> CompleteSubscriptionPayment(string reference);
    Task<object> CompleteSubscriptionPayment(string reference, string phoneNumber, int tenantId);
    Task CompleteRecurringSubscriptionPayment(string reference, int tenantId);
    Task<object> CompleteSubscriptionPaymentViaDirectCharge(string reference, int tenantId);
    Task<SubscriptionRenewalHistory> GetSubscriptionRenewalHistory(string reference, int tenantId);
}