using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RhyoliteOrbit.DomainServices.Dms.Storage;

/// <summary>Result of a successful write to a storage provider.</summary>
public class StoredObject
{
    public string Provider { get; set; }

    public string Key { get; set; }

    public string Url { get; set; }

    public long Size { get; set; }

    public string ContentType { get; set; }
}

/// <summary>
/// Abstraction over binary storage (Cloudinary, local disk, S3 compatible object stores).
/// Keys are always relative: {tenantId}/{collectionId}/{folderId}/{documentId}/{versionId}/{fileName}
/// </summary>
public interface IStorageProvider
{
    string Name { get; }

    Task<StoredObject> PutAsync(Stream content, string key, string contentType, CancellationToken cancellationToken = default);

    Task<Stream> GetAsync(string key, CancellationToken cancellationToken = default);

    Task DeleteAsync(string key, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>Time limited direct URL when the provider supports it; otherwise null.</summary>
    Task<string> GetSignedUrlAsync(string key, TimeSpan ttl, string contentDispositionFileName = null, CancellationToken cancellationToken = default);
}
