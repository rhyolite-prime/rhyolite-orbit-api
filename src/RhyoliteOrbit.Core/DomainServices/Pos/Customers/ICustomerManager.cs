using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.Customers;

public interface ICustomerManager: IDomainService
{
    Task Create(PosCustomer customer);
    Task Update(PosCustomer customer);
    Task<object> GetAll(int pageNo, int pageSize, string query);
    Task<PosCustomer> GetByPhoneNumber(string phoneNumber);

    // Financial & Credit Operations
    Task AdjustWalletBalance(Guid customerId, decimal amount, string referenceReason);
    Task UpdateCreditProfile(Guid customerId, decimal newLimit, int creditTermDays, bool isCreditAllowed);
    Task SetCreditBlockStatus(Guid customerId, bool isBlocked, string reason);
    Task<bool> CanPurchaseOnCredit(Guid customerId, decimal requestedCreditAmount);

    // Loyalty & Metrics
    Task AddLoyaltyPoints(Guid customerId, int points);
    Task DeductLoyaltyPoints(Guid customerId, int points);
    Task RecordPurchase(Guid customerId, decimal purchaseAmount);

    // System Cleanup / Anonymization
    Task DeactivateCustomer(Guid customerId);
    
    //reserved for integration
    Task RegisterCustomer(PosCustomer customer, int tenantId);
    Task<object> VerifyCustomer(string accountIdentifier, int tenantId);
    Task<object> CustomerLogin(string accountIdentifier, string password, int tenantId);
    Task<object> VerifyOtp(string accountIdentifier, string otp, int tenantId);
}