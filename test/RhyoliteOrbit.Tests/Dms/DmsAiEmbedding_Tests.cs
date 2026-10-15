using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RhyoliteOrbit.DomainServices.Dms.Ai;
using Shouldly;
using Xunit;

namespace RhyoliteOrbit.Tests.Dms;

/// <summary>Unit tests for the heuristic AI classification service.</summary>
public class DmsAiClassification_Tests
{
    private readonly IAiDocumentService _service = new HeuristicAiDocumentService();

    [Fact]
    public async Task Should_Classify_An_Invoice()
    {
        var text = "INVOICE\nBill to: Acme Ltd\nSubtotal 100.00\nVAT 15.00\nTotal due 115.00\nPayment terms: 30 days";

        var result = await _service.ClassifyAsync("acme-invoice.pdf", ".pdf", text);

        result.DocumentType.ShouldBe("Invoice");
        result.Confidence.ShouldBeGreaterThan(0);
        result.Language.ShouldBe("en");
        result.Summary.ShouldBe(text);
    }

    [Fact]
    public async Task Should_Classify_A_Payslip()
    {
        var text = "Payslip for October\nGross pay 5000\nPAYE 700\nSSNIT 550\nAllowance 200\nDeduction 100\nNet pay 3850";

        var result = await _service.ClassifyAsync("payslip.pdf", ".pdf", text);

        result.DocumentType.ShouldBe("Payslip");
    }

    [Fact]
    public async Task Should_Fall_Back_To_Document_Type_When_Nothing_Matches()
    {
        var result = await _service.ClassifyAsync("scan.pdf", ".pdf", "some scanned content without keywords");

        result.DocumentType.ShouldBe("Document");
        result.Confidence.ShouldBe(0.2);
    }

    [Fact]
    public async Task Should_Truncate_Long_Summaries()
    {
        var text = new string('x', 1000);

        var result = await _service.ClassifyAsync("big.txt", ".txt", text);

        result.Summary.Length.ShouldBe(403); // 400 chars + "..."
        result.Summary.EndsWith("...").ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Detect_Twi_Content()
    {
        var result = await _service.ClassifyAsync("note.txt", ".txt", "yɛ anka yɛbɛka no ɔ");

        result.Language.ShouldBe("tw");
    }

    [Fact]
    public async Task Should_Suggest_Tags_Without_Stop_Words()
    {
        var tags = await _service.SuggestTagsAsync("invoice invoice invoice payment payment receipt customer customer customer", 3);

        tags.Count.ShouldBe(3);
        tags.ShouldContain("invoice");
        tags.ShouldNotContain("the");
    }

    [Fact]
    public async Task Should_Not_Suggest_Tags_For_Empty_Text()
    {
        var tags = await _service.SuggestTagsAsync("   ", 5);

        tags.ShouldBeEmpty();
    }
}

/// <summary>Unit tests for the deterministic hashing embedding service.</summary>
public class DmsEmbedding_Tests
{
    private readonly IEmbeddingService _service = new HashingEmbeddingService();

    [Fact]
    public void Should_Expose_A_Stable_Model_Name()
    {
        _service.ModelName.ShouldBe("hashing-bow-v1");
        _service.Dimensions.ShouldBe(256);
    }

    [Fact]
    public async Task Should_Produce_Unit_Length_Vectors()
    {
        var vector = await _service.EmbedQueryAsync("invoice payment customer");

        vector.Length.ShouldBe(256);

        var norm = Math.Sqrt(vector.Sum(v => v * v));
        norm.ShouldBeGreaterThan(0.99);
        norm.ShouldBeLessThan(1.01);
    }

    [Fact]
    public async Task Should_Be_Deterministic()
    {
        var first = await _service.EmbedQueryAsync("document management system");
        var second = await _service.EmbedQueryAsync("document management system");

        first.ShouldBe(second);
    }

    [Fact]
    public async Task Should_Give_Similar_Text_A_Higher_Score_Than_Unrelated_Text()
    {
        var reference = await _service.EmbedQueryAsync("invoice amount due payment terms");
        var similar = await _service.EmbedQueryAsync("invoice payment terms amount due");
        var unrelated = await _service.EmbedQueryAsync("aerospace engineering turbine blades");

        Cosine(reference, similar).ShouldBeGreaterThan(Cosine(reference, unrelated));
    }

    [Fact]
    public async Task Should_Chunk_Long_Text()
    {
        var text = string.Join(" ", Enumerable.Range(0, 500).Select(i => $"word{i}"));

        var chunks = await _service.EmbedAsync(text, 100);

        chunks.Count.ShouldBeGreaterThan(1);
        chunks.Select(c => c.ChunkIndex).ShouldBe(Enumerable.Range(0, chunks.Count));
        chunks.All(c => c.Vector.Length == 256).ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Return_No_Chunks_For_Empty_Text()
    {
        var chunks = await _service.EmbedAsync("   ");

        chunks.ShouldBeEmpty();
    }

    private static double Cosine(float[] left, float[] right)
    {
        double dot = 0, leftNorm = 0, rightNorm = 0;

        for (var i = 0; i < left.Length; i++)
        {
            dot += left[i] * right[i];
            leftNorm += left[i] * left[i];
            rightNorm += right[i] * right[i];
        }

        return dot / (Math.Sqrt(leftNorm) * Math.Sqrt(rightNorm));
    }
}
