using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.DomainServices.HubtelPayment.Dto;

namespace RhyoliteOrbit.DomainServices.HubtelPayment;

public interface IHubtelPaymentApi: IDomainService
{
    //Task<object> InitializeDirectReceiveMoney(DirectReceiveMoneyDto dto);
    Task<TransactionStatusCheckResponse> InitializeWebCheckout(WebCheckoutDto dto, int tenantId);
    Task<DirectDebitResponse> InitializeDirectDebit(DirectDebitMobileMoneyDto dto, int tenantId);
    Task<PreapprovalStatusCheckResponse> PreapprovalStatusCheck(string clientReference, int tenantId);
    
    Task<PreapprovalCancellationResponse> CancelPreapproval(string customerMsisdn, int tenantId);
    
    Task<DirectDebitChargeResponse> DirectDebitCharge(DirectDebitChargeDto dto, int tenantId);
    
    //Task<TransactionStatusCheckResponse> DirectReceiveMoneyStatusCheck(TransactionStatusCheckRequest dto);
    Task<TransactionStatusCheckResponse> CheckoutStatusCheck(BaseHubtelPaymentRequestDto dto);
    //Task<TransactionStatusCheckResponse> InitializeDirectSendMobileMoney(DirectSendMobileMoneyDto dto);
    //Task<TransactionStatusCheckResponse> DirectSendMobileMoneyStatusCheck(TransactionStatusCheckRequest dto);
    //Task<TransactionStatusCheckResponse> InitializeDirectSendToBank(DirectSendToBankDto dto);
    //Task<TransactionStatusCheckResponse> DirectSendToBankStatusCheck(TransactionStatusCheckRequest dto);
}