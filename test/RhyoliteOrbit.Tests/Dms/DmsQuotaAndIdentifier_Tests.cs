using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using RhyoliteOrbit.DomainServices.Dms.Identifiers;
using RhyoliteOrbit.DomainServices.Dms.Quota;
using RhyoliteOrbit.DomainServices.Dms.Settings;
using RhyoliteOrbit.DomainServices.Dms.Shares;
using RhyoliteOrbit.Models.Dms;
using Shouldly;
using Xunit;

namespace RhyoliteOrbit.Tests.Dms;

/// <summary>Unit tests for the tenant storage quota logic.</summary>
public class DmsQuota_Tests
{
    private static QuotaManager CreateManager(int quotaGb, long bytesUsed = 0)
    {
        var settingsManager = Substitute.For<IDmsSettingsManager>();
        settingsManager.GetAsync().Returns(Task.FromResult(new DmsSettings { QuotaGb = quotaGb }));

        var usageRepository = Substitute.For<IRepository<DmsQuotaUsage, Guid>>();
        usageRepository.GetAllAsync().Returns(Task.FromResult(
            new List<DmsQuotaUsage> { new DmsQuotaUsage { TenantId = 1, BytesUsed = bytesUsed } }.AsQueryable()));

        var session = Substitute.For<IAbpSession>();
        session.TenantId.Returns(1);

        return new QuotaManager(
            usageRepository,
            Substitute.For<IRepository<DmsDocument, Guid>>(),
            Substitute.For<IRepository<DmsDocumentVersion, Guid>>(),
            settingsManager,
            session);
    }

    [Fact]
    public async Task Should_Compute_Quota_Bytes_From_Gigabytes()
    {
        var manager = CreateManager(quotaGb: 10);

        (await manager.GetQuotaBytesAsync()).ShouldBe(10L * 1024 * 1024 * 1024);
    }

    [Fact]
    public async Task Should_Treat_Zero_Quota_As_Unlimited()
    {
        var manager = CreateManager(quotaGb: 0);

        (await manager.GetQuotaBytesAsync()).ShouldBe(0);
        (await manager.HasSpaceAsync(long.MaxValue)).ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Allow_Uploads_Below_The_Quota()
    {
        var manager = CreateManager(quotaGb: 1, bytesUsed: 1024);

        (await manager.HasSpaceAsync(1024)).ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Reject_Uploads_Above_The_Quota()
    {
        var manager = CreateManager(quotaGb: 1, bytesUsed: 1024L * 1024 * 1024);

        (await manager.HasSpaceAsync(1)).ShouldBeFalse();
    }
}

/// <summary>Unit tests for the custom document identifier sequences.</summary>
public class DmsIdentifier_Tests
{
    private static DocumentIdentifierManager CreateManager(DmsCustomIdentifier identifier)
    {
        var repository = Substitute.For<IRepository<DmsCustomIdentifier, Guid>>();
        repository.FirstOrDefaultAsync(identifier.Id).Returns(Task.FromResult(identifier));
        repository.GetAllAsync().Returns(Task.FromResult(new List<DmsCustomIdentifier> { identifier }.AsQueryable()));

        return new DocumentIdentifierManager(repository, Substitute.For<IAbpSession>());
    }

    [Fact]
    public async Task Should_Generate_A_Padded_Sequence()
    {
        var identifier = new DmsCustomIdentifier
        {
            Id = Guid.NewGuid(),
            Prefix = "DOC",
            Suffix = "GH",
            PaddingLength = 5,
            CurrentSequence = 41,
            IsActive = true
        };

        var manager = CreateManager(identifier);

        (await manager.GenerateNextAsync(identifier.Id)).ShouldBe("DOC-00042-GH");
        identifier.CurrentSequence.ShouldBe(42);
    }

    [Fact]
    public async Task Should_Include_Year_And_Month_When_Configured()
    {
        var identifier = new DmsCustomIdentifier
        {
            Id = Guid.NewGuid(),
            Prefix = "INV",
            IncludeYear = true,
            IncludeMonth = true,
            PaddingLength = 4,
            IsActive = true
        };

        var manager = CreateManager(identifier);
        var expected = $"INV-{DateTime.UtcNow.Year}-{DateTime.UtcNow:MM}-0001";

        (await manager.GenerateNextAsync(identifier.Id)).ShouldBe(expected);
    }

    [Fact]
    public async Task Should_Throw_When_The_Identifier_Is_Inactive()
    {
        var identifier = new DmsCustomIdentifier
        {
            Id = Guid.NewGuid(),
            PaddingLength = 4,
            IsActive = false
        };

        var manager = CreateManager(identifier);

        await Should.ThrowAsync<Abp.UI.UserFriendlyException>(
            async () => await manager.GenerateNextAsync(identifier.Id));
    }

    [Fact]
    public async Task Should_Throw_When_The_Sequence_Is_Exhausted()
    {
        var identifier = new DmsCustomIdentifier
        {
            Id = Guid.NewGuid(),
            PaddingLength = 2,
            CurrentSequence = 99,
            IsActive = true
        };

        var manager = CreateManager(identifier);

        await Should.ThrowAsync<Abp.UI.UserFriendlyException>(
            async () => await manager.GenerateNextAsync(identifier.Id));
    }

    [Fact]
    public async Task Should_Preview_Without_Consuming_The_Sequence()
    {
        var identifier = new DmsCustomIdentifier
        {
            Id = Guid.NewGuid(),
            Prefix = "DOC",
            PaddingLength = 4,
            CurrentSequence = 7,
            IsActive = true
        };

        var manager = CreateManager(identifier);

        (await manager.PreviewNextAsync(identifier.Id)).ShouldBe("DOC-0008");
        identifier.CurrentSequence.ShouldBe(7);
    }
}

/// <summary>Unit tests for the public share token helpers.</summary>
public class DmsShareToken_Tests
{
    [Fact]
    public void Should_Generate_Unique_Tokens()
    {
        var tokens = Enumerable.Range(0, 50).Select(_ => ShareManager.GenerateToken()).ToList();

        tokens.Distinct().Count().ShouldBe(50);
        tokens.All(t => t.Length >= 20).ShouldBeTrue();
    }

    [Fact]
    public void Should_Hash_Tokens_Deterministically()
    {
        var hash = ShareManager.HashToken("token-value");

        hash.ShouldBe(ShareManager.HashToken("token-value"));
        hash.ShouldNotBe("token-value");
        hash.Length.ShouldBe(64);
    }

    [Fact]
    public void Should_Not_Hash_Different_Tokens_The_Same_Way()
    {
        ShareManager.HashToken("a").ShouldNotBe(ShareManager.HashToken("b"));
    }

    [Fact]
    public void Should_Hash_Passwords_With_A_Salt()
    {
        var first = ShareManager.HashPassword("secret");
        var second = ShareManager.HashPassword("secret");

        // salted: the same password produces different hashes
        first.ShouldNotBe(second);
        first.Split('.').Length.ShouldBe(2);
    }

    [Fact]
    public void Should_Verify_A_Hashed_Password()
    {
        var stored = ShareManager.HashPassword("secret");

        ShareManager.VerifyPassword("secret", stored).ShouldBeTrue();
        ShareManager.VerifyPassword("wrong", stored).ShouldBeFalse();
    }

    [Fact]
    public void Should_Treat_An_Empty_Stored_Hash_As_No_Password()
    {
        ShareManager.VerifyPassword(null, null).ShouldBeTrue();
        ShareManager.VerifyPassword("secret", null).ShouldBeFalse();
        ShareManager.VerifyPassword("secret", "not-a-hash").ShouldBeFalse();
    }
}
