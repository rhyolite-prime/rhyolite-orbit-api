using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using RhyoliteOrbit.DomainServices.Dms.Extraction;
using Shouldly;
using Xunit;

namespace RhyoliteOrbit.Tests.Dms;

/// <summary>Unit tests for the text extractors and the routing factory.</summary>
public class DmsTextExtraction_Tests
{
    private readonly ITextExtractorFactory _factory;

    public DmsTextExtraction_Tests()
    {
        _factory = new TextExtractorFactory(new ITextExtractor[]
        {
            new PlainTextExtractor(),
            new OpenXmlTextExtractor(),
            new PdfTextExtractor()
        });
    }

    [Fact]
    public async Task Should_Extract_Plain_Text()
    {
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("line one\nline two"));

        (await _factory.ExtractAsync(stream, ".txt")).ShouldBe("line one\nline two");
    }

    [Fact]
    public async Task Should_Strip_Html_Markup()
    {
        const string html = "<html><body><p>Hello&nbsp;world</p><script>var x=1;</script></body></html>";

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

        var text = await _factory.ExtractAsync(stream, ".html");

        text.ShouldContain("Hello");
        text.ShouldContain("world");
        text.ShouldNotContain("<p>");
        text.ShouldNotContain("var x");
    }

    [Fact]
    public async Task Should_Extract_Text_From_A_Docx_Package()
    {
        await using var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
        {
            var entry = archive.CreateEntry("word/document.xml");

            await using var writer = new StreamWriter(entry.Open(), Encoding.UTF8);
            await writer.WriteAsync(
                "<w:document><w:body><w:p><w:r><w:t>Invoice</w:t></w:r>" +
                "<w:r><w:t xml:space=\"preserve\"> total 1500</w:t></w:r></w:p></w:body></w:document>");
        }

        stream.Position = 0;

        var text = await _factory.ExtractAsync(stream, ".docx");

        text.ShouldContain("Invoice");
        text.ShouldContain("total 1500");
    }

    [Fact]
    public async Task Should_Return_Empty_Text_For_Unsupported_Extensions()
    {
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("binary-ish"));

        (await _factory.ExtractAsync(stream, ".dicom")).ShouldBeEmpty();
    }

    [Fact]
    public void Should_Route_By_Extension()
    {
        _factory.Resolve(".txt").ShouldBeOfType<PlainTextExtractor>();
        _factory.Resolve(".docx").ShouldBeOfType<OpenXmlTextExtractor>();
        _factory.Resolve(".pdf").ShouldBeOfType<PdfTextExtractor>();
        _factory.Resolve(".unknown").ShouldBeNull();
    }
}
