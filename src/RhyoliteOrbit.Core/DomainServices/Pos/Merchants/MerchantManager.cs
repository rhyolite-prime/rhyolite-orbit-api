using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Linq.Expressions;
using Abp.UI;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Pos;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Pos.Merchants;

public class MerchantManager: DomainService, IMerchantManager
{
    private readonly IRepository<Merchant, Guid> _repositoryMerchant;
    private readonly IRepository<SystemNumber, Guid> _repositorySystemNumber;
    private readonly IRepository<CreditSale, Guid> _creditSaleRepository;
    private readonly IRepository<CreditSaleRepaymentSchedule, Guid> _repaymentScheduleRepository;
    private readonly IRepository<CreditTerm, Guid> _creditTermRepository;
    private readonly IHashids _hashids;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public MerchantManager(
        IRepository<Merchant, Guid> repositoryMerchant, 
        IRepository<CreditSale, Guid> creditSaleRepository,
        IRepository<CreditSaleRepaymentSchedule, Guid> repaymentScheduleRepository,
        IRepository<CreditTerm, Guid> creditTermRepository,
        IHashids hashids, 
        IUnitOfWorkManager unitOfWorkManager, IRepository<SystemNumber, Guid> repositorySystemNumber)
    {
        _repositoryMerchant = repositoryMerchant;
        _creditSaleRepository = creditSaleRepository;
        _repaymentScheduleRepository = repaymentScheduleRepository;
        _creditTermRepository = creditTermRepository;
        _hashids = hashids;
        _unitOfWorkManager = unitOfWorkManager;
        _repositorySystemNumber = repositorySystemNumber;
    }
    
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        var predicate = PredicateBuilder.New<Merchant>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.Contains(query) || x.ContactPerson.Contains(query) || x.ContactEmail.Contains(query) || x.ContactNumber.Contains(query));

        }
            
        var totalCount =
                
            await _repositoryMerchant.CountAsync(predicate);
            
        var merchants = await (await _repositoryMerchant.GetAllAsync()).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

        var data = merchants.Select(a => new
        {
            a.Id,
            a.Name,
            a.ContactPerson,
            a.CountryName,
            a.Address,
            a.City,
            a.ContactEmail,
            a.RegionOrState,
            a.ContactNumber,
            a.MerchantIdentifier,
            a.BusinessDescription,
            a.BusinessClassification,
            a.BusinessSector,
            a.DateEstablished,
            a.IsFeeChargeOnCustomer,
            a.LogoUrl,
            a.IsActive,
            a.RevenueAccountNo,
            a.HasSubAccountEnabled,
            a.AverageRating,
            a.TaxIdentificationNumber,
            a.BusinessRegistrationNumber,
            ComplianceStatus = a.ComplianceDocuments != null && a.ComplianceDocuments.Any()
                ? (a.ComplianceDocuments.Count(x => !x.IsAccepted) > 0
                    ? $"{a.ComplianceDocuments.Count} Documents ({a.ComplianceDocuments.Count(x => !x.IsAccepted)} Rejected)"
                    : $"{a.ComplianceDocuments.Count} Documents ({a.ComplianceDocuments.Count} Accepted)")
                : "Pending Submission",
            a.PayoutAccountNo,
            a.IdentityType,
            a.IdentityCardNo,
            a.ComplianceDocuments,
            a.RemittanceAccounts
        }).ToList();
        
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

    public async Task<object> GetCreditReputation(Guid id)
    {
        var now = DateTime.UtcNow;

        // 1. Calculate Outstanding Balance
        var credits = await _creditSaleRepository.GetAllListAsync(x => x.MerchantId == id);
        decimal outstandingBalance = credits.Sum(x => x.OutstandingAmount);

        // 2. Get Total Credit Limit (from default term)
        var defaultTerm = await _creditTermRepository.FirstOrDefaultAsync(x => x.IsDefault && x.IsActive);
        decimal totalCreditLimit = defaultTerm?.MaxAmount ?? 2000;

        // 3. Count Overdue Installments
        var creditIds = credits.Select(c => c.Id).ToList();
        var allScheduleItems = await _repaymentScheduleRepository.GetAllListAsync(x => creditIds.Contains(x.CreditSaleId));
        int overdueCount = allScheduleItems.Count(x => !x.IsPaid && x.DueDate < now);

        // 4. Calculate Credit Score
        // Base score: 700
        // Penalty: -30 per overdue period
        // Reward: +10 per fully paid sale (capped at +100)
        int fullyPaidSales = credits.Count(x => x.Status == (int)RhyoliteOrbitConsts.CreditSaleStatus.FullyPaid);
        int creditScore = 700 - (overdueCount * 30) + Math.Min(100, fullyPaidSales * 10);
        
        // Clamp score between 300 and 900
        creditScore = Math.Clamp(creditScore, 300, 900);

        // 5. Determine status and color
        string statusLabel;
        string statusColour;

        if (creditScore >= 800)
        {
            statusLabel = "Excellent";
            statusColour = "Green";
        }
        else if (creditScore >= 700)
        {
            statusLabel = "Good Standing";
            statusColour = "Blue";
        }
        else if (creditScore >= 500)
        {
            statusLabel = "Fair";
            statusColour = "Orange";
        }
        else
        {
            statusLabel = "Poor";
            statusColour = "Red";
        }

        return new
        {
            creditScore,
            totalCreditLimit,
            outstandingBalance,
            overdueCount,
            statusLabel,
            statusColour
        };
    }
    
    
    public async Task Create(Merchant entity)
    {
        // Fetch system number configuration
        var systemNumber = await _repositorySystemNumber.FirstOrDefaultAsync(a => a.Key == "pos-merchant-id");
        if (systemNumber == null)
        {
            throw new UserFriendlyException(400, "Merchant ID sequence configuration not found.");
        }

        // Format the number sequence with zero-padding
        int nextNo = systemNumber.LastNo + 1;
        string formattedNo = nextNo.ToString().PadLeft(systemNumber.Padding, '0');

        // Build identifier handling optional Prefix, Suffix, and FormatPattern
        string prefix = systemNumber.Prefix ?? string.Empty;
        string suffix = systemNumber.Suffix ?? string.Empty;

        entity.MerchantIdentifier = string.IsNullOrEmpty(systemNumber.FormatPattern)
            ? $"{prefix}{formattedNo}{suffix}"
            : systemNumber.FormatPattern
                .Replace("{PREFIX}", prefix)
                .Replace("{NO}", formattedNo)
                .Replace("{SUFFIX}", suffix);

        // Increment and save the updated system number counter
        systemNumber.LastNo = nextNo;
        await _repositorySystemNumber.UpdateAsync(systemNumber);

        entity.RhyoliteCbsClientId = Guid.NewGuid();

        // Check for duplicates
        var existingMerchant = await _repositoryMerchant.FirstOrDefaultAsync(m => m.ContactEmail == entity.ContactEmail && m.ContactNumber == entity.ContactNumber);
        if (existingMerchant != null)
        {
            throw new UserFriendlyException(400, "Duplicate merchant records are not allowed.");
        }

        // Insert merchant record
        await _repositoryMerchant.InsertAsync(entity);
    }
    
    public async Task UpdateBasicInfo(Merchant entity)
    {
        var existingMerchant = await _repositoryMerchant.FirstOrDefaultAsync(m => m.Id == entity.Id);
        if (existingMerchant == null)
        {
            throw new UserFriendlyException(400, "Merchant not found.");
        }
        
        existingMerchant.Name = entity.Name;
        existingMerchant.ContactEmail = entity.ContactEmail;
        existingMerchant.ContactNumber = entity.ContactNumber;
        existingMerchant.DateEstablished = entity.DateEstablished;
        existingMerchant.Address = entity.Address;
        existingMerchant.City = entity.City;
        existingMerchant.RegionOrState = entity.RegionOrState;
        existingMerchant.CountryName = entity.CountryName;
        existingMerchant.BusinessDescription = entity.BusinessDescription;
        existingMerchant.BusinessClassification = entity.BusinessClassification;
        existingMerchant.ContactPerson = entity.ContactPerson;
        existingMerchant.BusinessSector = entity.BusinessSector;
        existingMerchant.Address = entity.Address;
        existingMerchant.TaxIdentificationNumber = entity.TaxIdentificationNumber;
        existingMerchant.BusinessRegistrationNumber = entity.BusinessRegistrationNumber;
        existingMerchant.IdentityType = entity.IdentityType;
        existingMerchant.IdentityCardNo = entity.IdentityCardNo;
        existingMerchant.IsFeeChargeOnCustomer = entity.IsFeeChargeOnCustomer;
        existingMerchant.HasSubAccountEnabled = entity.HasSubAccountEnabled;
        
        
        await _repositoryMerchant.UpdateAsync(existingMerchant);
    }
    
    public async Task UpdateComplianceDocuments(Guid id, List<ComplianceDocument> complianceDocuments)
    {
        var existingMerchant = await _repositoryMerchant.FirstOrDefaultAsync(m => m.Id == id);
        if (existingMerchant == null)
        {
            throw new UserFriendlyException(400, "Merchant not found.");
        }
        
        existingMerchant.ComplianceDocuments = complianceDocuments;
        
        await _repositoryMerchant.UpdateAsync(existingMerchant);
        
    }
    
    public async Task UpdateLogo(Guid id, string logoUrl)
    {
        var existingMerchant = await _repositoryMerchant.FirstOrDefaultAsync(m => m.Id == id);
        if (existingMerchant == null)
        {
            throw new UserFriendlyException(400, "Merchant not found.");
        }
        
        existingMerchant.LogoUrl = logoUrl;
        
        await _repositoryMerchant.UpdateAsync(existingMerchant);
        
    }
     
    
    public async Task UpdateRemittanceAccounts(Guid id, List<RemittanceAccount> remittanceAccounts)
    {
        var existingMerchant = await _repositoryMerchant.FirstOrDefaultAsync(m => m.Id == id);
        if (existingMerchant == null)
        {
            throw new UserFriendlyException(400, "Merchant not found.");
        }
        
        existingMerchant.RemittanceAccounts = remittanceAccounts;
        
        await _repositoryMerchant.UpdateAsync(existingMerchant);
        
    }
    
    public async Task Delete(Guid id)
    {
        await _repositoryMerchant.DeleteAsync(id);
    }

    public async Task<object> VerifyMerchant(string merchantIdentifier, string phoneNo, int pin, int tenantId)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            var existingMerchant = await _repositoryMerchant.FirstOrDefaultAsync(m => m.MerchantIdentifier == merchantIdentifier && m.ContactNumber == phoneNo);
            if (existingMerchant == null)
            {
                throw new UserFriendlyException(400, "Merchant not found.");
            }
            
            var hashedPin = _hashids.Encode(pin);
            if (existingMerchant.Pin != hashedPin)
            {
                throw new UserFriendlyException(400, "Invalid pin.");
            }

            return new
            {
                verified = true,
                message = "Merchant verified successfully."
                
            };

        }
    }
    
    public async Task ChangePin(string merchantIdentifier, string phoneNo, int oldPin, int newPin, int tenantId)
    {
        using (_unitOfWorkManager.Current.SetTenantId(tenantId))
        {
            var existingMerchant = await _repositoryMerchant.FirstOrDefaultAsync(m => m.MerchantIdentifier == merchantIdentifier && m.ContactNumber == phoneNo);
            if (existingMerchant == null)
            {
                throw new UserFriendlyException(400, "Merchant not found.");
            }
            
            //verify old pin
            var hashedOldPin = _hashids.Encode(oldPin);
            if (existingMerchant.Pin != hashedOldPin)
            {
                throw new UserFriendlyException(400, "Invalid old pin.");
            }
        
            var hashedPin = _hashids.Encode(newPin);
            existingMerchant.Pin = hashedPin;
        
            await _repositoryMerchant.UpdateAsync(existingMerchant);
        }

        
    }
}