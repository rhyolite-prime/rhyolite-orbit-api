using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Pos.Merchants;

public interface IMerchantManager: IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query);
    Task<object> GetCreditReputation(Guid id);
    Task Create(Merchant entity);
    Task UpdateBasicInfo(Merchant entity);
    Task UpdateComplianceDocuments(Guid id, List<ComplianceDocument> complianceDocuments);
    Task UpdateRemittanceAccounts(Guid id, List<RemittanceAccount> remittanceAccounts);
    Task Delete(Guid id);
    Task<object> VerifyMerchant(string merchantIdentifier, string phoneNo, int pin, int tenantId);
    Task UpdateLogo(Guid id, string logoUrl);
    Task ChangePin(string merchantIdentifier, string phoneNo, int oldPin, int newPin, int tenantId);
}