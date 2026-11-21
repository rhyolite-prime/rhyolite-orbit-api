using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Dms;

namespace RhyoliteOrbit.DomainServices.Dms.Settings;

public interface IDmsSettingsManager
{
    Task<DmsSettings> GetAsync();

    Task<DmsSettings> UpdateAsync(DmsSettings settings);

    Task<DmsSettings> EnsureCreatedAsync();
}

/// <summary>Per tenant DMS configuration with sane defaults.</summary>
public class DmsSettingsManager: DomainService, IDmsSettingsManager
{
    private static readonly string[] DefaultAllowedTypes =
    {
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".csv", ".json", ".xml",
        ".png", ".jpg", ".jpeg", ".gif", ".tif", ".tiff", ".bmp", ".webp",
        ".zip", ".eml", ".msg", ".md", ".html", ".htm", ".dicom", ".dcm"
    };

    private readonly IRepository<DmsSettings, Guid> _repository;
    private readonly IAbpSession _abpSession;

    public DmsSettingsManager(IRepository<DmsSettings, Guid> repository, IAbpSession abpSession)
    {
        _repository = repository;
        _abpSession = abpSession;
    }

    public async Task<DmsSettings> GetAsync()
    {
        var tenantId = _abpSession.TenantId ?? 0;

        var settings = await (await _repository.GetAllAsync())
            .FirstOrDefaultAsync(s => s.TenantId == tenantId);

        return settings ?? await EnsureCreatedAsync();
    }

    public async Task<DmsSettings> EnsureCreatedAsync()
    {
        var tenantId = _abpSession.TenantId ?? 0;

        var settings = new DmsSettings
        {
            TenantId = tenantId,
            StorageProvider = "Cloudinary",
            LocalRootPath = "/var/dms",
            AllowedFileTypes = new List<string>(DefaultAllowedTypes),
            MaxFileSizeMb = 512,
            QuotaGb = 0,
            OcrEnabled = true,
            OcrLanguages = new List<string> { "eng" },
            AntivirusEnabled = true,
            AiEnabled = true,
            WatermarkText = string.Empty,
            TrashRetentionDays = 30,
            EnforceVersioning = true,
            DefaultLanguage = "en",
            CreatorUserId = _abpSession.UserId
        };

        return await _repository.InsertAsync(settings);
    }

    public async Task<DmsSettings> UpdateAsync(DmsSettings settings)
    {
        var existing = await GetAsync();

        existing.StorageProvider = settings.StorageProvider;
        existing.LocalRootPath = settings.LocalRootPath;
        existing.AllowedFileTypes = settings.AllowedFileTypes ?? existing.AllowedFileTypes;
        existing.MaxFileSizeMb = settings.MaxFileSizeMb > 0 ? settings.MaxFileSizeMb : existing.MaxFileSizeMb;
        existing.QuotaGb = settings.QuotaGb >= 0 ? settings.QuotaGb : existing.QuotaGb;
        existing.OcrEnabled = settings.OcrEnabled;
        existing.OcrLanguages = settings.OcrLanguages ?? existing.OcrLanguages;
        existing.AntivirusEnabled = settings.AntivirusEnabled;
        existing.AiEnabled = settings.AiEnabled;
        existing.WatermarkText = settings.WatermarkText;
        existing.TrashRetentionDays = settings.TrashRetentionDays >= 0 ? settings.TrashRetentionDays : existing.TrashRetentionDays;
        existing.EnforceVersioning = settings.EnforceVersioning;
        existing.DefaultLanguage = settings.DefaultLanguage;

        return await _repository.UpdateAsync(existing);
    }
}
