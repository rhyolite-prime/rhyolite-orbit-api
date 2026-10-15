using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Services;

namespace RhyoliteOrbit.DomainServices.Dms.Extraction;

/// <summary>
/// Extracts text from OOXML containers (docx, xlsx, pptx) by reading the XML parts
/// of the zip package. No external dependency required.
/// </summary>
public class OpenXmlTextExtractor: DomainService, ITextExtractor
{
    public bool CanExtract(string extension)
    {
        return extension == ".docx" || extension == ".xlsx" || extension == ".pptx";
    }

    public async Task<string> ExtractAsync(Stream content, string extension, CancellationToken cancellationToken = default)
    {
        if (content.CanSeek && content.Position != 0)
        {
            content.Position = 0;
        }

        var builder = new StringBuilder();

        using (var archive = new ZipArchive(content, ZipArchiveMode.Read, true))
        {
            foreach (var entry in archive.Entries)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var name = entry.FullName;

                if (name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase) &&
                    (name.IndexOf("document", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     name.IndexOf("slide", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     name.IndexOf("sharedStrings", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     name.IndexOf("sheet", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     name.IndexOf("notesSlide", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    using var entryStream = entry.Open();
                    using var reader = new StreamReader(entryStream, Encoding.UTF8, true, 81920, true);
                    var xml = await reader.ReadToEndAsync();

                    // text nodes: <w:t>, <a:t>, <t>
                    var matches = System.Text.RegularExpressions.Regex.Matches(xml, "<(?:w|a):t[^>]*>([\\s\\S]*?)</(?:w|a):t>");
                    foreach (System.Text.RegularExpressions.Match match in matches)
                    {
                        var value = System.Net.WebUtility.HtmlDecode(match.Groups[1].Value);
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            builder.Append(value).Append(' ');
                        }
                    }

                    var plainMatches = System.Text.RegularExpressions.Regex.Matches(xml, "<t[^>]*>([\\s\\S]*?)</t>");
                    foreach (System.Text.RegularExpressions.Match match in plainMatches)
                    {
                        var value = System.Net.WebUtility.HtmlDecode(match.Groups[1].Value);
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            builder.Append(value).Append(' ');
                        }
                    }
                }
            }
        }

        return builder.ToString().Trim();
    }
}
