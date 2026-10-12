using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.MultiTenancy;

namespace RhyoliteOrbit.DomainServices.Dms.Subscription;

public class DmsSubscriptionGuard: DomainService, IDmsSubscriptionGuard
{
    /// <summary>Grace period (days) granted after the subscription end date.</summary>
    public const int GracePeriodDays = 7;

    private readonly TenantManager _tenantManager;
    private readonly IAbpSession _abpSession;
    private readonly IRepository<Models.Dms.DmsQuotaUsage, System.Guid> _quotaUsageRepository;
    private readonly IRepository<Models.Dms.DmsSettings, System.Guid> _settingsRepository;

    public DmsSubscriptionGuard(
        TenantManager tenantManager,
        IAbpSession abpSession,
        IRepository<Models.Dms.DmsQuotaUsage, System.Guid> quotaUsageRepository,
        IRepository<Models.Dms.DmsSettings, System.Guid> settingsRepository)
    {
        _tenantManager = tenantManager;
        _abpSession = abpSession;
        _quotaUsageRepository = quotaUsageRepository;
        _settingsRepository = settingsRepository;
    }

    public async Task<bool> IsActiveAsync()
    {
        var tenant = await GetTenantAsync();

        if (tenant == null || !tenant.HasSubscribedToDocumentManagement)
        {
            return false;
        }

        if (!tenant.DocumentManagementSubscriptionEndDate.HasValue)
        {
            return true;
        }

        return tenant.DocumentManagementSubscriptionEndDate.Value.AddDays(GracePeriodDays) >= DateTime.UtcNow.Date;
    }

    public async Task EnsureActiveAsync()
    {
        var tenant = await GetTenantAsync();

        if (tenant == null)
        {
            throw new Abp.UI.UserFriendlyException("A tenant context is required to use the document management module.");
        }

        if (!tenant.HasSubscribedToDocumentManagement)
        {
            throw new Abp.UI.UserFriendlyException(
                "DmsSubscriptionInactive",
                "Your Document Management subscription is not active. Please subscribe in the Module Manager to continue.");
        }

        if (tenant.DocumentManagementSubscriptionEndDate.HasValue &&
            tenant.DocumentManagementSubscriptionEndDate.Value.AddDays(GracePeriodDays) < DateTime.UtcNow.Date)
        {
            throw new Abp.UI.UserFriendlyException(
                "DmsSubscriptionExpired",
                $"Your Document Management subscription expired on {tenant.DocumentManagementSubscriptionEndDate.Value:dd-MMM-yyyy}. Please renew it to continue.");
        }
    }

    public async Task EnsureQuotaAsync(long additionalBytes)
    {
        var tenant = await GetTenantAsync();
        if (tenant == null)
        {
            return;
        }

        var settings = await GetSettingsAsync(tenant.Id);
        if (settings == null || settings.QuotaGb <= 0)
        {
            return;
        }

        var quotaBytes = settings.QuotaGb * 1024L * 1024L * 1024L;

        var usage = await (await _quotaUsageRepository.GetAllAsync())
            .Where(x => x.TenantId == tenant.Id)
            .FirstOrDefaultAsync();

        var used = usage?.BytesUsed ?? 0L;

        if (used + additionalBytes > quotaBytes)
        {
            throw new Abp.UI.UserFriendlyException(
                "DmsQuotaExceeded",
                "The document storage quota for this account has been reached. Please contact support.");
        }
    }

    private async Task<Tenant> GetTenantAsync()
    {
        var tenantId = _abpSession.TenantId;
        if (!tenantId.HasValue)
        {
            return null;
        }

        return await _tenantManager.GetByIdAsync(tenantId.Value);
    }

    private async Task<Models.Dms.DmsSettings> GetSettingsAsync(int tenantId)
    {
        return await (await _settingsRepository.GetAllAsync())
            .Where(x => x.TenantId == tenantId)
            .FirstOrDefaultAsync();
    }
}
