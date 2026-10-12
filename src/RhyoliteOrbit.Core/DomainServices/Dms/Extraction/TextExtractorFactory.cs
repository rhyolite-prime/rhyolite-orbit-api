using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp.Dependency;

namespace RhyoliteOrbit.DomainServices.Dms.Extraction;

/// <summary>Routes a binary to the first extractor that can handle its extension.</summary>
public class TextExtractorFactory: ITextExtractorFactory, ITransientDependency
{
    private readonly IEnumerable<ITextExtractor> _extractors;

    public TextExtractorFactory(IEnumerable<ITextExtractor> extractors)
    {
        _extractors = extractors;
    }

    public ITextExtractor Resolve(string extension)
    {
        return _extractors.FirstOrDefault(e => e.CanExtract(extension));
    }

    public async Task<string> ExtractAsync(Stream content, string extension, CancellationToken cancellationToken = default)
    {
        var extractor = Resolve(extension);
        if (extractor == null)
        {
            return string.Empty;
        }

        return await extractor.ExtractAsync(content, extension, cancellationToken);
    }
}

public interface ITextExtractorFactory
{
    ITextExtractor Resolve(string extension);

    Task<string> ExtractAsync(Stream content, string extension, CancellationToken cancellationToken = default);
}
