using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.Customers;

public interface ICustomerManager: IDomainService
{
    Task<PosCustomer> CreateCustomerAsync(PosCustomer customer);
    Task UpdateProfileAsync(Guid customerId, string firstName, string lastName, string email, string phoneNumber, string region);
    Task<PosCustomer> GetByPhoneNumberAsync(string phoneNumber);

    // Financial & Credit Operations
    Task AdjustWalletBalanceAsync(Guid customerId, decimal amount, string referenceReason);
    Task UpdateCreditProfileAsync(Guid customerId, decimal newLimit, int creditTermDays, bool isCreditAllowed);
    Task SetCreditBlockStatusAsync(Guid customerId, bool isBlocked, string reason);
    Task<bool> CanPurchaseOnCreditAsync(Guid customerId, decimal requestedCreditAmount);

    // Loyalty & Metrics
    Task AddLoyaltyPointsAsync(Guid customerId, int points);
    Task DeductLoyaltyPointsAsync(Guid customerId, int points);
    Task RecordPurchaseAsync(Guid customerId, decimal purchaseAmount);

    // System Cleanup / Anonymization
    Task DeactivateCustomerAsync(Guid customerId);
}