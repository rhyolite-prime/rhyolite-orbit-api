using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Services;

namespace RhyoliteOrbit.DomainServices.Dms.Extraction;

/// <summary>
/// Best effort PDF text extractor: walks the PDF objects, inflates FlateDecode
/// streams and collects text from the Tj/TJ/'/" operators. Replaceable by a
/// dedicated library (PdfPig/iText) without touching callers.
/// </summary>
public class PdfTextExtractor: DomainService, ITextExtractor
{
    public bool CanExtract(string extension)
    {
        return extension == ".pdf";
    }

    public async Task<string> ExtractAsync(Stream content, string extension, CancellationToken cancellationToken = default)
    {
        if (content.CanSeek)
        {
            content.Position = 0;
        }

        byte[] bytes;
        using (var memory = new MemoryStream())
        {
            await content.CopyToAsync(memory, 81920, cancellationToken);
            bytes = memory.ToArray();
        }

        var raw = Encoding.Latin1.GetString(bytes);
        var builder = new StringBuilder();

        foreach (Match streamMatch in Regex.Matches(raw, "stream\\r?\\n([\\s\\S]*?)\\r?\\nendstream"))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var payload = streamMatch.Groups[1].Value;
            byte[] decoded;

            try
            {
                var compressed = Encoding.Latin1.GetBytes(payload);
                using var input = new MemoryStream(compressed, 2, compressed.Length - 2);
                using var deflate = new DeflateStream(input, CompressionMode.Decompress);
                using var output = new MemoryStream();
                deflate.CopyTo(output);
                decoded = output.ToArray();
            }
            catch
            {
                continue; // not a flate stream
            }

            var text = Encoding.Latin1.GetString(decoded);
            AppendTextOperators(text, builder);
        }

        // also handle uncompressed content streams
        foreach (Match block in Regex.Matches(raw, "BT([\\s\\S]*?)ET"))
        {
            AppendTextOperators(block.Value, builder);
        }

        return builder.ToString();
    }

    private static void AppendTextOperators(string content, StringBuilder builder)
    {
        foreach (Match match in Regex.Matches(content, "\\((?:\\\\.|[^\\\\()])*\\)\\s*Tj"))
        {
            builder.Append(UnescapePdfString(match.Value)).Append(' ');
        }

        foreach (Match match in Regex.Matches(content, "\\[[\\s\\S]*?\\]\\s*TJ"))
        {
            var block = match.Value;
            foreach (var part in Regex.Matches(block, "\\((?:\\\\.|[^\\\\()])*\\)"))
            {
                builder.Append(UnescapePdfString(part.Value)).Append(' ');
            }
        }
    }

    private static string UnescapePdfString(raw)
    {
        var inner = raw;
        var start = inner.IndexOf('(');
        var end = inner.LastIndexOf(')');

        if (start >= 0 && end > start)
        {
            inner = inner.Substring(start + 1, end - start - 1);
        }

        return Regex.Replace(inner, "\\\\([nrtbf()\\\\]|[0-7]{1,3})", m =>
        {
            var code = m.Groups[1].Value;
            switch (code)
            {
                case "n": return "\n";
                case "r": return "\r";
                case "t": return "\t";
                case "b": return "\b";
                case "f": return "\f";
                case "(": return "(";
                case ")": return ")";
                case "\\": return "\\";
                default: return ((char)Convert.ToInt32(code, 8)).ToString();
            }
        });
    }
}
