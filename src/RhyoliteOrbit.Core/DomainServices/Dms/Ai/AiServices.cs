using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Dms;

namespace RhyoliteOrbit.DomainServices.Dms.Ai;

/// <summary>Result of an AI classification pass.</summary>
public class DocumentClassification
{
    public string DocumentType { get; set; }

    public string Summary { get; set; }

    public string Language { get; set; }

    public double Confidence { get; set; }

    public List<string> SuggestedTags { get; set; }

    public Dictionary<string, string> SuggestedMetadata { get; set; }

    public List<PiiHint> PiiHints { get; set; }
}

public class PiiHint
{
    public string Type { get; set; }

    public int Count { get; set; }
}

/// <summary>
/// AI enrichment of documents (classification, summarisation, tagging, PII hints).
/// The default implementation is a deterministic heuristic engine so the feature
/// works offline; an LLM backed implementation can be registered instead.
/// </summary>
public interface IAiDocumentService
{
    Task<DocumentClassification> ClassifyAsync(string fileName, string extension, string text, CancellationToken cancellationToken = default);

    Task<List<string>> SuggestTagsAsync(string text, int maxTags);
}

public class HeuristicAiDocumentService: DomainService, IAiDocumentService
{
    private static readonly Dictionary<string, string[]> DocumentTypeKeywords = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
    {
        ["Invoice"] = new[] { "invoice", "amount due", "bill to", "vat", "subtotal", "total due", "payment terms" },
        ["Receipt"] = new[] { "receipt", "paid", "change", "cashier", "transaction id" },
        ["Contract"] = new[] { "agreement", "party", "clause", "hereby", "terms and conditions", "signature" },
        ["Payslip"] = new[] { "payslip", "gross pay", "net pay", "ssnit", "paye", "allowance", "deduction" },
        ["Report"] = new[] { "report", "summary", "findings", "analysis", "conclusion", "quarterly" },
        ["Letter"] = new[] { "dear", "sincerely", "yours faithfully", "regards" },
        ["PurchaseOrder"] = new[] { "purchase order", "po number", "delivery date", "quantity ordered" },
        ["BankStatement"] = new[] { "statement", "opening balance", "closing balance", "debit", "credit", "account number" },
        ["Cv"] = new[] { "curriculum vitae", "resume", "experience", "education", "skills" }
    };

    public async Task<DocumentClassification> ClassifyAsync(string fileName, string extension, string text, CancellationToken cancellationToken = default)
    {
        var content = $"{fileName} {text}".ToLowerInvariant();

        var scores = DocumentTypeKeywords
            .Select(kvp => new { Type = kvp.Key, Score = kvp.Value.Count(keyword => content.Contains(keyword)) })
            .OrderByDescending(x => x.Score)
            .ToList();

        var best = scores.First();

        var classification = new DocumentClassification
        {
            DocumentType = best.Score > 0 ? best.Type : "Document",
            Confidence = best.Score > 0 ? Math.Min(1.0, best.Score / 4.0) : 0.2,
            Language = DetectLanguage(text),
            SuggestedTags = await SuggestTagsAsync(text, 5),
            SuggestedMetadata = new Dictionary<string, string>(),
            PiiHints = new List<PiiHint>()
        };

        classification.Summary = BuildSummary(text);

        return classification;
    }

    public Task<List<string>> SuggestTagsAsync(string text, int maxTags)
    {
        return SuggestTags(text, maxTags);
    }

    private static Task<List<string>> SuggestTags(string text, int maxTags)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Task.FromResult(new List<string>());
        }

        var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "the", "and", "for", "with", "that", "this", "from", "have", "are", "was", "were", "has", "had",
            "not", "but", "all", "can", "will", "you", "your", "our", "their", "there", "here", "when", "what"
        };

        var words = text.ToLowerInvariant()
            .Split(new[] { ' ', '\r', '\n', '\t', ',', '.', ';', ':', '(', ')', '/', '-' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length > 4 && !stopWords.Contains(w) && w.All(char.IsLetter))
            .GroupBy(w => w)
            .OrderByDescending(g => g.Count())
            .Take(maxTags)
            .Select(g => g.Key)
            .ToList();

        return Task.FromResult(words);
    }

    private static string BuildSummary(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var trimmed = text.Trim();

        return trimmed.Length <= 400 ? trimmed : trimmed.Substring(0, 400) + "...";
    }

    private static string DetectLanguage(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "en";
        }

        var ghanaMarkers = new[] { " Ɛ", "ɔ", "anaa", "yɛ", "naa" };
        var lower = text.ToLowerInvariant();

        return ghanaMarkers.Any(m => lower.Contains(m)) ? "tw" : "en";
    }
}

/// <summary>Chunk of text plus its vector, used by semantic search.</summary>
public class EmbeddedChunk
{
    public int ChunkIndex { get; set; }

    public string Text { get; set; }

    public float[] Vector { get; set; }
}

/// <summary>
/// Produces dense embeddings for document chunks. The default is a deterministic
/// hashing embedder (bag of words -> fixed size vector) so semantic search works
/// without an external model; swap for an OpenAI/Cohere/local model implementation.
/// </summary>
public interface IEmbeddingService
{
    int Dimensions { get; }

    string ModelName { get; }

    Task<List<EmbeddedChunk>> EmbedAsync(string text, int chunkSize = 800);

    Task<float[]> EmbedQueryAsync(string text);
}

public class HashingEmbeddingService: DomainService, IEmbeddingService
{
    public const int EmbeddingDimensions = 256;

    public int Dimensions => EmbeddingDimensions;

    public string ModelName => "hashing-bow-v1";

    public Task<List<EmbeddedChunk>> EmbedAsync(string text, int chunkSize = 800)
    {
        var chunks = new List<EmbeddedChunk>();

        if (string.IsNullOrWhiteSpace(text))
        {
            return Task.FromResult(chunks);
        }

        var normalized = text.Trim();
        var index = 0;

        while (index < normalized.Length)
        {
            var length = Math.Min(chunkSize, normalized.Length - index);
            var chunkText = normalized.Substring(index, length);
            index += length;

            chunks.Add(new EmbeddedChunk
            {
                ChunkIndex = chunks.Count,
                Text = chunkText,
                Vector = Embed(chunkText)
            });
        }

        return Task.FromResult(chunks);
    }

    public Task<float[]> EmbedQueryAsync(string text)
    {
        return Task.FromResult(Embed(text ?? string.Empty));
    }

    private static float[] Embed(string text)
    {
        var vector = new float[EmbeddingDimensions];

        foreach (var token in Tokenize(text))
        {
            // A stable hash is required: string.GetHashCode is randomised per
            // process, which would make the persisted vectors meaningless after
            // a restart. FNV-1a is deterministic across processes and machines.
            var slot = (int)(StableHash(token) % (uint)EmbeddingDimensions);
            vector[slot] += 1f;
        }

        var norm = (float)Math.Sqrt(vector.Sum(v => v * v));

        if (norm > 0)
        {
            for (var i = 0; i < vector.Length; i++)
            {
                vector[i] /= norm;
            }
        }

        return vector;
    }

    private static uint StableHash(string value)
    {
        // FNV-1a over the UTF-16 code units.
        uint hash = 2166136261;

        foreach (var character in value)
        {
            hash ^= character;
            hash *= 16777619;
        }

        return hash;
    }

    private static IEnumerable<string> Tokenize(string text)
    {
        return text.ToLowerInvariant()
            .Split(new[] { ' ', '\r', '\n', '\t', ',', '.', ';', ':', '(', ')', '/', '-', '"', '\'' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length > 2);
    }
}

/// <summary>Retrieval augmented generation over the tenant's documents.</summary>
public interface IRagService
{
    Task<object> AskAsync(string question, int maxChunks = 5);
}

/// <summary>
/// Extractor based question answering: embeds the question, retrieves the most
/// similar chunks and returns them with citations. An LLM can be layered on top
/// of the retrieved context without changing the contract.
/// </summary>
public class RagService: DomainService, IRagService
{
    private readonly IRepository<DmsEmbedding, Guid> _embeddingRepository;
    private readonly IRepository<DmsDocument, Guid> _documentRepository;
    private readonly IEmbeddingService _embeddingService;
    private readonly IAbpSession _abpSession;

    public RagService(
        IRepository<DmsEmbedding, Guid> embeddingRepository,
        IRepository<DmsDocument, Guid> documentRepository,
        IEmbeddingService embeddingService,
        IAbpSession abpSession)
    {
        _embeddingRepository = embeddingRepository;
        _documentRepository = documentRepository;
        _embeddingService = embeddingService;
        _abpSession = abpSession;
    }

    public async Task<object> AskAsync(string question, int maxChunks = 5)
    {
        if (maxChunks <= 0)
        {
            maxChunks = 5;
        }

        var queryVector = await _embeddingService.EmbedQueryAsync(question);

        var embeddings = await (await _embeddingRepository.GetAllAsync()).ToListAsync();

        var documentIds = embeddings.Select(e => e.DocumentId).Distinct().ToList();

        var documents = await _documentRepository.GetAll()
            .Where(d => documentIds.Contains(d.Id))
            .ToDictionaryAsync(d => d.Id, d => d.Name);

        var scored = embeddings
            .Select(e => new
            {
                Embedding = e,
                Score = CosineSimilarity(queryVector, Deserialize(e.VectorJson))
            })
            .Where(x => x.Score > 0.01f)
            .OrderByDescending(x => x.Score)
            .Take(maxChunks)
            .ToList();

        return new
        {
            Question = question,
            Answer = scored.Any()
                ? string.Join(" ", scored.Select(s => Truncate(s.Embedding.ChunkText, 500)))
                : "No relevant documents were found for this question.",
            Citations = scored.Select(s => new
            {
                s.Embedding.DocumentId,
                DocumentName = documents.TryGetValue(s.Embedding.DocumentId, out var name) ? name : null,
                s.Embedding.ChunkIndex,
                Score = Math.Round(s.Score, 4)
            })
        };
    }

    private static float[] Deserialize(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return Array.Empty<float>();
        }

        try
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<float[]>(json) ?? Array.Empty<float>();
        }
        catch
        {
            return Array.Empty<float>();
        }
    }

    private static float CosineSimilarity(float[] a, float[] b)
    {
        if (a.Length == 0 || b.Length == 0)
        {
            return 0f;
        }

        var length = Math.Min(a.Length, b.Length);
        float dot = 0f;

        for (var i = 0; i < length; i++)
        {
            dot += a[i] * b[i];
        }

        return dot;
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
    }
}
