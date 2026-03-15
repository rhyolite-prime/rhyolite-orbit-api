using System;

namespace RhyoliteOrbit.DomainServices.HubtelPayment.Dto;

public class BaseHubtelPaymentRequestDto
{
    public string ClientReference { get; set; }
}

public class BaseTransactionStatusCheckResponse
{
    public string Message { get; set; }
    public string ResponseCode { get; set; }
    public string Status { get; set; }
}

public class DirectReceiveMoneyDto
{
    public string CustomerName { get; set; }
    public string CustomerMsisdn { get; set; }
    public string CustomerEmail { get; set; }
    public decimal Amount { get; set; }
    public string PrimaryCallbackUrl { get; set; }
    public string Description { get; set; }
    public string Provider { get; set; }
    // auth
    public string CbsBaseUrl { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string PosSalesAccountId { get; set; }
    //dse-api
    public string TransactionType { get; set; }
    public string AccountType { get; set; } //F,K,U,S,L
    public string IntegrationChannel { get; set; }
    public string IntegratingService { get; set; }
    public string KiddyAccountSerialNo { get; set; }
    public int Pin { get; set; }
    public string TenantName { get; set; }
    
}




public class TransactionStatusCheckRequest: BaseHubtelPaymentRequestDto
{
    
    public string CbsBaseUrl { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string PosSalesAccountId { get; set; }
    public string PrepaidAccountId { get; set; }
}

public class DirectDebitMobileMoneyDto 
{
    public string ClientReferenceId { get; set; }
    public string CustomerMsisdn { get; set; }
    public string Channel { get; set; }
    public string CallbackUrl { get; set; }
    
}

public class DirectDebitChargeDto
{
    public string CustomerMsisdn { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerName { get; set; }
    public string Channel { get; set; }
    public string PrimaryCallbackUrl { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    
}
public class DirectSendMobileMoneyDto : BaseHubtelPaymentRequestDto
{
    public string RecipientName { get; set; }
    public string RecipientMsisdn { get; set; }
    public string SenderMsisdn { get; set; }
    
    public string CustomerEmail { get; set; }
    public decimal Amount { get; set; }
    public string PrimaryCallbackUrl { get; set; }
    public string Description { get; set; }
    public string Provider { get; set; }
    // auth
    public string CbsBaseUrl { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string PosSalesAccountId { get; set; }
    
    public string PrepaidAccountId { get; set; }
    //dse-api
    public string TransactionType { get; set; }
    public string IntegrationChannel { get; set; }
    public string IntegratingService { get; set; } //erp
    
}

public class WebCheckoutDto
{
    public string PhoneNo { get; set; }
    public decimal TotalAmount { get; set; }
    public string Description { get; set; }
    public string CallbackUrl { get; set; }
    public string ReturnUrl { get; set; }
    public string CancellationUrl { get; set; }
    
     
}

public class DirectSendToBankDto : BaseHubtelPaymentRequestDto
{
    public decimal Amount { get; set; }
    public string BankName { get; set; }
    public string BankBranch { get; set; }
    public string BankBranchCode { get; set; }
    public string BankAccountNumber { get; set; }
    public string BankAccountName { get; set; }
    public string PrimaryCallbackUrl { get; set; }
    public string Description { get; set; }
    public string RecipientPhoneNumber { get; set; }
    public string SenderMsisdn { get; set; }
    // auth
    public string CbsBaseUrl { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string PrepaidAccountId { get; set; }
    //dse-api
    public string TransactionType { get; set; }
    public string AccountType { get; set; } //F,K,U,S,L
    public string IntegrationChannel { get; set; }
    public string IntegratingService { get; set; }
    
    public string KiddyAccountSerialNo { get; set; }
    public int Pin { get; set; }
    public string TenantName { get; set; }
}

public class HubtelPaymentApiResponse: BaseTransactionStatusCheckResponse
{
    public TransactionStatusData Data { get; set; }
}

public class HubtelWebCheckoutResponse: BaseTransactionStatusCheckResponse
{
    public string Status { get; set; }
    public WebCheckoutData Data { get; set; }
}

public class WebCheckoutData
{
    public string CheckoutUrl { get; set; }
    public string CheckoutId { get; set; }
    public string ClientReference { get; set; }
    public string CheckoutDirectUrl { get; set; }
     
}

public class HubtelWebCheckoutCallbackResponse: BaseTransactionStatusCheckResponse
{
    public HubtelWebCheckoutCallbackData Data { get; set; }
}

public class HubtelPreapprovalCallbackResponse
{
    public string CustomerMsisdn { get; set; }

    public string VerificationType { get; set; }
    
    public string PreapprovalStatus { get; set; }
    
    public string HubtelPreapprovalId { get; set; }
    
    public string ClientReferenceId { get; set; }
}

public class DirectDebitChargeCallbackResponse: BaseTransactionStatusCheckResponse
{
    public DirectDebitChargeCallbackResponseData Data { get; set; }
}

public class DirectDebitChargeCallbackResponseData
{
    public decimal Amount { get; set; }
    public decimal Charges { get; set; }
    public decimal AmountAfterCharges { get; set; }
    public string Description { get; set; }
    public string ClientReference { get; set; }
    public string TransactionId { get; set; }
    public string ExternalTransactionId { get; set; }
    public decimal AmountCharged { get; set; }
    public string OrderId { get; set; }
    public string PaymentDate { get; set; }
}

public class HubtelWebCheckoutCallbackData
{
    public string CheckoutId { get; set; }

    public string SalesInvoiceId { get; set; }
    public string ClientReference { get; set; }

    public string Status { get; set; }

    public decimal Amount { get; set; }

    public string CustomerPhoneNumber { get; set; }

    public string Description { get; set; }
    
}

public class TransactionStatusData
{
    public string TransactionId { get; set; }
    public string NetworkTransactionId { get; set; }
    public decimal Amount { get; set; }
    public decimal Fees { get; set; }
    public string ClientReference { get; set; }
    public string Channel { get; set; }
    public string CustomerNumber { get; set; }
    public string TransactionStatus { get; set; }
    public string CreatedAt { get; set; }
}

public class HubtelBankChannel
{
    public string BankChannel { get; set; }
    public string BankCode { get; set; }
}

public class TransactionStatusCheckResponse
{
    public int Code { get; set; }
    public string Status { get; set; }
    public string ClientReference { get; set; }
    public string Message { get; set; }
    public string CheckoutUrl { get; set; }
}

public class DirectDebitResponse: BaseTransactionStatusCheckResponse
{
    public DirectDebitResponseData Data { get; set; }
    
}

public class DirectDebitChargeResponse: BaseTransactionStatusCheckResponse
{
    public DirectDebitChargeResponseData Data { get; set; }
}


public class DirectDebitChargeResponseData
{
    public string TransactionId { get; set; }
    public string Description { get; set; }
    public string ClientReference { get; set; }
    public decimal Amount { get; set; }
    public decimal Charges { get; set; }
    public decimal AmountAfterCharges { get; set; }
    public decimal AmountCharged { get; set; }
}
public class DirectDebitResponseData
{
    public string HubtelPreApprovalId { get; set; }
    public string ClientReferenceId { get; set; }
    public string VerificationType { get; set; }
    public string OtpPrefix { get; set; }
    public string PreapprovalStatus { get; set; }
    
}

public class PreapprovalStatusCheckResponse: BaseTransactionStatusCheckResponse
{
    public PreapprovalStatusCheckResponseData Data { get; set; }
}

public class PreapprovalCancellationResponse: BaseTransactionStatusCheckResponse
{
    public bool Data { get; set; }
    
}

public class PreapprovalStatusCheckResponseData
{
    public string CustomerMsisdn { get; set; }
    public string VerificationType { get; set; }
    public string PreapprovalStatus { get; set; }
    public string HubtelPreapprovalId { get; set; }
    public string ClientReferenceId { get; set; }
    public string CreatedAt { get; set; }
}