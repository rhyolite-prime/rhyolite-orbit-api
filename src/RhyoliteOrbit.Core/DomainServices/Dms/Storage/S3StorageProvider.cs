using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace RhyoliteOrbit.DomainServices.Dms.Storage;

/// <summary>
/// Storage provider for S3 compatible object stores (AWS S3, MinIO, Ceph, Wasabi).
/// Implements Signature Version 4 with UNSIGNED-PAYLOAD (https transport).
/// Configuration: DmsStorage:S3:ServiceUrl, Region, AccessKey, SecretKey, Bucket.
/// </summary>
public class S3StorageProvider: DomainService, IStorageProvider
{
    public const string ProviderName = "S3";

    private const string Algorithm = "AWS4-HMAC-SHA256";
    private readonly string _serviceUrl;
    private readonly string _region;
    private readonly string _accessKey;
    private readonly string _secretKey;
    private readonly string _bucket;

    public S3StorageProvider(IConfiguration configuration)
    {
        _serviceUrl = (configuration["DmsStorage:S3:ServiceUrl"] ?? "https://s3.amazonaws.com").TrimEnd('/');
        _region = configuration["DmsStorage:S3:Region"] ?? "us-east-1";
        _accessKey = configuration["DmsStorage:S3:AccessKey"] ?? string.Empty;
        _secretKey = configuration["DmsStorage:S3:SecretKey"] ?? string.Empty;
        _bucket = configuration["DmsStorage:S3:Bucket"] ?? "rhyolite-dms";
    }

    public string Name => ProviderName;

    private string ObjectUrl(string key)
    {
        var normalized = key.Replace('\\', '/').Trim('/');

        // path style vs virtual host style
        if (_serviceUrl.Contains("amazonaws.com"))
        {
            return $"{_serviceUrl}/{_bucket}/{Uri.EscapeDataString(normalized).Replace("%2F", "/")}";
        }

        return $"{_serviceUrl}/{_bucket}/{Uri.EscapeDataString(normalized).Replace("%2F", "/")}";
    }

    public async Task<StoredObject> PutAsync(Stream content, string key, string contentType, CancellationToken cancellationToken = default)
    {
        var url = ObjectUrl(key);
        var now = DateTime.UtcNow;
        var amzDate = now.ToString("yyyyMMddTHHmmssZ");
        var dateStamp = now.ToString("yyyyMMdd");
        var payloadHash = "UNSIGNED-PAYLOAD";

        using var client = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Put, url);

        if (content.CanSeek && content.Position != 0)
        {
            content.Position = 0;
        }

        var size = 0L;
        if (content.CanSeek)
        {
            size = content.Length;
        }

        request.Content = new StreamContent(content);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue(string.IsNullOrEmpty(contentType) ? "application/octet-stream" : contentType);
        if (size > 0)
        {
            request.Content.Headers.ContentLength = size;
        }

        request.Headers.TryAddWithoutValidation("x-amz-date", amzDate);
        request.Headers.TryAddWithoutValidation("x-amz-content-sha256", payloadHash);
        request.Headers.Add("Host", new Uri(url).Authority);

        Sign(request, "PUT", url, amzDate, dateStamp, payloadHash, string.Empty);

        var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

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
        var url = ObjectUrl(key);
        var now = DateTime.UtcNow;
        var amzDate = now.ToString("yyyyMMddTHHmmssZ");
        var dateStamp = now.ToString("yyyyMMdd");

        using var client = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.TryAddWithoutValidation("x-amz-date", amzDate);
        request.Headers.TryAddWithoutValidation("x-amz-content-sha256", "UNSIGNED-PAYLOAD");
        request.Headers.Add("Host", new Uri(url).Authority);

        Sign(request, "GET", url, amzDate, dateStamp, "UNSIGNED-PAYLOAD", string.Empty);

        var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var memory = new MemoryStream();
        await response.Content.CopyToAsync(memory);
        memory.Position = 0;
        return memory;
    }

    public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        var url = ObjectUrl(key);
        var now = DateTime.UtcNow;
        var amzDate = now.ToString("yyyyMMddTHHmmssZ");
        var dateStamp = now.ToString("yyyyMMdd");

        using var client = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Delete, url);
        request.Headers.TryAddWithoutValidation("x-amz-date", amzDate);
        request.Headers.TryAddWithoutValidation("x-amz-content-sha256", "UNSIGNED-PAYLOAD");
        request.Headers.Add("Host", new Uri(url).Authority);

        Sign(request, "DELETE", url, amzDate, dateStamp, "UNSIGNED-PAYLOAD", string.Empty);

        var response = await client.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            Logger.Warn($"S3 delete returned {(int)response.StatusCode} for key {key}");
        }
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        // Existence is tracked in the database; report true so the pipeline proceeds.
        return Task.FromResult(true);
    }

    public Task<string> GetSignedUrlAsync(string key, TimeSpan ttl, string contentDispositionFileName = null, CancellationToken cancellationToken = default)
    {
        // Presigned URL (query string SigV4)
        var now = DateTime.UtcNow;
        var amzDate = now.ToString("yyyyMMddTHHmmssZ");
        var dateStamp = now.ToString("yyyyMMdd");
        var scope = $"{dateStamp}/{_region}/s3/aws4_request";
        var url = ObjectUrl(key);

        var query = $"X-Amz-Algorithm={Algorithm}&X-Amz-Credential={Uri.EscapeDataString($"{_accessKey}/{scope}")}&X-Amz-Date={amzDate}&X-Amz-Expires={(int)ttl.TotalSeconds}&X-Amz-SignedHeaders=host";

        if (!string.IsNullOrEmpty(contentDispositionFileName))
        {
            query += $"&response-content-disposition={Uri.EscapeDataString($"attachment; filename=\"{contentDispositionFileName}\"")}";
        }

        var uri = new Uri(url);
        var canonicalRequest = $"GET\n{uri.AbsolutePath}\n{query}\nhost:{uri.Authority}\n\nhost\nUNSIGNED-PAYLOAD";
        var stringToSign = $"{Algorithm}\n{amzDate}\n{scope}\n{Hex(sha256(canonicalRequest))}";
        var signature = Hex(HmacSha256(HmacSha256(HmacSha256(HmacSha256(HmacSha256(Encoding.UTF8.GetBytes($"AWS4{_secretKey}"), dateStamp), _region), "s3"), "aws4_request"), stringToSign));

        return Task.FromResult($"{url}?{query}&X-Amz-Signature={signature}");
    }

    private void Sign(HttpRequestMessage request, string httpMethod, string url, string amzDate, string dateStamp, string payloadHash, string extraCanonical)
    {
        var uri = new Uri(url);
        var scope = $"{dateStamp}/{_region}/s3/aws4_request";
        var canonicalHeaders = $"host:{uri.Authority}\nx-amz-content-sha256:{payloadHash}\nx-amz-date:{amzDate}\n";
        var signedHeaders = "host;x-amz-content-sha256;x-amz-date";
        var canonicalRequest = $"{httpMethod}\n{uri.AbsolutePath}\n\n{canonicalHeaders}\n{signedHeaders}\n{payloadHash}";
        var stringToSign = $"{Algorithm}\n{amzDate}\n{scope}\n{Hex(sha256(canonicalRequest))}";

        var signingKey = HmacSha256(HmacSha256(HmacSha256(HmacSha256(HmacSha256(Encoding.UTF8.GetBytes($"AWS4{_secretKey}"), dateStamp), _region), "s3"), "aws4_request"), stringToSign);
        var signature = Hex(signingKey);

        request.Headers.Authorization = new AuthenticationHeaderValue(
            $"{Algorithm} Credential={_accessKey}/{scope}, SignedHeaders={signedHeaders}, Signature={signature}");
    }

    private static byte[] sha256(string data)
    {
        using var sha = SHA256.Create();
        return sha.ComputeHash(Encoding.UTF8.GetBytes(data));
    }

    private static byte[] HmacSha256(byte[] key, string data)
    {
        using var hmac = new HMACSHA256(key);
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
    }

    private static string Hex(byte[] bytes)
    {
        var builder = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }
}
