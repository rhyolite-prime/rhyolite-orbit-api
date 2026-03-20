using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.DomainServices.HubtelPayment.Dto;

namespace RhyoliteOrbit.DomainServices.HubtelPayment;

public interface IHubtelPaymentApi: IDomainService
{
    
    Task<TransactionStatusCheckResponse> InitializeWebCheckout(WebCheckoutDto dto, int tenantId);
    Task<TransactionStatusCheckResponse> CheckoutStatusCheck(BaseHubtelPaymentRequestDto dto);
    Task<DirectDebitResponse> InitializePreapproval(MobileMoneyPreapprovalDto dto, int tenantId);
    Task<PreapprovalStatusCheckResponse> PreapprovalStatusCheck(string clientReference, int tenantId);
    
    Task<PreapprovalCancellationResponse> CancelPreapproval(string customerMsisdn, int tenantId);

    Task<VerifyPreapprovalResponse> VerifyPreapproval(VerifyPreapprovalDto dto, int tenantId);

    Task<DirectDebitResponse> ReactivatePreapproval(string customerMsisdn, int tenantId);

    Task<TransactionStatusCheckResponse> TransactionStatusCheck(string clientReference, int tenantId);
    
    Task<DirectDebitChargeResponse> DirectDebitCharge(DirectDebitChargeDto dto, int tenantId);
    
    //Task<object> InitializeDirectReceiveMoney(DirectReceiveMoneyDto dto);
    
    //Task<TransactionStatusCheckResponse> DirectReceiveMoneyStatusCheck(TransactionStatusCheckRequest dto);
    
}