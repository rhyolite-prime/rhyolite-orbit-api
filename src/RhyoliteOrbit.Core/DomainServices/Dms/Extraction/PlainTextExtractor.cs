using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Services;

namespace RhyoliteOrbit.DomainServices.Dms.Extraction;

/// <summary>Handles plain text formats (txt, csv, json, xml, md, log, html).</summary>
public class PlainTextExtractor: DomainService, ITextExtractor
{
    private static readonly string[] Extensions =
    {
        ".txt", ".csv", ".json", ".xml", ".md", ".log", ".html", ".htm", ".yaml", ".yml", ".ini", ".cfg"
    };

    public bool CanExtract(string extension)
    {
        return Array.IndexOf(Extensions, extension) >= 0;
    }

    public async Task<string> ExtractAsync(Stream content, string extension, CancellationToken cancellationToken = default)
    {
        if (content.CanSeek && content.Position != 0)
        {
            content.Position = 0;
        }

        using var reader = new StreamReader(content, Encoding.UTF8, true, 81920, true);
        var text = await reader.ReadToEndAsync();

        if (extension == ".html" || extension == ".htm")
        {
            text = StripHtml(text);
        }

        return text;
    }

    internal static string StripHtml(string html)
    {
        if (string.IsNullOrEmpty(html))
        {
            return html;
        }

        var text = System.Text.RegularExpressions.Regex.Replace(html, "<script[^>]*>[\\s\\S]*?</script>", " ", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        text = System.Text.RegularExpressions.Regex.Replace(text, "<style[^>]*>[\\s\\S]*?</style>", " ", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        text = System.Text.RegularExpressions.Regex.Replace(text, "<[^>]+>", " ");
        text = System.Net.WebUtility.HtmlDecode(text);

        return System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();
    }
}
