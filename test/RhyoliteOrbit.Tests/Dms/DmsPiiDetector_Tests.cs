using System.Linq;
using System.Threading.Tasks;
using RhyoliteOrbit.DomainServices.Dms.Security;
using Shouldly;
using Xunit;

namespace RhyoliteOrbit.Tests.Dms;

/// <summary>Unit tests for the rule based PII detector.</summary>
public class DmsPiiDetector_Tests
{
    private readonly IPiiDetector _detector = new RegexPiiDetector();

    [Fact]
    public async Task Should_Detect_Email_Addresses()
    {
        var matches = await _detector.DetectAsync("Contact us at accounts@rhyoliteorbit.com for details.");

        matches.ShouldContain(m => m.Type == "Email" && m.Value == "accounts@rhyoliteorbit.com");
    }

    [Fact]
    public async Task Should_Detect_Ghana_Card_And_Tin()
    {
        var matches = await _detector.DetectAsync("Ghana Card GHA-123456789-0 and TIN P00123456789.");

        matches.ShouldContain(m => m.Type == "GhanaCard");
        matches.ShouldContain(m => m.Type == "Tin");
    }

    [Fact]
    public async Task Should_Detect_Ghana_Phone_Numbers()
    {
        var matches = await _detector.DetectAsync("Call 0244123456 or +233 20 123 4567 today.");

        matches.Count(m => m.Type == "Phone").ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task Should_Not_Report_Pii_In_Plain_Text()
    {
        var matches = await _detector.DetectAsync("The quarterly report was presented to the board.");

        matches.ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_Handle_Empty_Text()
    {
        (await _detector.DetectAsync(null)).ShouldBeEmpty();
        (await _detector.DetectAsync(string.Empty)).ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_Report_Position_Of_The_Match()
    {
        const string text = "prefix accounts@rhyoliteorbit.com suffix";

        var matches = await _detector.DetectAsync(text);

        var email = matches.First(m => m.Type == "Email");
        text.Substring(email.Start, email.Length).ShouldBe(email.Value);
    }

    [Fact]
    public void Should_Mask_Values()
    {
        var masked = _detector.Mask("accounts@rhyoliteorbit.com");

        masked.ShouldNotBe("accounts@rhyoliteorbit.com");
        masked.ShouldContain("*");
    }
}
