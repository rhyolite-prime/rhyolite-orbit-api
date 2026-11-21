using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RhyoliteOrbit.DomainServices.Dms.Security;

/// <summary>Scans binaries for malware before they are published.</summary>
public interface IAntivirusScanner
{
    Task<ScanVerdict> ScanAsync(Stream content, string fileName, CancellationToken cancellationToken = default);
}

public class ScanVerdict
{
    public bool IsInfected { get; set; }

    public string Signature { get; set; }

    public bool ScannerAvailable { get; set; }

    public static ScanVerdict Clean()
    {
        return new ScanVerdict { IsInfected = false, ScannerAvailable = true };
    }

    public static ScanVerdict Unavailable()
    {
        return new ScanVerdict { IsInfected = false, ScannerAvailable = false };
    }
}
