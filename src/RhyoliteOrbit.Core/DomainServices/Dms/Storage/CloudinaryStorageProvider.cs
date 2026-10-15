using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Microsoft.Extensions.Configuration;
using RhyoliteOrbit.CloudinaryApi;

namespace RhyoliteOrbit.DomainServices.Dms.Storage;

/// <summary>
/// Storage provider backed by Cloudinary (the default in Rhyolite Orbit).
/// Reuses the existing <see cref="IFileUploadApi"/> upload primitive.
/// </summary>
public class CloudinaryStorageProvider: DomainService, IStorageProvider
{
    public const string ProviderName = "Cloudinary";

    private readonly IFileUploadApi _fileUploadApi;
    private readonly string _cloudName;

    public CloudinaryStorageProvider(IFileUploadApi fileUploadApi, IConfiguration configuration)
    {
        _fileUploadApi = fileUploadApi;
        _cloudName = configuration["CloudinarySettings:Cloud"] ?? "rhyoliteprime";
    }

    public string Name => ProviderName;

    public async Task<StoredObject> PutAsync(Stream content, string key, string contentType, CancellationToken cancellationToken = default)
    {
        var size = 0L;
        try
        {
            if (content.CanSeek)
            {
                size = content.Length;
                if (content.Position != 0)
                {
                    content.Position = 0;
                }
            }
        }
        catch
        {
            // length is best effort only
        }

        var extension = Path.GetExtension(key) ?? string.Empty;
        var fileName = Path.GetFileNameWithoutExtension(key) ?? Guid.NewGuid().ToString("N");
        var folder = (Path.GetDirectoryName(key) ?? string.Empty).Replace('\\', '/');

        var url = await _fileUploadApi.UploadFileStream(content, fileName, extension, folder);

        return new StoredObject
        {
            Provider = Name,
            Key = key,
            Url = url,
            Size = size,
            ContentType = contentType
        };
    }

    public async Task<Stream> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        var url = await GetSignedUrlAsync(key, TimeSpan.FromMinutes(15));
        if (string.IsNullOrEmpty(url))
        {
            throw new Abp.UI.UserFriendlyException("The document binary could not be resolved from storage.");
        }

        using var client = new System.Net.Http.HttpClient();
        var response = await client.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var memory = new MemoryStream();
        await response.Content.CopyToAsync(memory);
        memory.Position = 0;
        return memory;
    }

    public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        // Document and version rows are removed immediately so the binary becomes
        // unreachable; physical deletion is completed by the retention job.
        Logger.Info($"Cloudinary delete requested for key {key}");
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task<string> GetSignedUrlAsync(string key, TimeSpan ttl, string contentDispositionFileName = null, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(key) ?? string.Empty;
        var fileName = Path.GetFileNameWithoutExtension(key) ?? key;
        var folder = (Path.GetDirectoryName(key) ?? string.Empty).Replace('\\', '/');
        var publicId = string.IsNullOrEmpty(folder) ? fileName : $"{folder}/{fileName}";

        var url = $"https://res.cloudinary.com/{_cloudName}/raw/upload/{publicId}{extension}";

        if (!string.IsNullOrEmpty(contentDispositionFileName))
        {
            url = $"{url}?fl_attachment={Uri.EscapeDataString(contentDispositionFileName)}";
        }

        return Task.FromResult(url);
    }
}
