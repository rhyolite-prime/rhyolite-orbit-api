using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.Dms.Ai;
using RhyoliteOrbit.DomainServices.Dms.Extraction;
using RhyoliteOrbit.DomainServices.Dms.Notifications;
using RhyoliteOrbit.DomainServices.Dms.Ocr;
using RhyoliteOrbit.DomainServices.Dms.Security;
using RhyoliteOrbit.DomainServices.Dms.Settings;
using RhyoliteOrbit.DomainServices.Dms.Versions;
using RhyoliteOrbit.Models.Dms;

namespace RhyoliteOrbit.DomainServices.Dms;

/// <summary>Result of running the document pipeline.</summary>
public class PipelineResult
{
    public bool Success { get; set; }

    public string Message { get; set; }

    public int ExtractedCharacters { get; set; }

    public bool OcrApplied { get; set; }

    public bool Infected { get; set; }
}

/// <summary>
/// Asynchronous processing of an uploaded binary: antivirus scan, text extraction,
/// OCR when needed, AI classification, PII detection and embedding generation.
/// </summary>
public interface IDocumentPipeline
{
    Task<PipelineResult> ProcessAsync(Guid documentId, Guid versionId, CancellationToken cancellationToken = default);
}

public class DocumentPipeline: DomainService, IDocumentPipeline
{
    private readonly IRepository<DmsDocument, Guid> _documentRepository;
    private readonly IVersionManager _versionManager;
    private readonly ITextExtractorFactory _textExtractorFactory;
    private readonly IAntivirusScanner _antivirusScanner;
    private readonly IOcrService _ocrService;
    private readonly IAiDocumentService _aiDocumentService;
    private readonly IEmbeddingService _embeddingService;
    private readonly IPiiDetector _piiDetector;
    private readonly IDmsSettingsManager _settingsManager;
    private readonly IRepository<DmsEmbedding, Guid> _embeddingRepository;
    private readonly IRepository<DmsTag, Guid> _tagRepository;
    private readonly IRepository<DmsDocumentTag, Guid> _documentTagRepository;
    private readonly IDmsNotifier _notifier;
    private readonly IAbpSession _abpSession;

    public DocumentPipeline(
        IRepository<DmsDocument, Guid> documentRepository,
        IVersionManager versionManager,
        ITextExtractorFactory textExtractorFactory,
        IAntivirusScanner antivirusScanner,
        IOcrService ocrService,
        IAiDocumentService aiDocumentService,
        IEmbeddingService embeddingService,
        IPiiDetector piiDetector,
        IDmsSettingsManager settingsManager,
        IRepository<DmsEmbedding, Guid> embeddingRepository,
        IRepository<DmsTag, Guid> tagRepository,
        IRepository<DmsDocumentTag, Guid> documentTagRepository,
        IDmsNotifier notifier,
        IAbpSession abpSession)
    {
        _documentRepository = documentRepository;
        _versionManager = versionManager;
        _textExtractorFactory = textExtractorFactory;
        _antivirusScanner = antivirusScanner;
        _ocrService = ocrService;
        _aiDocumentService = aiDocumentService;
        _embeddingService = embeddingService;
        _piiDetector = piiDetector;
        _settingsManager = settingsManager;
        _embeddingRepository = embeddingRepository;
        _tagRepository = tagRepository;
        _documentTagRepository = documentTagRepository;
        _notifier = notifier;
        _abpSession = abpSession;
    }

    public async Task<PipelineResult> ProcessAsync(Guid documentId, Guid versionId, CancellationToken cancellationToken = default)
    {
        var document = await _documentRepository.FirstOrDefaultAsync(documentId);
        if (document == null)
        {
            return new PipelineResult { Success = false, Message = "Document not found." };
        }

        var version = await _versionManager.GetAsync(versionId);
        if (version == null)
        {
            return new PipelineResult { Success = false, Message = "Version not found." };
        }

        var settings = await _settingsManager.GetAsync();
        var extension = version.Extension ?? string.Empty;

        await using var content = await _versionManager.OpenReadAsync(versionId);

        // 1. antivirus
        if (settings?.AntivirusEnabled == true)
        {
            document.ScanStatus = (int)RhyoliteOrbitConsts.ScanStatus.Queued;
            await _documentRepository.UpdateAsync(document);

            var verdict = await _antivirusScanner.ScanAsync(content, version.FileName, cancellationToken);

            if (verdict.IsInfected)
            {
                document.ScanStatus = (int)RhyoliteOrbitConsts.ScanStatus.Infected;
                document.Status = (int)RhyoliteOrbitConsts.DocumentStatus.Quarantined;
                document.IndexStatus = (int)RhyoliteOrbitConsts.IndexStatus.Failed;
                await _documentRepository.UpdateAsync(document);

                Logger.Error($"Virus detected in document {documentId}: {verdict.Signature}");

                await _notifier.PublishAsync("Document.Quarantined", documentId, new { verdict.Signature });

                return new PipelineResult
                {
                    Success = false,
                    Infected = true,
                    Message = $"The uploaded file was rejected by the antivirus scanner ({verdict.Signature})."
                };
            }

            document.ScanStatus = (int)RhyoliteOrbitConsts.ScanStatus.Clean;
            await _documentRepository.UpdateAsync(document);
        }

        // 2. text extraction
        document.IndexStatus = (int)RhyoliteOrbitConsts.IndexStatus.Indexing;
        await _documentRepository.UpdateAsync(document);

        string extracted = string.Empty;

        try
        {
            extracted = await _textExtractorFactory.ExtractAsync(content, extension, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.Warn($"Text extraction failed for document {documentId}: {ex.Message}");
        }

        var ocrApplied = false;

        // 3. OCR for scans and images without a text layer
        var needsOcr = string.IsNullOrWhiteSpace(extracted) &&
                       (extension == ".pdf" || _ocrService.IsImageLike(extension));

        if (needsOcr && settings?.OcrEnabled == true)
        {
            document.OcrStatus = (int)RhyoliteOrbitConsts.OcrStatus.Running;
            await _documentRepository.UpdateAsync(document);

            try
            {
                var language = settings.OcrLanguages != null && settings.OcrLanguages.Any()
                    ? settings.OcrLanguages.First()
                    : "eng";

                var ocrText = await _ocrService.RecognizeAsync(content, extension, language, cancellationToken);

                if (!string.IsNullOrWhiteSpace(ocrText))
                {
                    extracted = ocrText;
                    ocrApplied = true;
                    document.OcrStatus = (int)RhyoliteOrbitConsts.OcrStatus.Completed;
                }
                else
                {
                    document.OcrStatus = (int)RhyoliteOrbitConsts.OcrStatus.Failed;
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"OCR failed for document {documentId}: {ex.Message}");
                document.OcrStatus = (int)RhyoliteOrbitConsts.OcrStatus.Failed;
            }

            await _documentRepository.UpdateAsync(document);
        }

        await _versionManager.UpdateIndexStateAsync(versionId, extracted, ocrApplied, CountPages(extracted), settings?.DefaultLanguage ?? "en");

        // 4. AI classification
        if (settings?.AiEnabled == true)
        {
            document.AiStatus = (int)RhyoliteOrbitConsts.AiStatus.Running;
            await _documentRepository.UpdateAsync(document);

            try
            {
                var classification = await _aiDocumentService.ClassifyAsync(version.FileName, extension, extracted, cancellationToken);

                document.Language = classification.Language;
                document.AiStatus = (int)RhyoliteOrbitConsts.AiStatus.Completed;

                if (!string.IsNullOrWhiteSpace(classification.Summary) && string.IsNullOrWhiteSpace(document.Description))
                {
                    document.Description = classification.Summary;
                }

                await ApplySuggestedTagsAsync(document, classification.SuggestedTags);
            }
            catch (Exception ex)
            {
                Logger.Warn($"AI classification failed for document {documentId}: {ex.Message}");
                document.AiStatus = (int)RhyoliteOrbitConsts.AiStatus.Failed;
            }

            await _documentRepository.UpdateAsync(document);
        }

        // 5. PII detection (hints recorded in the audit log, values stay untouched)
        try
        {
            var pii = await _piiDetector.DetectAsync(extracted, cancellationToken);

            if (pii.Any())
            {
                var summary = pii.GroupBy(p => p.Type)
                    .ToDictionary(g => g.Key, g => g.Count());

                Logger.Info($"PII detected in document {documentId}: {string.Join(", ", summary.Select(s => $"{s.Key}x{s.Value}"))}");
            }
        }
        catch (Exception ex)
        {
            Logger.Warn($"PII detection failed for document {documentId}: {ex.Message}");
        }

        // 6. embeddings for semantic search
        try
        {
            var existingEmbeddings = await (await _embeddingRepository.GetAllAsync())
                .Where(e => e.VersionId == versionId)
                .ToListAsync();

            foreach (var existing in existingEmbeddings)
            {
                await _embeddingRepository.HardDeleteAsync(existing);
            }

            if (!string.IsNullOrWhiteSpace(extracted))
            {
                var chunks = await _embeddingService.EmbedAsync(extracted);

                foreach (var chunk in chunks)
                {
                    await _embeddingRepository.InsertAsync(new DmsEmbedding
                    {
                        TenantId = _abpSession.TenantId ?? 0,
                        DocumentId = documentId,
                        VersionId = versionId,
                        ChunkIndex = chunk.ChunkIndex,
                        ChunkText = chunk.Text,
                        VectorJson = Newtonsoft.Json.JsonConvert.SerializeObject(chunk.Vector),
                        Model = _embeddingService.ModelName
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Warn($"Embedding generation failed for document {documentId}: {ex.Message}");
        }

        document.IndexStatus = (int)RhyoliteOrbitConsts.IndexStatus.Indexed;
        document.IndexedAt = DateTime.UtcNow;
        await _documentRepository.UpdateAsync(document);

        await _notifier.PublishAsync("Document.Indexed", documentId, new
        {
            versionId,
            characters = extracted?.Length ?? 0,
            ocrApplied,
            document.Language
        });

        return new PipelineResult
        {
            Success = true,
            ExtractedCharacters = extracted?.Length ?? 0,
            OcrApplied = ocrApplied,
            Message = "Document processed."
        };
    }

    private async Task ApplySuggestedTagsAsync(DmsDocument document, System.Collections.Generic.List<string> suggestedTags)
    {
        if (suggestedTags == null || !suggestedTags.Any())
        {
            return;
        }

        var existing = await (await _documentTagRepository.GetAllAsync())
            .Where(t => t.DocumentId == document.Id)
            .Select(t => t.TagId)
            .ToListAsync();

        foreach (var name in suggestedTags.Take(5))
        {
            var tag = await (await _tagRepository.GetAllAsync())
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());

            if (tag == null)
            {
                tag = await _tagRepository.InsertAsync(new DmsTag
                {
                    TenantId = document.TenantId,
                    Name = name,
                    IsSystem = true,
                    ColorCode = "#6c757d"
                });
            }

            if (!existing.Contains(tag.Id))
            {
                await _documentTagRepository.InsertAsync(new DmsDocumentTag
                {
                    TenantId = document.TenantId,
                    DocumentId = document.Id,
                    TagId = tag.Id
                });
            }
        }
    }

    private static int CountPages(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        // rough estimate: ~3000 characters per page
        return Math.Max(1, (int)Math.Ceiling(text.Length / 3000.0));
    }
}
