using System;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using RhyoliteOrbit.DomainServices.HubtelPayment.Dto;

namespace RhyoliteOrbit.DomainServices.HubtelPayment;

public class HubtelPaymentApi: DomainService, IHubtelPaymentApi
{
    private readonly IConfiguration _configuration;
    
    public HubtelPaymentApi(IConfiguration configuration)
    {
        _configuration = configuration;
         
    }
     
    public async Task<TransactionStatusCheckResponse> InitializeWebCheckout(WebCheckoutDto dto, int tenantId)
    {
        try
        {
            var reference = $"rhy_{DateTime.Now:yyyyMMddHHmmss}_{tenantId}";
            
            var client = new RestClient("https://payproxyapi.hubtel.com/items/initiate");
            var request = new RestRequest();
    
            // Add headers
            request.AddHeader("Content-Type", "application/json");
            string username = _configuration["HubtelPaymentApi:ApiIdOrUsername"];
            string password = _configuration["HubtelPaymentApi:ApiKeyOrPassword"];
            string merchantAccountNumber = _configuration["HubtelPaymentApi:MerchantAccountNumber"];
            
            // Add Basic Authentication
            var authBytes = System.Text.Encoding.UTF8.GetBytes($"{username}:{password}");
            var authHeader = Convert.ToBase64String(authBytes);
            request.AddHeader("Authorization", $"Basic {authHeader}");
            
            var payload = new
            {
                dto.TotalAmount,
                dto.ReturnUrl,
                merchantAccountNumber,
                dto.CallbackUrl,
                dto.CancellationUrl,
                dto.Description,
                ClientReference = reference
            };
            
            request.AddJsonBody(payload);
    
            // Set timeout
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(30000);
    
            // Execute request
            var response = await client.ExecutePostAsync<object>(request, cancellationTokenSource.Token);

            Logger.Info($"InitializeCheckout response => {JsonConvert.SerializeObject(response.Content)}, reference: {reference}");
            
            if (response.IsSuccessful && response.Content != null)
            {
                var result = JsonConvert.DeserializeObject<HubtelWebCheckoutResponse>(response.Content);
                
                Logger.Info($"InitializeCheckout response => {JsonConvert.SerializeObject(result.Data)}, reference: {reference}");
                
                return new TransactionStatusCheckResponse
                {
                    Status = result.ResponseCode == "0000"? "Success" : "Failure",
                    ClientReference = reference,
                    Code =  result.ResponseCode == "0000"? (int)RhyoliteOrbitConsts.PaymentStatus.Processing : (int)RhyoliteOrbitConsts.PaymentStatus.Cancelled,
                    CheckoutUrl = result.Data.CheckoutUrl,
                    Message = result.ResponseCode == "0000"? "Web Checkout initialized successfully." : "Transaction failed.",
                };
            }
    
            return new TransactionStatusCheckResponse
            {
                ClientReference = reference,
                Status = "Failure",
                Code = (int)RhyoliteOrbitConsts.PaymentStatus.Failed,
                CheckoutUrl = string.Empty,
                Message = "Failed to initiate web_checkout check. Please try again."
            };
        }
        catch (Exception ex)
        {
            Logger.Error("Error! Failed to initiate Hubtel web_checkout.", ex);

            return new TransactionStatusCheckResponse
            {
                ClientReference = string.Empty,
                Status = "Failure",
                Code = (int)RhyoliteOrbitConsts.PaymentStatus.Failed,
                CheckoutUrl = string.Empty,
                Message = "Failed to initiate web_checkout check. Please try again."
            };
        }

    }
    
    public async Task<TransactionStatusCheckResponse> CheckoutStatusCheck(BaseHubtelPaymentRequestDto dto)
    {
        try
        {
            string posSalesAccountId = _configuration["HubtelPaymentApi:PosSalesAccountId"];
            string username = _configuration["HubtelPaymentApi:ApiIdOrUsername"];
            string password = _configuration["HubtelPaymentApi:ApiKeyOrPassword"];
            
            var client = new RestClient($"https://api-txnstatus.hubtel.com/transactions/{posSalesAccountId}/status");
            var request = new RestRequest();
    
            // Add headers
            request.AddHeader("Content-Type", "application/json");
            
            // Add Basic Authentication
            var authBytes = System.Text.Encoding.UTF8.GetBytes($"{username}:{password}");
            var authHeader = Convert.ToBase64String(authBytes);
            request.AddHeader("Authorization", $"Basic {authHeader}");
            
            request.AddParameter("clientReference", dto.ClientReference);
    
            // Set timeout
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(30000);
    
            // Execute request
            var response = await client.GetAsync(request, cancellationTokenSource.Token);
    
            Logger.Info($"CheckoutStatusCheck response => {JsonConvert.SerializeObject(response.Content)}, reference: {dto.ClientReference}");

            if (response.IsSuccessful && response.Content != null)
            {
                var result = JsonConvert.DeserializeObject<HubtelPaymentApiResponse>(response.Content);
                
                Logger.Info($"CheckoutStatusCheck response => {JsonConvert.SerializeObject(result)}, reference: {dto.ClientReference}");
                
                return new TransactionStatusCheckResponse
                {
                    Status = result.ResponseCode == "0000"? "Success" : "Failure",
                    ClientReference = dto.ClientReference,
                    Code =  result.ResponseCode == "0000"? (int)RhyoliteOrbitConsts.PaymentStatus.Completed : (int)RhyoliteOrbitConsts.PaymentStatus.Failed,
                    Message = result.ResponseCode == "0000"? "Transaction processed successfully." : "Transaction failed.",
                };
            }
    
            return new TransactionStatusCheckResponse
            {
                ClientReference = dto.ClientReference,
                Status = "Failure",
                Code = 1,
                Message = "Failed to initiate transaction status check. Please try again."
            };
        }
        catch (Exception ex)
        {
            Logger.Error("Error! Failed to initiate Hubtel transaction_status_check.", ex);

            return new TransactionStatusCheckResponse
            {
                ClientReference = dto.ClientReference,
                Status = "Failure",
                Code = 1,
                Message = "Failed to initiate transaction status check. Please try again."
            };
        }
        
    }
    public async Task<DirectDebitResponse> InitializePreapproval(MobileMoneyPreapprovalDto dto, int tenantId)
    {
        try
        {
            var reference = $"rhy_{DateTime.Now:yyyyMMddHHmmss}_{tenantId}";
            
            string merchantAccountNumber = _configuration["HubtelPaymentApi:MerchantAccountNumber"];
            
            var client = new RestClient($"https://preapproval.hubtel.com/api/v2/merchant/{merchantAccountNumber}/preapproval/initiate");
            var request = new RestRequest();
    
            // Add headers
            request.AddHeader("Content-Type", "application/json");
            string username = _configuration["HubtelPaymentApi:ApiIdOrUsername"];
            string password = _configuration["HubtelPaymentApi:ApiKeyOrPassword"];
            
            
            // Add Basic Authentication
            var authBytes = System.Text.Encoding.UTF8.GetBytes($"{username}:{password}");
            var authHeader = Convert.ToBase64String(authBytes);
            request.AddHeader("Authorization", $"Basic {authHeader}");
            
            var payload = new
            { 
                dto.CustomerMsisdn,
                dto.CallbackUrl,
                dto.Channel,
                clientReferenceId = reference,
            };
            
            request.AddJsonBody(payload);
    
            // Set timeout
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(30000);
    
            // Execute request
            var response = await client.ExecutePostAsync<object>(request, cancellationTokenSource.Token);

            Logger.Info($"InitializeDirectDebit response => {JsonConvert.SerializeObject(response.Content)}, reference: {reference}");
            
            if (response.IsSuccessful && response.Content != null)
            {
                var result = JsonConvert.DeserializeObject<DirectDebitResponse>(response.Content);
                
                Logger.Info($"InitializeDirectDebit response => {JsonConvert.SerializeObject(result.Data)}, reference: {reference}");

                return result;
            }

            return null;
        }
        catch (Exception ex)
        {
            Logger.Error("Error! Failed to initiate Hubtel direct_debit.", ex);

            return null;
        }
    }
    
    public async Task<PreapprovalStatusCheckResponse> PreapprovalStatusCheck(string clientReference, int tenantId)
    {
        
        try
        {
            
            string merchantAccountNumber = _configuration["HubtelPaymentApi:MerchantAccountNumber"];
            
            var client = new RestClient($"https://preapproval.hubtel.com/api/v2/merchant/{merchantAccountNumber}/preapproval/{clientReference}/status");
            var request = new RestRequest();
    
            // Add headers
            request.AddHeader("Content-Type", "application/json");
            string username = _configuration["HubtelPaymentApi:ApiIdOrUsername"];
            string password = _configuration["HubtelPaymentApi:ApiKeyOrPassword"];
            
            
            // Add Basic Authentication
            var authBytes = System.Text.Encoding.UTF8.GetBytes($"{username}:{password}");
            var authHeader = Convert.ToBase64String(authBytes);
            request.AddHeader("Authorization", $"Basic {authHeader}");
    
            // Set timeout
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(30000);
    
            // Execute request
            var response = await client.ExecuteGetAsync<object>(request, cancellationTokenSource.Token);

            Logger.Info($"PreapprovalStatusCheck response => {JsonConvert.SerializeObject(response.Content)}");
            
            if (response.IsSuccessful && response.Content != null)
            {
                var result = JsonConvert.DeserializeObject<PreapprovalStatusCheckResponse>(response.Content);
                
                Logger.Info($"PreapprovalStatusCheck response => {JsonConvert.SerializeObject(result.Data)}");

                return result;
            }

            return null;
        }
        catch (Exception ex)
        {
            Logger.Error("Error! Preapproval status check failed", ex);

            return null;
        }
        
    }

    public async Task<PreapprovalCancellationResponse> CancelPreapproval(string customerMsisdn, int tenantId)
    {
        try
        {
            string merchantAccountNumber = _configuration["HubtelPaymentApi:MerchantAccountNumber"];
            
            var client = new RestClient($"https://preapproval.hubtel.com/api/v2/merchant/{merchantAccountNumber}/preapproval/{customerMsisdn}/cancel");
            var request = new RestRequest();
    
            // Add headers
            request.AddHeader("Content-Type", "application/json");
            string username = _configuration["HubtelPaymentApi:ApiIdOrUsername"];
            string password = _configuration["HubtelPaymentApi:ApiKeyOrPassword"];
            
            
            // Add Basic Authentication
            var authBytes = System.Text.Encoding.UTF8.GetBytes($"{username}:{password}");
            var authHeader = Convert.ToBase64String(authBytes);
            request.AddHeader("Authorization", $"Basic {authHeader}");
    
            // Set timeout
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(30000);
    
            // Execute request
            var response = await client.ExecuteGetAsync<object>(request, cancellationTokenSource.Token);

            Logger.Info($"CancelPreapproval response => {JsonConvert.SerializeObject(response.Content)}");
            
            if (response.IsSuccessful && response.Content != null)
            {
                var result = JsonConvert.DeserializeObject<PreapprovalCancellationResponse>(response.Content);
                
                Logger.Info($"CancelPreapproval response => {JsonConvert.SerializeObject(result.Data)}");

                return result;
            }

            return null;
        }
        catch (Exception ex)
        {
            Logger.Error("Error! Preapproval cancellation failed", ex);

            return null;
        }
        
    }

    public async Task<VerifyPreapprovalResponse> VerifyPreapproval(VerifyPreapprovalDto dto, int tenantId)
    {
        try
        {
            var reference = $"rhy_{DateTime.Now:yyyyMMddHHmmss}_{tenantId}";
            
            string merchantAccountNumber = _configuration["HubtelPaymentApi:MerchantAccountNumber"];
            
            var client = new RestClient($"https://preapproval.hubtel.com/api/v2/merchant/{merchantAccountNumber}/preapproval/verifyotp");
            var request = new RestRequest();
    
            // Add headers
            request.AddHeader("Content-Type", "application/json");
            string username = _configuration["HubtelPaymentApi:ApiIdOrUsername"];
            string password = _configuration["HubtelPaymentApi:ApiKeyOrPassword"];
            
            
            // Add Basic Authentication
            var authBytes = System.Text.Encoding.UTF8.GetBytes($"{username}:{password}");
            var authHeader = Convert.ToBase64String(authBytes);
            request.AddHeader("Authorization", $"Basic {authHeader}");
            
            var payload = new
            { 
                dto.CustomerMsisdn,
                dto.HubtelPreApprovalId,
                clientReferenceId = reference,
                
            };
            
            request.AddJsonBody(payload);
    
            // Set timeout
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(30000);
    
            // Execute request
            var response = await client.ExecutePostAsync<object>(request, cancellationTokenSource.Token);

            Logger.Info($"VerifyPreapproval response => {JsonConvert.SerializeObject(response.Content)}, reference: {reference}");
            
            if (response.IsSuccessful && response.Content != null)
            {
                var result = JsonConvert.DeserializeObject<VerifyPreapprovalResponse>(response.Content);
                
                Logger.Info($"VerifyPreapproval response => {JsonConvert.SerializeObject(result.Data)}, reference: {reference}");

                return result;
            }

            return null;
        }
        catch (Exception ex)
        {
            Logger.Error("Error! Failed to initiate Hubtel direct_debit.", ex);

            return null;
        }
    }
    public async Task<DirectDebitResponse> ReactivatePreapproval(string customerMsisdn, int tenantId)
    {
        try
        {
            string merchantAccountNumber = _configuration["HubtelPaymentApi:MerchantAccountNumber"];
            
            var client = new RestClient($"https://preapproval.hubtel.com/api/v2/merchant/{merchantAccountNumber}/preapproval/reactivate");
            var request = new RestRequest();
    
            // Add headers
            request.AddHeader("Content-Type", "application/json");
            string username = _configuration["HubtelPaymentApi:ApiIdOrUsername"];
            string password = _configuration["HubtelPaymentApi:ApiKeyOrPassword"];
            
            
            // Add Basic Authentication
            var authBytes = System.Text.Encoding.UTF8.GetBytes($"{username}:{password}");
            var authHeader = Convert.ToBase64String(authBytes);
            request.AddHeader("Authorization", $"Basic {authHeader}");
    
            // Set timeout
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(30000);
    
            // Execute request
            var response = await client.ExecuteGetAsync<object>(request, cancellationTokenSource.Token);

            Logger.Info($"ReactivatePreapproval response => {JsonConvert.SerializeObject(response.Content)}");
            
            if (response.IsSuccessful && response.Content != null)
            {
                var result = JsonConvert.DeserializeObject<DirectDebitResponse>(response.Content);
                
                Logger.Info($"ReactivatePreapproval response => {JsonConvert.SerializeObject(result.Data)}");

                return result;
            }

            return null;
        }
        catch (Exception ex)
        {
            Logger.Error("Error! Preapproval activation failed", ex);

            return null;
        }
        
    }

    public async Task<TransactionStatusCheckResponse> TransactionStatusCheck(string hubtelTransactionId, int tenantId)
    {
        try
        {
            string posSalesAccountId = _configuration["HubtelPaymentApi:PosSalesAccountId"];
            string username = _configuration["HubtelPaymentApi:ApiIdOrUsername"];
            string password = _configuration["HubtelPaymentApi:ApiKeyOrPassword"];
            
            var client = new RestClient($"https://api-txnstatus.hubtel.com/transactions/{posSalesAccountId}/status");
            var request = new RestRequest();
    
            // Add headers
            request.AddHeader("Content-Type", "application/json");
            
            // Add Basic Authentication
            var authBytes = System.Text.Encoding.UTF8.GetBytes($"{username}:{password}");
            var authHeader = Convert.ToBase64String(authBytes);
            request.AddHeader("Authorization", $"Basic {authHeader}");
            
            request.AddParameter("hubtelTransactionId", hubtelTransactionId);
    
            // Set timeout
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(30000);
    
            // Execute request
            var response = await client.GetAsync(request, cancellationTokenSource.Token);
    
            Logger.Info($"TransactionStatusCheck response => {JsonConvert.SerializeObject(response.Content)}, hubtelTransactionId: {hubtelTransactionId}");

            if (response.IsSuccessful && response.Content != null)
            {
                var result = JsonConvert.DeserializeObject<HubtelPaymentApiResponse>(response.Content);
                
                Logger.Info($"TransactionStatusCheck response => {JsonConvert.SerializeObject(result)}, hubtelTransactionId: {hubtelTransactionId}");
                
                return new TransactionStatusCheckResponse
                {
                    Status = result.ResponseCode == "0000"? "Success" : "Failure",
                    ClientReference = hubtelTransactionId,
                    Code =  result.ResponseCode == "0000" ? (int)RhyoliteOrbitConsts.PaymentStatus.Completed : (int)RhyoliteOrbitConsts.PaymentStatus.Failed,
                    Message = result.ResponseCode == "0000"? "Transaction processed successfully." : "Transaction failed.",
                };
            }
    
            return new TransactionStatusCheckResponse
            {
                ClientReference = hubtelTransactionId,
                Status = "Failure",
                Code = 1,
                Message = "Failed to initiate transaction status check. Please try again."
            };
        }
        catch (Exception ex)
        {
            Logger.Error("Error! Failed to initiate Hubtel transaction_status_check.", ex);

            return new TransactionStatusCheckResponse
            {
                ClientReference = hubtelTransactionId,
                Status = "Failure",
                Code = 1,
                Message = "Failed to initiate transaction status check. Please try again."
            };
        }
        
    }
    public async Task<DirectDebitChargeResponse> DirectDebitCharge(DirectDebitChargeDto dto, int tenantId)
    {
        try
        {
            var reference = $"rhy_{DateTime.Now:yyyyMMddHHmmss}_{tenantId}";
            
            string merchantAccountNumber = _configuration["HubtelPaymentApi:MerchantAccountNumber"];
            
            var client = new RestClient($"https://rmp.hubtel.com/merchantaccount/merchants/{merchantAccountNumber}/receive/mobilemoney");
            var request = new RestRequest();
    
            // Add headers
            request.AddHeader("Content-Type", "application/json");
            string username = _configuration["HubtelPaymentApi:ApiIdOrUsername"];
            string password = _configuration["HubtelPaymentApi:ApiKeyOrPassword"];
            
            
            // Add Basic Authentication
            var authBytes = System.Text.Encoding.UTF8.GetBytes($"{username}:{password}");
            var authHeader = Convert.ToBase64String(authBytes);
            request.AddHeader("Authorization", $"Basic {authHeader}");
            
            var payload = new
            { 
                dto.CustomerMsisdn,
                dto.PrimaryCallbackUrl,
                dto.Channel,
                clientReferenceId = reference,
                dto.CustomerEmail,
                dto.CustomerName,
                dto.Amount, 
                dto.Description
                
            };
            
            request.AddJsonBody(payload);
    
            // Set timeout
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(30000);
    
            // Execute request
            var response = await client.ExecutePostAsync<object>(request, cancellationTokenSource.Token);

            Logger.Info($"DirectDebitCharge response => {JsonConvert.SerializeObject(response.Content)}, reference: {reference}");
            
            if (response.IsSuccessful && response.Content != null)
            {
                var result = JsonConvert.DeserializeObject<DirectDebitChargeResponse>(response.Content);
                
                Logger.Info($"DirectDebitCharge response => {JsonConvert.SerializeObject(result.Data)}, reference: {reference}");

                return result;
            }

            return null;
        }
        catch (Exception ex)
        {
            Logger.Error("Error! Failed to initiate Hubtel direct_debit.", ex);

            return null;
        }
        
        
    }
    
   
}