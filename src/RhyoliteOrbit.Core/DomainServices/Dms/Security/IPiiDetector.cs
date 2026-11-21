using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RhyoliteOrbit.DomainServices.Dms.Security;

/// <summary>Detects personally identifiable information inside document text.</summary>
public interface IPiiDetector
{
    Task<List<PiiMatch>> DetectAsync(string text, CancellationToken cancellationToken = default);

    string Mask(string text);
}

public class PiiMatch
{
    /// <summary>Email, Phone, GhanaCard, Ssnit, Tin, Card, BankAccount</summary>
    public string Type { get; set; }

    public int Start { get; set; }

    public int Length { get; set; }

    public string Value { get; set; }
}
