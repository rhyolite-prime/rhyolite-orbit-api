using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.DomainServices.HubtelPayment.Dto;
using RhyoliteOrbit.DomainServices.Subscription.Dto;

namespace RhyoliteOrbit.DomainServices.Subscription;

public interface ISubscriptionManager: IDomainService
{
    Task<object> ListPayments(int pageNo, int pageSize, string query);
    Task<object> InitializeSubscriptionPayment(SubscriptionPaymentDto dto);
    Task<object> InitializeRecurringSubscriptionPayment(SubscriptionPaymentDto dto);
    
    Task<PreapprovalStatusCheckResponse> PreapprovalStatusCheck(string clientReference, int tenantId);
    Task<DirectDebitChargeResponse> RenewSubscription(DirectDebitChargeDto dto, int tenantId);
    Task<PreapprovalCancellationResponse> CancelPreapproval(string customerMsisdn, int tenantId);
    Task<object> CompleteSubscriptionPayment(string reference);
    Task<object> CompleteSubscriptionPayment(string reference, string phoneNumber, int tenantId);
}