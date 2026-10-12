using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RhyoliteOrbit.DomainServices.Dms.Extraction;

/// <summary>Extracts plain text from a binary for indexing and search.</summary>
public interface ITextExtractor
{
    /// <summary>File extensions (lower case, with dot) handled by this extractor.</summary>
    bool CanExtract(string extension);

    Task<string> ExtractAsync(Stream content, string extension, CancellationToken cancellationToken = default);
}
