using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Dms;
using RhyoliteOrbit.DomainServices.Dms.Versions;

namespace RhyoliteOrbit.DomainServices.Dms.Quota;

public interface IQuotaManager
{
    Task<object> GetUsageAsync();

    Task RecomputeAsync();

    Task<bool> HasSpaceAsync(long additionalBytes);

    Task<long> GetQuotaBytesAsync();
}

/// <summary>Tracks storage consumption per tenant and enforces the quota.</summary>
public class QuotaManager: DomainService, IQuotaManager
{
    private readonly IRepository<DmsQuotaUsage, Guid> _usageRepository;
    private readonly IRepository<DmsDocument, Guid> _documentRepository;
    private readonly IRepository<DmsDocumentVersion, Guid> _versionRepository;
    private readonly IDmsSettingsManager _settingsManager;
    private readonly IAbpSession _abpSession;

    public QuotaManager(
        IRepository<DmsQuotaUsage, Guid> usageRepository,
        IRepository<DmsDocument, Guid> documentRepository,
        IRepository<DmsDocumentVersion, Guid> versionRepository,
        IDmsSettingsManager settingsManager,
        IAbpSession abpSession)
    {
        _usageRepository = usageRepository;
        _documentRepository = documentRepository;
        _versionRepository = versionRepository;
        _settingsManager = settingsManager;
        _abpSession = abpSession;
    }

    public async Task<object> GetUsageAsync()
    {
        var tenantId = _abpSession.TenantId ?? 0;

        var usage = await (await _usageRepository.GetAllAsync())
            .FirstOrDefaultAsync(u => u.TenantId == tenantId);

        var settings = await _settingsManager.GetAsync();
        var quotaBytes = settings != null && settings.QuotaGb > 0 ? settings.QuotaGb * 1024L * 1024L * 1024L : 0;

        var used = usage?.BytesUsed ?? 0L;

        return new
        {
            BytesUsed = used,
            DocumentCount = usage?.DocumentCount ?? 0,
            VersionCount = usage?.VersionCount ?? 0,
            QuotaBytes = quotaBytes,
            QuotaGb = settings?.QuotaGb ?? 0,
            PercentUsed = quotaBytes > 0 ? Math.Round(used * 100m / quotaBytes, 2) : 0,
            ComputedAt = usage?.ComputedAt
        };
    }

    public async Task RecomputeAsync()
    {
        var tenantId = _abpSession.TenantId ?? 0;

        var documents = await (await _documentRepository.GetAllAsync())
            .Where(d => !d.IsDeleted)
            .ToListAsync();

        var bytesUsed = documents.Sum(d => d.FileSize);
        var versionCount = await (await _versionRepository.GetAllAsync()).CountAsync();

        var usage = await (await _usageRepository.GetAllAsync())
            .FirstOrDefaultAsync(u => u.TenantId == tenantId);

        if (usage == null)
        {
            usage = new DmsQuotaUsage
            {
                TenantId = tenantId,
                CreatorUserId = _abpSession.UserId
            };

            usage = await _usageRepository.InsertAsync(usage);
        }

        usage.BytesUsed = bytesUsed;
        usage.DocumentCount = documents.Count;
        usage.VersionCount = versionCount;
        usage.ComputedAt = DateTime.UtcNow;

        await _usageRepository.UpdateAsync(usage);
    }

    public async Task<bool> HasSpaceAsync(long additionalBytes)
    {
        var settings = await _settingsManager.GetAsync();
        if (settings == null || settings.QuotaGb <= 0)
        {
            return true;
        }

        var tenantId = _abpSession.TenantId ?? 0;
        var usage = await (await _usageRepository.GetAllAsync())
            .FirstOrDefaultAsync(u => u.TenantId == tenantId);

        var quotaBytes = settings.QuotaGb * 1024L * 1024L * 1024L;
        var used = usage?.BytesUsed ?? 0L;

        return used + additionalBytes <= quotaBytes;
    }

    public async Task<long> GetQuotaBytesAsync()
    {
        var settings = await _settingsManager.GetAsync();

        return settings != null && settings.QuotaGb > 0 ? settings.QuotaGb * 1024L * 1024L * 1024L : 0;
    }
}
