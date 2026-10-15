using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RhyoliteOrbit.DomainServices.Dms.Ocr;

/// <summary>
/// OCR service that delegates to a Tesseract worker exposed over HTTP
/// (e.g. the tesseract-ocr-rest container). Configure with
/// "OcrService:BaseUrl" and "OcrService:ApiKey".
/// </summary>
public class TesseractOcrService: DomainService, IOcrService
{
    private static readonly string[] ImageExtensions = { ".png", ".jpg", ".jpeg", ".tif", ".tiff", ".bmp", ".gif", ".webp" };

    private readonly string _baseUrl;
    private readonly string _apiKey;

    public TesseractOcrService(IConfiguration configuration)
    {
        _baseUrl = configuration["OcrService:BaseUrl"] ?? "http://localhost:8884";
        _apiKey = configuration["OcrService:ApiKey"] ?? string.Empty;
    }

    public bool IsImageLike(string extension)
    {
        return Array.IndexOf(ImageExtensions, extension) >= 0;
    }

    public async Task<string> RecognizeAsync(Stream content, string extension, string language, CancellationToken cancellationToken = default)
    {
        if (content.CanSeek)
        {
            content.Position = 0;
        }

        var bytes = await ReadAllBytesAsync(content, cancellationToken);
        var base64 = Convert.ToBase64String(bytes);

        var client = new RestClient(_baseUrl);
        var request = new RestRequest("tesseract", Method.Post);
        request.AddJsonBody(new
        {
            data = base64,
            language = string.IsNullOrEmpty(language) ? "eng" : language
        });

        if (!string.IsNullOrEmpty(_apiKey))
        {
            request.AddHeader("apikey", _apiKey);
        }

        var response = await client.ExecuteAsync(request, cancellationToken);

        if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
        {
            Logger.Warn($"OCR request failed with status {response.StatusCode}");
            return string.Empty;
        }

        try
        {
            var json = JObject.Parse(response.Content);
            return json["data"]?["stdout"]?.ToString() ?? string.Empty;
        }
        catch
        {
            return response.Content;
        }
    }

    private static async Task<byte[]> ReadAllBytesAsync(Stream content, CancellationToken cancellationToken)
    {
        using var memory = new MemoryStream();
        await content.CopyToAsync(memory, 81920, cancellationToken);
        return memory.ToArray();
    }
}
