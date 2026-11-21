using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RhyoliteOrbit.DomainServices.Dms.Ocr;

/// <summary>Optical character recognition for scanned documents and images.</summary>
public interface IOcrService
{
    bool IsImageLike(string extension);

    /// <summary>Returns the recognized text; empty when nothing was recognized.</summary>
    Task<string> RecognizeAsync(Stream content, string extension, string language, CancellationToken cancellationToken = default);
}
