using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RhyoliteOrbit.Models.Pos;
using RhyoliteOrbit.Models.Shared;
using RhyoliteOrbit.Redis;

namespace RhyoliteOrbit.DomainServices.Pos.Customers;

public class CustomerManager: DomainService, ICustomerManager
{
    private readonly IRepository<PosCustomer, Guid> _customerRepository;
    private readonly IRepository<BusinessProfile, Guid> _repositoryBusinessProfile;
    private readonly IRedisCacheManager _redisCacheManager;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IPasswordHasher<PosCustomer> _passwordHasher;
    private const int OtpTtlMinutes = 5;
    public CustomerManager(IRepository<PosCustomer, Guid> customerRepository, IConfiguration configuration, IRedisCacheManager redisCacheManager, IUnitOfWorkManager unitOfWorkManager, IRepository<BusinessProfile, Guid> repositoryBusinessProfile, IPasswordHasher<PosCustomer> passwordHasher)
    {
        _customerRepository = customerRepository;
        _redisCacheManager = redisCacheManager;
        _unitOfWorkManager = unitOfWorkManager;
        _repositoryBusinessProfile = repositoryBusinessProfile;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
            
        var predicate = PredicateBuilder.New<PosCustomer>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.FirstName.ToLower().Contains(query.ToLower()) || x.LastName.ToLower().Contains(query.ToLower()) || x.PhoneNumber.Contains(query.ToLower()) || x.Email.ToLower().Contains(query.ToLower()) || x.Username.ToLower().Contains(query.ToLower()));

        }
            
        var totalCount =
                
            await _customerRepository.CountAsync(predicate);
            
        var data = await (await _customerRepository.GetAllAsync()).Where(predicate).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

        var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

        var paginatedResult = new
        {
            pageNo,
            totalCount,
            totalPages,
            data,
            lowerBound = pageSize * (pageNo - 1) + 1,
            upperBound = totalPages == pageNo ? totalCount : pageNo * pageSize,
        };

        return paginatedResult;
            
    }


    
    public async Task Create(PosCustomer customer)
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
        await _customerRepository.InsertAsync(customer);
    }

    public async Task Update(PosCustomer customer)
    {
        var customerInfo = await _customerRepository.FirstOrDefaultAsync(customer.Id);
        // Check for email collision if updated
        if (!string.IsNullOrWhiteSpace(customer.Email) && customer.Email != customerInfo.Email)
        {
            var existingEmail = await _customerRepository.FirstOrDefaultAsync(x => x.Email == customer.Email && x.Id != customer.Id);
            if (existingEmail != null)
            {
                throw new UserFriendlyException(400, "Email is already taken by another customer.");
            }
        }

        customer.FirstName = customer.FirstName;
        customer.LastName = customer.LastName;
        customer.Email = customer.Email;
        customer.PhoneNumber = customer.PhoneNumber;
        customer.DefaultRegion = customer.PhoneNumber;

        await _customerRepository.UpdateAsync(customer);
    }

    public async Task<PosCustomer> GetByPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return null;
        }

        return await _customerRepository.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
    }
    
    public async Task AdjustWalletBalance(Guid customerId, decimal amount, string referenceReason)
    {
        var customer = await _customerRepository.FirstOrDefaultAsync(customerId);

        if (amount < 0 && (customer.WalletBalance + amount) < 0)
        {
            throw new UserFriendlyException(400, "Insufficient wallet balance.");
        }

        customer.WalletBalance += amount;
        await _customerRepository.UpdateAsync(customer);
    }

    public async Task UpdateCreditProfile(Guid customerId, decimal newLimit, int creditTermDays, bool isCreditAllowed)
    {
        if (newLimit < 0)
        {
            throw new UserFriendlyException(400, "Credit limit cannot be negative.");
        }

        var customer = await _customerRepository.FirstOrDefaultAsync(customerId);

        customer.CreditLimit = newLimit;
        customer.DefaultCreditTermInDays = creditTermDays;
        customer.IsCreditAllowed = isCreditAllowed;

        await _customerRepository.UpdateAsync(customer);
    }

    public async Task SetCreditBlockStatus(Guid customerId, bool isBlocked, string reason)
    {
        var customer = await _customerRepository.FirstOrDefaultAsync(customerId);

        customer.BlockedFromCredit = isBlocked;
        await _customerRepository.UpdateAsync(customer);
    }

    public async Task<bool> CanPurchaseOnCredit(Guid customerId, decimal requestedCreditAmount)
    {
        var customer = await _customerRepository.FirstOrDefaultAsync(customerId);

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

    public async Task AddLoyaltyPoints(Guid customerId, int points)
    {
        if (points <= 0) return;

        var customer = await _customerRepository.FirstOrDefaultAsync(customerId);

        customer.LoyaltyPoints += points;
        await _customerRepository.UpdateAsync(customer);
    }

    public async Task DeductLoyaltyPoints(Guid customerId, int points)
    {
        if (points <= 0) return;

        var customer = await _customerRepository.FirstOrDefaultAsync(customerId);

        if (customer.LoyaltyPoints < points)
        {
            throw new UserFriendlyException(400, "Insufficient loyalty points balance.");
        }

        customer.LoyaltyPoints -= points;
        await _customerRepository.UpdateAsync(customer);
    }

    public async Task RecordPurchase(Guid customerId, decimal purchaseAmount)
    {
        var customer = await _customerRepository.FirstOrDefaultAsync(customerId);

        customer.TotalAmountSpent += purchaseAmount;
        customer.NumberOfVisits += 1;

        await _customerRepository.UpdateAsync(customer);
    }

    public async Task DeactivateCustomer(Guid customerId)
    {
        await _customerRepository.DeleteAsync(customerId);
    }

    public async Task RegisterCustomer(PosCustomer customer, int tenantId)
    {
        if (customer == null)
        {
            throw new UserFriendlyException(400, "Customer data is required.");
        }

        if (string.IsNullOrWhiteSpace(customer.PhoneNumber) &&
            string.IsNullOrWhiteSpace(customer.Email))
        {
            throw new UserFriendlyException(400,
                "A phone number or email address is required to register a customer.");
        }

        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            // Uniqueness within the tenant
            if (!string.IsNullOrWhiteSpace(customer.PhoneNumber))
            {
                var existingByPhone = await _customerRepository.FirstOrDefaultAsync(x =>
                    x.PhoneNumber == customer.PhoneNumber);
                if (existingByPhone != null)
                {
                    throw new UserFriendlyException(400,
                        "A customer with this phone number already exists.");
                }
            }

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                var existingByEmail = await _customerRepository.FirstOrDefaultAsync(x =>
                    x.Email == customer.Email);
                if (existingByEmail != null)
                {
                    throw new UserFriendlyException(400,
                        "A customer with this email already exists.");
                }
            }

            customer.TenantId = tenantId;
            customer.Pin = _passwordHasher.HashPassword(customer, customer.Pin); // hash the password

             await _customerRepository.InsertAsync(customer);
        }
 
        //send email to customer
       
       
    }

    public async Task<object> VerifyCustomer(string accountIdentifier, int tenantId)
    {
        if (string.IsNullOrWhiteSpace(accountIdentifier))
            throw new UserFriendlyException(400, "Account identifier is required.");

        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            var customer = await _customerRepository.FirstOrDefaultAsync(x =>
                x.PhoneNumber == accountIdentifier || x.Email == accountIdentifier);

            if (customer == null)
                throw new UserFriendlyException(404, "Customer not found.");

            // Generate and send a new OTP
            var otp = GenerateOtp();
            var otpKey = BuildOtpKey(tenantId, accountIdentifier);
            await _redisCacheManager.SetValueAsync(otpKey, otp);
            await _redisCacheManager.SetExpireTimeAsync(otpKey, TimeSpan.FromMinutes(5));

            return new
            {
                CustomerId = customer.Id,
                AccountIdentifier = accountIdentifier,
                OtpSent = true,
                ExpiresInMinutes = 5
            };
        }
    }

    public async Task<object> CustomerLogin(string accountIdentifier, string password, int tenantId)
    {
        
        if (string.IsNullOrWhiteSpace(accountIdentifier) || string.IsNullOrWhiteSpace(password))
        {
            throw new UserFriendlyException(400,
                "Account identifier and password are required.");
        }

        accountIdentifier = accountIdentifier.Trim();

        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            // 1. Resolve customer by phone OR email
            var customer = await _customerRepository.FirstOrDefaultAsync(x =>
                x.PhoneNumber == accountIdentifier || x.Email == accountIdentifier);

            // 2. Deliberately identical error for "not found" and "bad password"
            //    so we don't leak which accounts exist.
            if (customer == null)
            {
                throw new UserFriendlyException(401, "Invalid credentials.");
            }

            var hashedPassword = _passwordHasher.HashPassword(customer, password);
            
            // 3. Verify the password against the stored hash
            var verification = _passwordHasher.VerifyHashedPassword(customer, hashedPassword, password);

            if (verification == PasswordVerificationResult.Failed)
            {
                throw new UserFriendlyException(401, "Invalid credentials.");
            }
            
            // 6. Issue the JWT
            var jwtToken = await GenerateCustomerTokenAsync(customer, tenantId);

            return new
            {
                Authenticated = true,
                CustomerId = customer.Id,
                customer.Username,
                customer.Email,
                AccountIdentifier = accountIdentifier,
                token = jwtToken
            };
        }
    }
    
    
    public async Task<object> VerifyOtp(string accountIdentifier, string otp, int tenantId)
    {
        if (string.IsNullOrWhiteSpace(accountIdentifier) || string.IsNullOrWhiteSpace(otp))
        {
            throw new UserFriendlyException(400, "Account identifier and OTP are required.");
        }

        accountIdentifier = accountIdentifier.Trim();
        otp = otp.Trim();

        var otpKey = BuildOtpKey(tenantId, accountIdentifier);
        var storedOtp = await _redisCacheManager.GetValueAsync(otpKey);

        if (string.IsNullOrWhiteSpace(storedOtp))
        {
            throw new UserFriendlyException(400,
                "OTP has expired or was never requested for this account.");
        }

        // Fixed-time comparison to avoid leaking timing information.
        if (!FixedTimeEquals(storedOtp, otp))
        {
            throw new UserFriendlyException(400, "Invalid OTP.");
        }

        // Consume the OTP so it cannot be replayed.
        await _redisCacheManager.RemoveValueAsync(otpKey);

        
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            var customer = await _customerRepository.FirstOrDefaultAsync(x =>
                x.PhoneNumber == accountIdentifier || x.Email == accountIdentifier);

            if (customer == null)
            {
                throw new UserFriendlyException(404, "Customer not found.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes($"{_configuration["Authentication:JwtBearer:SecurityKey"]}"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            
            string defaultAudience = $"RhyolitePrime Customer";
            
            //retrieve business name from business profile.

            var businessProfile = await _repositoryBusinessProfile.FirstOrDefaultAsync(x => x.TenantId == tenantId);
            
            
            var claims = new[]
            {
                new Claim("customerId", customer.Id.ToString()),
                new Claim("phoneNumber", customer.PhoneNumber ?? ""),
                new Claim("email", customer.Email ?? ""),
                new Claim("firstName", customer.FirstName ?? ""),
                new Claim("lastName", customer.LastName ?? ""),
                new Claim("username", customer.Username ?? ""),
            };

            var token = new JwtSecurityToken(
                issuer: "RhyolitePrime",
                audience: businessProfile != null ? $"{businessProfile.CompanyName} Customer" : defaultAudience,
                claims: claims,
                notBefore: now,
                expires: now.AddMonths(3),
                signingCredentials: creds
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            
            return new
            {
                Verified = true,
                CustomerId = customer.Id,
                AccountIdentifier = accountIdentifier,
                token = jwtToken
            };
        }

        
    }
    
    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    private static string ResolveAccountIdentifier(string phoneNumber, string email)
        => !string.IsNullOrWhiteSpace(phoneNumber) ? phoneNumber : email;

    private static string BuildOtpKey(int tenantId, string accountIdentifier)
        => $"pos:customer:otp:{tenantId}:{accountIdentifier.ToLowerInvariant()}";

    private static string GenerateOtp()
    {
        // 6-digit, cryptographically-random numeric OTP.
        var buffer = new byte[4];
        RandomNumberGenerator.Fill(buffer);
        var value = BitConverter.ToUInt32(buffer, 0) % 1_000_000u;
        return value.ToString("D6");
    }

    private async Task<string> GenerateCustomerTokenAsync(PosCustomer customer, int tenantId)
    {
        var securityKey = _configuration["Authentication:JwtBearer:SecurityKey"];
        if (string.IsNullOrWhiteSpace(securityKey))
        {
            throw new UserFriendlyException(500,
                "Authentication is misconfigured: JWT security key is missing.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var now = DateTime.UtcNow;

        var businessProfile = await _repositoryBusinessProfile
            .FirstOrDefaultAsync(x => x.TenantId == tenantId);

        var audience = businessProfile != null
            ? $"{businessProfile.CompanyName} Customer"
            : "RhyolitePrime Customer";

        var claims = new[]
        {
            new Claim("customerId", customer.Id.ToString()),
            new Claim("tenantId", tenantId.ToString()),
            new Claim("phoneNumber", customer.PhoneNumber ?? string.Empty),
            new Claim("email", customer.Email ?? string.Empty),
            new Claim("firstName", customer.FirstName ?? string.Empty),
            new Claim("lastName", customer.LastName ?? string.Empty),
            new Claim("username", customer.Username ?? string.Empty),
        };

        var token = new JwtSecurityToken(
            issuer: "RhyolitePrime",
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMonths(6),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private static bool FixedTimeEquals(string a, string b)
    {
        if (a == null || b == null || a.Length != b.Length)
        {
            return false;
        }

        var diff = 0;
        for (var i = 0; i < a.Length; i++)
        {
            diff |= a[i] ^ b[i];
        }
        return diff == 0;
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