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
using RhyoliteOrbit.DomainServices.Dms.Folders;
using RhyoliteOrbit.DomainServices.Dms.Metadata;
using RhyoliteOrbit.DomainServices.Dms.Settings;
using RhyoliteOrbit.DomainServices.Dms.Storage;
using RhyoliteOrbit.DomainServices.Dms.Versions;
using RhyoliteOrbit.Models.Dms;

namespace RhyoliteOrbit.DomainServices.Dms.Documents;

public class DocumentManager: DomainService, IDocumentManager
{
    private readonly IRepository<DmsDocument, Guid> _documentRepository;
    private readonly IRepository<DmsDocumentTag, Guid> _documentTagRepository;
    private readonly IRepository<DmsDocumentMetadataValues, Guid> _metadataValueRepository;
    private readonly IFolderManager _folderManager;
    private readonly IVersionManager _versionManager;
    private readonly IStorageProviderFactory _storageProviderFactory;
    private readonly IDmsSettingsManager _settingsManager;
    private readonly IAbpSession _abpSession;

    public DocumentManager(
        IRepository<DmsDocument, Guid> documentRepository,
        IRepository<DmsDocumentTag, Guid> documentTagRepository,
        IRepository<DmsDocumentMetadataValues, Guid> metadataValueRepository,
        IFolderManager folderManager,
        IVersionManager versionManager,
        IStorageProviderFactory storageProviderFactory,
        IDmsSettingsManager settingsManager,
        IAbpSession abpSession)
    {
        _documentRepository = documentRepository;
        _documentTagRepository = documentTagRepository;
        _metadataValueRepository = metadataValueRepository;
        _folderManager = folderManager;
        _versionManager = versionManager;
        _storageProviderFactory = storageProviderFactory;
        _settingsManager = settingsManager;
        _abpSession = abpSession;
    }

    public async Task<DmsDocument> UploadAsync(DocumentUploadInput input)
    {
        var tenantId = _abpSession.TenantId ?? 0;
        var settings = await _settingsManager.GetAsync();

        var folder = await _folderManager.GetAsync(input.FolderId);
        if (folder == null)
        {
            throw new UserFriendlyException("DmsFolderNotFound", "The target folder was not found.");
        }

        var extension = Path.GetExtension(input.FileName ?? input.Name ?? string.Empty).ToLowerInvariant();

        await ValidateFileAsync(settings, extension, input.Content);

        var checksum = await ComputeChecksumAsync(input.Content);

        if (await IsDuplicateAsync(checksum))
        {
            Logger.Info($"Duplicate upload detected for checksum {checksum} in tenant {tenantId}");
        }

        var document = new DmsDocument
        {
            TenantId = tenantId,
            FolderId = folder.Id,
            FolderName = folder.Name,
            CollectionId = folder.CollectionId,
            Name = input.Name ?? Path.GetFileNameWithoutExtension(input.FileName),
            FileName = input.FileName,
            Extension = extension,
            Title = input.Title,
            Description = input.Description,
            CustomId = input.CustomId,
            MimeType = input.ContentType ?? "application/octet-stream",
            Checksum = checksum,
            Status = input.IsDraft
                ? (int)RhyoliteOrbitConsts.DocumentStatus.Draft
                : (int)RhyoliteOrbitConsts.DocumentStatus.Active,
            OwnerId = input.CreatedById,
            IndexStatus = (int)RhyoliteOrbitConsts.IndexStatus.Queued,
            ScanStatus = (int)RhyoliteOrbitConsts.ScanStatus.Queued,
            OcrStatus = (int)RhyoliteOrbitConsts.OcrStatus.NotRequired,
            AiStatus = (int)RhyoliteOrbitConsts.AiStatus.NotRun,
            CreatorUserId = input.CreatedById
        };

        document = await _documentRepository.InsertAsync(document);

        var version = await _versionManager.CreateInitialAsync(document, new VersionUploadInput
        {
            Content = input.Content,
            FileName = input.FileName,
            ContentType = input.ContentType,
            ChangeNote = "Initial upload",
            Scheme = (int)RhyoliteOrbitConsts.DocumentVersionScheme.Major,
            CreatedById = input.CreatedById
        });

        document.CurrentVersionId = version.Id;
        document.FileSize = version.FileSize;
        document.StorageProvider = version.StorageProvider;
        document.StorageKey = version.StorageKey;
        document.FilePath = version.StorageKey;
        document.PageCount = version.PageCount;

        await _documentRepository.UpdateAsync(document);

        if (input.Metadata != null && input.Metadata.Any())
        {
            await SaveMetadataAsync(document.Id, input.Metadata, input.CreatedById);
        }

        if (input.TagIds != null && input.TagIds.Any())
        {
            await AssignTagsAsync(document.Id, input.TagIds, input.CreatedById);
        }

        return document;
    }

    public async Task<DmsDocument> UpdateAsync(DmsDocument document, long updatedBy)
    {
        var existing = await GetAsync(document.Id);
        if (existing == null)
        {
            throw new UserFriendlyException("DmsDocumentNotFound", "The requested document was not found.");
        }

        await EnsureWritableAsync(existing);

        existing.Name = document.Name;
        existing.Title = document.Title;
        existing.Description = document.Description;
        existing.CustomId = document.CustomId;
        existing.ExpiryDate = document.ExpiryDate;

        return await _documentRepository.UpdateAsync(existing);
    }

    public async Task MoveAsync(Guid documentId, Guid targetFolderId, long movedBy)
    {
        var document = await GetAsync(documentId);
        if (document == null)
        {
            throw new UserFriendlyException("DmsDocumentNotFound", "The requested document was not found.");
        }

        var folder = await _folderManager.GetAsync(targetFolderId);
        if (folder == null)
        {
            throw new UserFriendlyException("DmsFolderNotFound", "The target folder was not found.");
        }

        await EnsureWritableAsync(document);

        document.FolderId = folder.Id;
        document.FolderName = folder.Name;
        document.CollectionId = folder.CollectionId;

        await _documentRepository.UpdateAsync(document);
    }

    public async Task<DmsDocument> CopyAsync(Guid documentId, Guid targetFolderId, long copiedBy)
    {
        var source = await GetAsync(documentId);
        if (source == null)
        {
            throw new UserFriendlyException("DmsDocumentNotFound", "The requested document was not found.");
        }

        var folder = await _folderManager.GetAsync(targetFolderId);
        if (folder == null)
        {
            throw new UserFriendlyException("DmsFolderNotFound", "The target folder was not found.");
        }

        await EnsureWritableAsync(source);

        var version = await _versionManager.GetCurrentAsync(source.Id);
        if (version == null)
        {
            throw new UserFriendlyException("DmsVersionNotFound", "The document has no version to copy.");
        }

        await using var content = await _versionManager.OpenReadAsync(version.Id);

        var copy = await UploadAsync(new DocumentUploadInput
        {
            FolderId = folder.Id,
            Name = await SuggestCopyNameAsync(source.Name, folder.Id),
            FileName = version.FileName,
            Title = source.Title,
            Description = source.Description,
            ContentType = version.MimeType,
            Content = content,
            CreatedById = copiedBy
        });

        var values = await (await _metadataValueRepository.GetAllAsync())
            .Where(v => v.DocumentId == source.Id)
            .ToListAsync();

        foreach (var value in values)
        {
            await _metadataValueRepository.InsertAsync(new DmsDocumentMetadataValues
            {
                TenantId = copy.TenantId,
                DocumentId = copy.Id,
                AttributeId = value.AttributeId,
                Value = value.Value
            });
        }

        return copy;
    }

    public async Task DeleteAsync(Guid documentId, long deletedBy)
    {
        var document = await GetAsync(documentId);
        if (document == null)
        {
            throw new UserFriendlyException("DmsDocumentNotFound", "The requested document was not found.");
        }

        if (document.IsOnLegalHold)
        {
            throw new UserFriendlyException(
                "DmsDocumentLocked",
                "The document is on legal hold or write protected and cannot be modified.");
        }

        document.IsDeleted = true;
        document.DeletionTime = DateTime.UtcNow;
        document.DeleterUserId = deletedBy;
        document.Status = (int)RhyoliteOrbitConsts.DocumentStatus.Deleted;

        await _documentRepository.UpdateAsync(document);
    }

    public async Task RestoreAsync(Guid documentId, long restoredBy)
    {
        var document = await _documentRepository.FirstOrDefaultAsync(documentId);
        if (document == null)
        {
            throw new UserFriendlyException("DmsDocumentNotFound", "The requested document was not found.");
        }

        var folder = await _folderManager.GetAsync(document.FolderId);
        if (folder == null || folder.IsDeleted)
        {
            throw new UserFriendlyException(
                "DmsTrashRestoreFailed",
                "The original folder no longer exists; specify a target folder.");
        }

        document.IsDeleted = false;
        document.DeletionTime = null;
        document.DeleterUserId = null;
        document.Status = (int)RhyoliteOrbitConsts.DocumentStatus.Active;

        await _documentRepository.UpdateAsync(document);
    }

    public async Task PurgeAsync(Guid documentId, long purgedBy)
    {
        var document = await _documentRepository.FirstOrDefaultAsync(documentId);
        if (document == null)
        {
            throw new UserFriendlyException("DmsDocumentNotFound", "The requested document was not found.");
        }

        if (document.IsOnLegalHold || document.IsWorm)
        {
            throw new UserFriendlyException(
                "DmsDocumentLocked",
                "The document is on legal hold or write protected and cannot be modified.");
        }

        var versions = await _versionManager.ListAsync(documentId);
        var provider = _storageProviderFactory.GetProvider(document.StorageProvider);

        foreach (var version in versions)
        {
            try
            {
                await provider.DeleteAsync(version.StorageKey);
            }
            catch (Exception ex)
            {
                Logger.Warn($"Could not delete binary for version {version.Id}: {ex.Message}");
            }
        }

        var tags = await (await _documentTagRepository.GetAllAsync())
            .Where(t => t.DocumentId == documentId)
            .ToListAsync();

        foreach (var tag in tags)
        {
            await _documentTagRepository.HardDeleteAsync(tag);
        }

        var values = await (await _metadataValueRepository.GetAllAsync())
            .Where(v => v.DocumentId == documentId)
            .ToListAsync();

        foreach (var value in values)
        {
            await _metadataValueRepository.HardDeleteAsync(value);
        }

        await _documentRepository.HardDeleteAsync(document);
    }

    public async Task SetStarredAsync(Guid documentId, bool starred, long userId)
    {
        var document = await GetAsync(documentId);
        if (document == null)
        {
            throw new UserFriendlyException("DmsDocumentNotFound", "The requested document was not found.");
        }

        document.IsStarred = starred;
        await _documentRepository.UpdateAsync(document);
    }

    public async Task<DmsDocument> GetAsync(Guid documentId)
    {
        return await _documentRepository.FirstOrDefaultAsync(documentId);
    }

    public async Task<DmsDocument> GetByCustomIdAsync(string customId)
    {
        if (string.IsNullOrWhiteSpace(customId))
        {
            return null;
        }

        return await (await _documentRepository.GetAllAsync())
            .FirstOrDefaultAsync(d => d.CustomId == customId.Trim());
    }

    public async Task<PagedDocumentResult> ListAsync(DocumentListRequest request)
    {
        if (request.PageNo < 1)
        {
            request.PageNo = 1;
        }

        if (request.PageSize < 1)
        {
            request.PageSize = 20;
        }

        var query = await _documentRepository.GetAllAsync();

        if (request.FolderId.HasValue)
        {
            query = query.Where(d => d.FolderId == request.FolderId.Value);
        }

        if (request.CollectionId.HasValue)
        {
            query = query.Where(d => d.CollectionId == request.CollectionId.Value);
        }

        if (request.TagId.HasValue)
        {
            var documentIds = (await _documentTagRepository.GetAllAsync())
                .Where(t => t.TagId == request.TagId.Value)
                .Select(t => t.DocumentId);

            query = query.Where(d => documentIds.Contains(d.Id));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(d => d.Status == request.Status.Value);
        }

        if (request.OnlyStarred)
        {
            query = query.Where(d => d.IsStarred);
        }

        if (!request.IncludeDeleted)
        {
            query = query.Where(d => !d.IsDeleted);
        }

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var term = request.Query.Trim().ToLower();
            query = query.Where(d =>
                d.Name.ToLower().Contains(term) ||
                (d.Title != null && d.Title.ToLower().Contains(term)) ||
                (d.CustomId != null && d.CustomId.ToLower().Contains(term)) ||
                (d.FileName != null && d.FileName.ToLower().Contains(term)));
        }

        query = request.SortOrder switch
        {
            1 => query.OrderByDescending(d => d.CreationTime),
            2 => query.OrderByDescending(d => d.LastModificationTime ?? d.CreationTime),
            3 => query.OrderByDescending(d => d.FileSize),
            _ => query.OrderBy(d => d.Name)
        };

        var totalCount = await query.CountAsync();
        var data = await query.Skip((request.PageNo - 1) * request.PageSize).Take(request.PageSize).ToListAsync();

        return new PagedDocumentResult
        {
            PageNo = request.PageNo,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalCount > 0 ? (int)Math.Ceiling(totalCount / (double)request.PageSize) : 0,
            Data = data
        };
    }

    public async Task CheckOutAsync(Guid documentId, long userId, TimeSpan? lockDuration = null)
    {
        var document = await GetAsync(documentId);
        if (document == null)
        {
            throw new UserFriendlyException("DmsDocumentNotFound", "The requested document was not found.");
        }

        await EnsureWritableAsync(document);

        if (document.IsCheckedOut && document.CheckedOutBy != userId)
        {
            if (document.LockExpiry.HasValue && document.LockExpiry.Value > DateTime.UtcNow)
            {
                throw new UserFriendlyException(
                    "DmsDocumentCheckedOut",
                    $"The document is checked out by another user until {document.LockExpiry.Value:dd-MMM-yyyy HH:mm}.");
            }
        }

        document.IsCheckedOut = true;
        document.CheckedOutBy = userId;
        document.CheckedOutAt = DateTime.UtcNow;
        document.LockExpiry = DateTime.UtcNow.Add(lockDuration ?? TimeSpan.FromHours(12));

        await _documentRepository.UpdateAsync(document);
    }

    public async Task CheckInAsync(Guid documentId, long userId)
    {
        var document = await GetAsync(documentId);
        if (document == null)
        {
            throw new UserFriendlyException("DmsDocumentNotFound", "The requested document was not found.");
        }

        document.IsCheckedOut = false;
        document.CheckedOutBy = null;
        document.CheckedOutAt = null;
        document.LockExpiry = null;

        await _documentRepository.UpdateAsync(document);
    }

    public async Task ForceUnlockAsync(Guid documentId, long unlockedBy)
    {
        var document = await GetAsync(documentId);
        if (document == null)
        {
            throw new UserFriendlyException("DmsDocumentNotFound", "The requested document was not found.");
        }

        document.IsCheckedOut = false;
        document.CheckedOutBy = null;
        document.CheckedOutAt = null;
        document.LockExpiry = null;

        await _documentRepository.UpdateAsync(document);
    }

    public async Task<Stream> OpenReadAsync(Guid documentId, Guid? versionId = null)
    {
        var version = versionId.HasValue
            ? await _versionManager.GetAsync(versionId.Value)
            : await _versionManager.GetCurrentAsync(documentId);

        if (version == null)
        {
            throw new UserFriendlyException("DmsVersionNotFound", "The requested document version was not found.");
        }

        return await _versionManager.OpenReadAsync(version.Id);
    }

    public async Task<bool> IsDuplicateAsync(string checksum)
    {
        if (string.IsNullOrEmpty(checksum))
        {
            return false;
        }

        return await (await _documentRepository.GetAllAsync())
            .AnyAsync(d => d.Checksum == checksum && !d.IsDeleted);
    }

    public async Task SetRetentionAsync(Guid documentId, Guid? policyId, DateTime? eventDate, long userId)
    {
        var document = await GetAsync(documentId);
        if (document == null)
        {
            throw new UserFriendlyException("DmsDocumentNotFound", "The requested document was not found.");
        }

        document.RetentionPolicyId = policyId;
        document.RetentionEventDate = eventDate;

        await _documentRepository.UpdateAsync(document);
    }

    public async Task SetLegalHoldAsync(Guid documentId, bool onHold, long userId)
    {
        var document = await GetAsync(documentId);
        if (document == null)
        {
            throw new UserFriendlyException("DmsDocumentNotFound", "The requested document was not found.");
        }

        document.IsOnLegalHold = onHold;
        await _documentRepository.UpdateAsync(document);
    }

    public async Task SetWormAsync(Guid documentId, bool worm, long userId)
    {
        var document = await GetAsync(documentId);
        if (document == null)
        {
            throw new UserFriendlyException("DmsDocumentNotFound", "The requested document was not found.");
        }

        document.IsWorm = worm;
        await _documentRepository.UpdateAsync(document);
    }

    public async Task SaveMetadataAsync(Guid documentId, Dictionary<Guid, string> values, long updatedBy)
    {
        var document = await GetAsync(documentId);
        if (document == null)
        {
            throw new UserFriendlyException("DmsDocumentNotFound", "The requested document was not found.");
        }

        await EnsureWritableAsync(document);

        var existing = await (await _metadataValueRepository.GetAllAsync())
            .Where(v => v.DocumentId == documentId)
            .ToListAsync();

        foreach (var value in values ?? new Dictionary<Guid, string>())
        {
            var current = existing.FirstOrDefault(v => v.AttributeId == value.Key);

            if (current == null)
            {
                await _metadataValueRepository.InsertAsync(new DmsDocumentMetadataValues
                {
                    TenantId = document.TenantId,
                    DocumentId = documentId,
                    AttributeId = value.Key,
                    Value = value.Value
                });
            }
            else
            {
                current.Value = value.Value;
                await _metadataValueRepository.UpdateAsync(current);
            }
        }
    }

    public async Task AssignTagsAsync(Guid documentId, List<Guid> tagIds, long updatedBy)
    {
        var document = await GetAsync(documentId);
        if (document == null)
        {
            throw new UserFriendlyException("DmsDocumentNotFound", "The requested document was not found.");
        }

        var existing = await (await _documentTagRepository.GetAllAsync())
            .Where(t => t.DocumentId == documentId)
            .ToListAsync();

        foreach (var existingTag in existing)
        {
            await _documentTagRepository.HardDeleteAsync(existingTag);
        }

        foreach (var tagId in tagIds ?? new List<Guid>())
        {
            await _documentTagRepository.InsertAsync(new DmsDocumentTag
            {
                TenantId = document.TenantId,
                DocumentId = documentId,
                TagId = tagId
            });
        }
    }

    private async Task ValidateFileAsync(DmsSettings settings, string extension, Stream content)
    {
        var maxSizeMb = settings != null && settings.MaxFileSizeMb > 0 ? settings.MaxFileSizeMb : 512;
        var maxBytes = maxSizeMb * 1024L * 1024L;

        if (content != null && content.CanSeek && content.Length > maxBytes)
        {
            throw new UserFriendlyException(
                "DmsFileTooLarge",
                $"The file exceeds the maximum allowed size of {maxSizeMb} MB.");
        }

        var allowed = settings?.AllowedFileTypes;

        if (allowed != null && allowed.Any() && !string.IsNullOrEmpty(extension))
        {
            if (!allowed.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                throw new UserFriendlyException(
                    "DmsFileTypeNotAllowed",
                    $"Files of type {extension} are not allowed.");
            }
        }
    }

    private static async Task EnsureWritableAsync(DmsDocument document)
    {
        if (document.IsOnLegalHold || document.IsWorm)
        {
            throw new Abp.UI.UserFriendlyException(
                "DmsDocumentLocked",
                "The document is on legal hold or write protected and cannot be modified.");
        }

        await Task.CompletedTask;
    }

    private async Task<string> SuggestCopyNameAsync(string name, Guid folderId)
    {
        var extension = Path.GetExtension(name);
        var baseName = Path.GetFileNameWithoutExtension(name);

        var existing = await (await _documentRepository.GetAllAsync())
            .Where(d => d.FolderId == folderId && !d.IsDeleted)
            .Select(d => d.Name)
            .ToListAsync();

        var candidate = $"{baseName} (copy){extension}";
        var counter = 1;

        while (existing.Any(n => string.Equals(n, candidate, StringComparison.OrdinalIgnoreCase)))
        {
            counter++;
            candidate = $"{baseName} (copy {counter}){extension}";
        }

        return candidate;
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
