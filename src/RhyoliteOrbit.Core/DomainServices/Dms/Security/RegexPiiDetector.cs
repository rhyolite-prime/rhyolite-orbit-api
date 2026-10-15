using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Services;

namespace RhyoliteOrbit.DomainServices.Dms.Security;

/// <summary>
/// Rule based PII detection covering the identifiers common in Ghana and the
/// international formats (email, phone, cards, SSN/TIN style numbers).
/// </summary>
public class RegexPiiDetector: DomainService, IPiiDetector
{
    private static readonly List<(string Type, Regex Pattern)> Rules = new List<(string, Regex)>
    {
        ("Email", new Regex(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b", RegexOptions.Compiled)),
        ("GhanaCard", new Regex(@"\bGHA-\d{9}-\d\b", RegexOptions.Compiled)),
        ("Ssnit", new Regex(@"\b[CU]\d{9,12}\b", RegexOptions.Compiled)),
        ("Tin", new Regex(@"\bP00\d{8}\b", RegexOptions.Compiled)),
        ("Phone", new Regex(@"\b(?:\+233|0)\d{2}[\s-]?\d{3}[\s-]?\d{4}\b", RegexOptions.Compiled)),
        ("Card", new Regex(@"\b(?:\d[ -]*?){13,16}\b", RegexOptions.Compiled)),
        ("BankAccount", new Regex(@"\b\d{10,16}\b", RegexOptions.Compiled))
    };

    public Task<List<PiiMatch>> DetectAsync(string text, CancellationToken cancellationToken = default)
    {
        var matches = new List<PiiMatch>();

        if (string.IsNullOrEmpty(text))
        {
            return Task.FromResult(matches);
        }

        foreach (var (type, pattern) in Rules)
        {
            foreach (Match match in pattern.Matches(text))
            {
                cancellationToken.ThrowIfCancellationRequested();

                matches.Add(new PiiMatch
                {
                    Type = type,
                    Start = match.Index,
                    Length = match.Length,
                    Value = match.Value
                });
            }
        }

        return Task.FromResult(matches);
    }

    public string Mask(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        var masked = text;
        foreach (var (_, pattern) in Rules)
        {
            masked = pattern.Replace(masked, m =>
            {
                var value = m.Value;
                if (value.Length <= 4)
                {
                    return new string('*', value.Length);
                }

                return value.Substring(0, 2) + new string('*', value.Length - 4) + value.Substring(value.Length - 2);
            });
        }

        return masked;
    }
}
