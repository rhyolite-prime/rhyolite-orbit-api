using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace RhyoliteOrbit.DomainServices.Dms.Storage;

/// <summary>
/// Stores binaries on the local file system (development and on-premise deployments).
/// The root path is taken from configuration "DmsStorage:LocalRootPath" or /var/dms.
/// </summary>
public class LocalStorageProvider: DomainService, IStorageProvider
{
    public const string ProviderName = "Local";

    private readonly string _rootPath;

    public LocalStorageProvider(IConfiguration configuration)
    {
        _rootPath = configuration["DmsStorage:LocalRootPath"] ?? "/var/dms";
    }

    public string Name => ProviderName;

    private string ResolvePath(string key)
    {
        var normalized = key.Replace('\\', '/').Trim('/');
        var full = Path.GetFullPath(Path.Combine(_rootPath, normalized));

        if (!full.StartsWith(Path.GetFullPath(_rootPath), StringComparison.Ordinal))
        {
            throw new Abp.UI.UserFriendlyException("Invalid storage key.");
        }

        return full;
    }

    public async Task<StoredObject> PutAsync(Stream content, string key, string contentType, CancellationToken cancellationToken = default)
    {
        var full = ResolvePath(key);
        var directory = Path.GetDirectoryName(full);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        long size = 0;
        if (content.CanSeek)
        {
            size = content.Length;
            if (content.Position != 0)
            {
                content.Position = 0;
            }
        }

        using (var fileStream = new FileStream(full, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await content.CopyToAsync(fileStream, 81920, cancellationToken);
        }

        return new StoredObject
        {
            Provider = Name,
            Key = key,
            Url = $"/api/services/app/DmsFile/Download?key={Uri.EscapeDataString(key)}",
            Size = size,
            ContentType = contentType
        };
    }

    public Task<Stream> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        var full = ResolvePath(key);

        if (!File.Exists(full))
        {
            throw new Abp.UI.UserFriendlyException("The document binary was not found in storage.");
        }

        Stream stream = new FileStream(full, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        var full = ResolvePath(key);

        if (File.Exists(full))
        {
            File.Delete(full);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(File.Exists(ResolvePath(key)));
    }

    public Task<string> GetSignedUrlAsync(string key, TimeSpan ttl, string contentDispositionFileName = null, CancellationToken cancellationToken = default)
    {
        var url = $"/api/services/app/DmsFile/Download?key={Uri.EscapeDataString(key)}";
        return Task.FromResult(url);
    }
}
