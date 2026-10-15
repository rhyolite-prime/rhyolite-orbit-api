using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.Dms.Settings;
using RhyoliteOrbit.DomainServices.Dms.Storage;
using RhyoliteOrbit.Models.Dms;

namespace RhyoliteOrbit.DomainServices.Dms.Versions;

public class VersionManager: DomainService, IVersionManager
{
    private readonly IRepository<DmsDocumentVersion, Guid> _versionRepository;
    private readonly IRepository<DmsDocument, Guid> _documentRepository;
    private readonly IStorageProviderFactory _storageProviderFactory;
    private readonly IAbpSession _abpSession;

    public VersionManager(
        IRepository<DmsDocumentVersion, Guid> versionRepository,
        IRepository<DmsDocument, Guid> documentRepository,
        IStorageProviderFactory storageProviderFactory,
        IAbpSession abpSession)
    {
        _versionRepository = versionRepository;
        _documentRepository = documentRepository;
        _storageProviderFactory = storageProviderFactory;
        _abpSession = abpSession;
    }

    public async Task<DmsDocumentVersion> CreateInitialAsync(DmsDocument document, VersionUploadInput input)
    {
        var tenantId = _abpSession.TenantId ?? 0;
        var settings = await ResolveStorageProviderAsync(document);

        var versionId = Guid.NewGuid();
        var extension = Path.GetExtension(input.FileName ?? string.Empty).ToLowerInvariant();
        var fileName = $"{versionId:N}{extension}";
        var key = $"{tenantId}/{document.CollectionId}/{document.FolderId}/{document.Id}/{fileName}";

        var stored = await settings.PutAsync(input.Content, key, input.ContentType);

        var checksum = await ComputeChecksumAsync(input.Content);

        var version = new DmsDocumentVersion
        {
            TenantId = tenantId,
            Id = versionId,
            DocumentId = document.Id,
            VersionNumber = 1,
            MinorNumber = 0,
            VersionLabel = "1.0",
            ChangeNote = input.ChangeNote,
            FileName = input.FileName,
            Extension = extension,
            FileSize = stored.Size,
            MimeType = input.ContentType,
            StorageProvider = stored.Provider,
            StorageKey = stored.Key,
            Checksum = checksum,
            IsCurrent = true,
            CreatorUserId = input.CreatedById
        };

        version = await _versionRepository.InsertAsync(version);
        return version;
    }

    public async Task<DmsDocumentVersion> AddAsync(Guid documentId, VersionUploadInput input)
    {
        var document = await _documentRepository.FirstOrDefaultAsync(documentId);
        if (document == null)
        {
            throw new Abp.UI.UserFriendlyException("DmsDocumentNotFound", "The requested document was not found.");
        }

        if (document.IsCheckedOut && document.CheckedOutBy != input.CreatedById)
        {
            throw new Abp.UI.UserFriendlyException(
                "DmsDocumentCheckedOut",
                "The document is checked out by another user; new versions cannot be uploaded.");
        }

        var tenantId = _abpSession.TenantId ?? 0;
        var provider = _storageProviderFactory.GetProvider(document.StorageProvider);

        var current = await GetCurrentAsync(documentId);

        var versionId = Guid.NewGuid();
        var extension = Path.GetExtension(input.FileName ?? string.Empty).ToLowerInvariant();
        var fileName = $"{versionId:N}{extension}";
        var key = $"{tenantId}/{document.CollectionId}/{document.FolderId}/{document.Id}/{fileName}";

        var stored = await provider.PutAsync(input.Content, key, input.ContentType);
        var checksum = await ComputeChecksumAsync(input.Content);

        int major = current?.VersionNumber ?? 1;
        int minor = current?.MinorNumber ?? 0;

        if (input.Scheme == (int)RhyoliteOrbitConsts.DocumentVersionScheme.Major)
        {
            major += 1;
            minor = 0;
        }
        else
        {
            minor += 1;
        }

        var version = new DmsDocumentVersion
        {
            TenantId = tenantId,
            Id = versionId,
            DocumentId = documentId,
            VersionNumber = major,
            MinorNumber = minor,
            VersionLabel = $"{major}.{minor}",
            ChangeNote = input.ChangeNote,
            FileName = input.FileName,
            Extension = extension,
            FileSize = stored.Size,
            MimeType = input.ContentType,
            StorageProvider = stored.Provider,
            StorageKey = stored.Key,
            Checksum = checksum,
            IsCurrent = true,
            CreatorUserId = input.CreatedById
        };

        if (current != null)
        {
            current.IsCurrent = false;
            await _versionRepository.UpdateAsync(current);
        }

        version = await _versionRepository.InsertAsync(version);

        document.CurrentVersionId = version.Id;
        document.FileSize = version.FileSize;
        document.StorageProvider = version.StorageProvider;
        document.StorageKey = version.StorageKey;
        document.FilePath = version.StorageKey;
        document.Checksum = version.Checksum;
        document.FileName = version.FileName;
        document.Extension = version.Extension;
        document.MimeType = version.MimeType;
        document.IndexStatus = (int)RhyoliteOrbitConsts.IndexStatus.Queued;
        document.ScanStatus = (int)RhyoliteOrbitConsts.ScanStatus.Queued;
        document.AiStatus = (int)RhyoliteOrbitConsts.AiStatus.NotRun;

        await _documentRepository.UpdateAsync(document);

        return version;
    }

    public async Task PromoteAsync(Guid versionId, long promotedBy)
    {
        var version = await GetAsync(versionId);
        if (version == null)
        {
            throw new Abp.UI.UserFriendlyException("DmsVersionNotFound", "The requested document version was not found.");
        }

        var versions = await ListAsync(version.DocumentId);

        foreach (var item in versions.Where(v => v.IsCurrent))
        {
            item.IsCurrent = false;
            await _versionRepository.UpdateAsync(item);
        }

        version.IsCurrent = true;
        await _versionRepository.UpdateAsync(version);

        var document = await _documentRepository.FirstOrDefaultAsync(version.DocumentId);
        if (document != null)
        {
            document.CurrentVersionId = version.Id;
            document.FileSize = version.FileSize;
            document.StorageProvider = version.StorageProvider;
            document.StorageKey = version.StorageKey;
            document.Checksum = version.Checksum;
            await _documentRepository.UpdateAsync(document);
        }
    }

    public async Task<List<DmsDocumentVersion>> ListAsync(Guid documentId)
    {
        return await (await _versionRepository.GetAllAsync())
            .Where(v => v.DocumentId == documentId)
            .OrderByDescending(v => v.VersionNumber)
            .ThenByDescending(v => v.MinorNumber)
            .ToListAsync();
    }

    public async Task<DmsDocumentVersion> GetAsync(Guid versionId)
    {
        return await _versionRepository.FirstOrDefaultAsync(versionId);
    }

    public async Task<DmsDocumentVersion> GetCurrentAsync(Guid documentId)
    {
        return await (await _versionRepository.GetAllAsync())
            .Where(v => v.DocumentId == documentId && v.IsCurrent)
            .OrderByDescending(v => v.VersionNumber)
            .ThenByDescending(v => v.MinorNumber)
            .FirstOrDefaultAsync();
    }

    public async Task<Stream> OpenReadAsync(Guid versionId)
    {
        var version = await GetAsync(versionId);
        if (version == null)
        {
            throw new Abp.UI.UserFriendlyException("DmsVersionNotFound", "The requested document version was not found.");
        }

        var provider = _storageProviderFactory.GetProvider(version.StorageProvider);
        return await provider.GetAsync(version.StorageKey);
    }

    public async Task UpdateIndexStateAsync(Guid versionId, string extractedText, bool ocrApplied, int pageCount, string language)
    {
        var version = await GetAsync(versionId);
        if (version == null)
        {
            return;
        }

        version.ExtractedText = extractedText;
        version.OcrApplied = ocrApplied;
        version.PageCount = pageCount;
        version.Language = language;

        await _versionRepository.UpdateAsync(version);

        var document = await _documentRepository.FirstOrDefaultAsync(version.DocumentId);
        if (document != null)
        {
            document.PageCount = pageCount;
            document.Language = language;
            document.IndexStatus = (int)RhyoliteOrbitConsts.IndexStatus.Indexed;
            document.IndexedAt = DateTime.UtcNow;
            await _documentRepository.UpdateAsync(document);
        }
    }

    public async Task DeleteAsync(Guid versionId, long deletedBy)
    {
        var version = await GetAsync(versionId);
        if (version == null)
        {
            return;
        }

        if (version.IsCurrent)
        {
            throw new Abp.UI.UserFriendlyException("The current version of a document cannot be deleted.");
        }

        var provider = _storageProviderFactory.GetProvider(version.StorageProvider);

        try
        {
            await provider.DeleteAsync(version.StorageKey);
        }
        catch (Exception ex)
        {
            Logger.Warn($"Could not delete binary for version {version.Id}: {ex.Message}");
        }

        await _versionRepository.HardDeleteAsync(version);
    }

    private async Task<IStorageProvider> ResolveStorageProviderAsync(DmsDocument document)
    {
        return _storageProviderFactory.GetProvider(null);
    }

    private static async Task<string> ComputeChecksumAsync(Stream content)
    {
        if (content == null)
        {
            return null;
        }

        if (content.CanSeek)
        {
            content.Position = 0;
        }

        using var sha = SHA256.Create();
        var hash = await sha.ComputeHashAsync(content);

        var builder = new StringBuilder(hash.Length * 2);
        foreach (var b in hash)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }
}
