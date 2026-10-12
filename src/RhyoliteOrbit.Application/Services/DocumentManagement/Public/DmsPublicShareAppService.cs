using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.Dms.Shares;
using RhyoliteOrbit.DomainServices.Dms.Storage;
using RhyoliteOrbit.Models.Dms;
using RhyoliteOrbit.Services.DocumentManagement.Dto;

namespace RhyoliteOrbit.Services.DocumentManagement.Public;

/// <summary>
/// Public (token authenticated) access to shared documents. The share token is
/// the only secret, therefore this service deliberately has no
/// <c>[AbpAuthorize]</c> attribute: the token itself authorises the request.
/// </summary>
public class DmsPublicShareAppService: ApplicationService
{
    private readonly IShareManager _shares;
    private readonly IRepository<DmsShare, Guid> _shareRepository;
    private readonly IRepository<DmsDocument, Guid> _documentRepository;
    private readonly IRepository<DmsDocumentVersion, Guid> _versionRepository;
    private readonly IStorageProviderFactory _storageFactory;

    public DmsPublicShareAppService(
        IShareManager shares,
        IRepository<DmsShare, Guid> shareRepository,
        IRepository<DmsDocument, Guid> documentRepository,
        IRepository<DmsDocumentVersion, Guid> versionRepository,
        IStorageProviderFactory storageFactory)
    {
        _shares = shares;
        _shareRepository = shareRepository;
        _documentRepository = documentRepository;
        _versionRepository = versionRepository;
        _storageFactory = storageFactory;
    }

    /// <summary>Resolves a public share token and describes the shared object.</summary>
    [HttpGet]
    public async Task<object> Resolve(string token, string password = null)
    {
        var share = await _shares.ResolveTokenAsync(token, password);

        var document = share.DocumentId.HasValue
            ? await (await _documentRepository.GetAllAsync())
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(d => d.Id == share.DocumentId.Value)
            : null;

        return new
        {
            shareType = share.ShareType,
            expiresAt = share.ExpiresAt,
            allowPreview = share.AllowPreview,
            allowDownload = share.AllowDownload,
            allowUpload = share.AllowUpload,
            requiresWatermark = share.RequiresWatermark,
            documentId = share.DocumentId,
            folderId = share.FolderId,
            documentName = document?.Name,
            fileName = document?.FileName,
            fileSize = document?.FileSize ?? 0,
            mimeType = document?.MimeType
        };
    }

    /// <summary>Streams the content of a shared document.</summary>
    [HttpGet]
    public async Task<IActionResult> Download(string token, string password = null)
    {
        var share = await _shares.ResolveTokenAsync(token, password);

        if (!share.AllowDownload)
        {
            throw new Abp.UI.UserFriendlyException("DmsShareDownloadDisabled", "Downloads are disabled for this link.");
        }

        if (!share.DocumentId.HasValue)
        {
            throw new Abp.UI.UserFriendlyException("DmsShareFolder", "Folder shares cannot be downloaded directly.");
        }

        var document = await (await _documentRepository.GetAllAsync())
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == share.DocumentId.Value);

        if (document == null)
        {
            throw new Abp.UI.UserFriendlyException("DmsDocumentNotFound", "The shared document no longer exists.");
        }

        await _shares.RegisterDownloadAsync(share.Id);

        var provider = await _storageFactory.GetProviderAsync(document.TenantId);
        var content = await provider.GetAsync(document.StorageKey);

        return new FileStreamResult(content, document.MimeType ?? "application/octet-stream")
        {
            FileDownloadName = document.FileName
        };
    }

    /// <summary>Returns a short lived preview URL for a shared document.</summary>
    [HttpGet]
    public async Task<DownloadResultDto> Preview(string token, string password = null)
    {
        var share = await _shares.ResolveTokenAsync(token, password);

        if (!share.AllowPreview)
        {
            throw new Abp.UI.UserFriendlyException("DmsSharePreviewDisabled", "Preview is disabled for this link.");
        }

        if (!share.DocumentId.HasValue)
        {
            throw new Abp.UI.UserFriendlyException("DmsShareFolder", "Folder shares cannot be previewed.");
        }

        var document = await (await _documentRepository.GetAllAsync())
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == share.DocumentId.Value);

        if (document == null)
        {
            throw new Abp.UI.UserFriendlyException("DmsDocumentNotFound", "The shared document no longer exists.");
        }

        var provider = await _storageFactory.GetProviderAsync(document.TenantId);
        var url = await provider.GetSignedUrlAsync(document.StorageKey, TimeSpan.FromMinutes(30), document.FileName);

        return new DownloadResultDto
        {
            FileName = document.FileName,
            ContentType = document.MimeType,
            Url = url,
            FileSize = document.FileSize
        };
    }
}
