using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.Customers;

public class CustomerManager: DomainService, ICustomerManager
{
    private readonly IRepository<PosCustomer, Guid> _customerRepository;

    public CustomerManager(IRepository<PosCustomer, Guid> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<PosCustomer> CreateCustomerAsync(PosCustomer customer)
    {
        // Uniqueness checks
        if (!string.IsNullOrWhiteSpace(customer.PhoneNumber))
        {
            var existingByPhone = await _customerRepository.FirstOrDefaultAsync(x => x.PhoneNumber == customer.PhoneNumber);
            if (existingByPhone != null)
            {
                throw new UserFriendlyException(400, "A customer with this phone number already exists.");
            }
        }

        if (!string.IsNullOrWhiteSpace(customer.Email))
        {
            var existingByEmail = await _customerRepository.FirstOrDefaultAsync(x => x.Email == customer.Email);
            if (existingByEmail != null)
            {
                throw new UserFriendlyException(400, "A customer with this email already exists.");
            }
        }

        return await _customerRepository.InsertAsync(customer);
    }

    public async Task UpdateProfileAsync(Guid customerId, string firstName, string lastName, string email, string phoneNumber, string region)
    {
        var customer = await GetEntityByIdAsync(customerId);

        // Check for email collision if updated
        if (!string.IsNullOrWhiteSpace(email) && email != customer.Email)
        {
            var existingEmail = await _customerRepository.FirstOrDefaultAsync(x => x.Email == email && x.Id != customerId);
            if (existingEmail != null)
            {
                throw new UserFriendlyException(400, "Email is already taken by another customer.");
            }
        }

        customer.FirstName = firstName;
        customer.LastName = lastName;
        customer.Email = email;
        customer.PhoneNumber = phoneNumber;
        customer.DefaultRegion = region;

        await _customerRepository.UpdateAsync(customer);
    }

    public async Task<PosCustomer> GetByPhoneNumberAsync(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return null;
        }

        return await _customerRepository.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
    }
    
    public async Task AdjustWalletBalanceAsync(Guid customerId, decimal amount, string referenceReason)
    {
        var customer = await GetEntityByIdAsync(customerId);

        if (amount < 0 && (customer.WalletBalance + amount) < 0)
        {
            throw new UserFriendlyException(400, "Insufficient wallet balance.");
        }

        customer.WalletBalance += amount;
        await _customerRepository.UpdateAsync(customer);
    }

    public async Task UpdateCreditProfileAsync(Guid customerId, decimal newLimit, int creditTermDays, bool isCreditAllowed)
    {
        if (newLimit < 0)
        {
            throw new UserFriendlyException(400, "Credit limit cannot be negative.");
        }

        var customer = await GetEntityByIdAsync(customerId);

        customer.CreditLimit = newLimit;
        customer.DefaultCreditTermInDays = creditTermDays;
        customer.IsCreditAllowed = isCreditAllowed;

        await _customerRepository.UpdateAsync(customer);
    }

    public async Task SetCreditBlockStatusAsync(Guid customerId, bool isBlocked, string reason)
    {
        var customer = await GetEntityByIdAsync(customerId);
        customer.BlockedFromCredit = isBlocked;
        await _customerRepository.UpdateAsync(customer);
    }

    public async Task<bool> CanPurchaseOnCreditAsync(Guid customerId, decimal requestedCreditAmount)
    {
        var customer = await GetEntityByIdAsync(customerId);

        if (!customer.IsCreditAllowed || customer.BlockedFromCredit)
        {
            return false;
        }

        if (requestedCreditAmount > customer.CreditLimit)
        {
            return false;
        }

        return true;
    }

    

    public async Task AddLoyaltyPointsAsync(Guid customerId, int points)
    {
        if (points <= 0) return;

        var customer = await GetEntityByIdAsync(customerId);
        customer.LoyaltyPoints += points;
        await _customerRepository.UpdateAsync(customer);
    }

    public async Task DeductLoyaltyPointsAsync(Guid customerId, int points)
    {
        if (points <= 0) return;

        var customer = await GetEntityByIdAsync(customerId);
        if (customer.LoyaltyPoints < points)
        {
            throw new UserFriendlyException(400, "Insufficient loyalty points balance.");
        }

        customer.LoyaltyPoints -= points;
        await _customerRepository.UpdateAsync(customer);
    }

    public async Task RecordPurchaseAsync(Guid customerId, decimal purchaseAmount)
    {
        var customer = await GetEntityByIdAsync(customerId);

        customer.TotalAmountSpent += purchaseAmount;
        customer.NumberOfVisits += 1;

        await _customerRepository.UpdateAsync(customer);
    }

    public async Task DeactivateCustomerAsync(Guid customerId)
    {
        // ABP FullAuditedEntity supports soft-delete automatically
        await _customerRepository.DeleteAsync(customerId);
    }

    private async Task<PosCustomer> GetEntityByIdAsync(Guid customerId)
    {
        var customer = await _customerRepository.FirstOrDefaultAsync(customerId);
        if (customer == null)
        {
            throw new UserFriendlyException(404, "Customer not found.");
        }

        return customer;
    }

     
}