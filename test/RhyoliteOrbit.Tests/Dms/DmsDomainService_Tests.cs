using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.Dms.Documents;
using RhyoliteOrbit.DomainServices.Dms.Folders;
using RhyoliteOrbit.DomainServices.Dms.Identifiers;
using RhyoliteOrbit.DomainServices.Dms.Quota;
using RhyoliteOrbit.DomainServices.Dms.Settings;
using RhyoliteOrbit.DomainServices.Dms.Versions;
using RhyoliteOrbit.Models.Dms;
using Shouldly;
using Xunit;

namespace RhyoliteOrbit.Tests.Dms;

/// <summary>
/// Integration tests for the core DMS domain services (folders, documents,
/// versions, identifiers and quota) running against the in-memory database used
/// by the test host.
/// </summary>
public class DmsDomainService_Tests: RhyoliteOrbitTestBase
{
    private readonly IFolderManager _folderManager;
    private readonly IDocumentManager _documentManager;
    private readonly IVersionManager _versionManager;
    private readonly IDocumentIdentifierManager _identifierManager;
    private readonly IQuotaManager _quotaManager;
    private readonly IDmsSettingsManager _settingsManager;
    private readonly IRepository<DmsSettings, Guid> _settingsRepository;

    public DmsDomainService_Tests()
    {
        _folderManager = Resolve<IFolderManager>();
        _documentManager = Resolve<IDocumentManager>();
        _versionManager = Resolve<IVersionManager>();
        _identifierManager = Resolve<IDocumentIdentifierManager>();
        _quotaManager = Resolve<IQuotaManager>();
        _settingsManager = Resolve<IDmsSettingsManager>();
        _settingsRepository = Resolve<IRepository<DmsSettings, Guid>>();
    }

    private async Task UseLocalStorageAsync()
    {
        var settings = await _settingsManager.EnsureCreatedAsync();
        settings.StorageProvider = "Local";
        settings.LocalRootPath = Path.Combine(Path.GetTempPath(), "dms-it-" + Guid.NewGuid().ToString("n"));
        settings.AntivirusEnabled = false;
        settings.OcrEnabled = false;
        settings.AiEnabled = false;

        await _settingsManager.UpdateAsync(settings);
    }

    private async Task<DmsFolder> CreateFolderAsync(string name = "Contracts")
    {
        var collection = await _folderManager.GetOrCreateDefaultCollectionAsync();

        return await _folderManager.CreateAsync(new DmsFolder
        {
            Name = name,
            CollectionId = collection.Id,
            OwnerId = AbpSession.GetUserId(),
            InheritPermissions = true,
            Depth = 0
        });
    }

    private async Task<DmsDocument> UploadAsync(Guid folderId, string fileName, string content)
    {
        return await _documentManager.UploadAsync(new DocumentUploadInput
        {
            FolderId = folderId,
            Name = Path.GetFileNameWithoutExtension(fileName),
            FileName = fileName,
            ContentType = "text/plain",
            Content = new MemoryStream(Encoding.UTF8.GetBytes(content)),
            CreatedById = AbpSession.GetUserId()
        });
    }

    [Fact]
    public async Task Should_Create_A_Root_Folder_And_Rebuild_Its_Path()
    {
        await UseLocalStorageAsync();

        var folder = await CreateFolderAsync("Policies");

        folder.Path.ShouldBe("Policies");
        folder.Depth.ShouldBe(0);
    }

    [Fact]
    public async Task Should_Create_A_Sub_Folder_With_The_Parent_Path()
    {
        await UseLocalStorageAsync();

        var parent = await CreateFolderAsync("Parent");
        var child = await _folderManager.CreateAsync(new DmsFolder
        {
            Name = "Child",
            ParentId = parent.Id,
            CollectionId = parent.CollectionId,
            Depth = 0
        });

        await _folderManager.RebuildPathsAsync();

        child.Path.ShouldBe("Parent/Child");
        child.Depth.ShouldBe(1);
    }

    [Fact]
    public async Task Should_Upload_A_Document_With_Its_First_Version()
    {
        await UseLocalStorageAsync();

        var folder = await CreateFolderAsync();
        var document = await UploadAsync(folder.Id, "terms.txt", "payment terms: 30 days");

        document.Id.ShouldNotBe(Guid.Empty);
        document.CurrentVersionId.ShouldNotBeNull();
        document.Checksum.ShouldNotBeNullOrWhiteSpace();
        document.Status.ShouldBe((int)RhyoliteOrbitConsts.DocumentStatus.Active);

        var current = await _versionManager.GetCurrentAsync(document.Id);
        current.ShouldNotBeNull();
        current.VersionNumber.ShouldBe(1);
        current.IsCurrent.ShouldBeTrue();
        current.ChangeNote.ShouldBe("Initial upload");
    }

    [Fact]
    public async Task Should_Compute_A_Stable_Checksum_For_Identical_Content()
    {
        await UseLocalStorageAsync();

        var folder = await CreateFolderAsync();

        var first = await UploadAsync(folder.Id, "a.txt", "same content");
        var second = await UploadAsync(folder.Id, "b.txt", "same content");

        first.Checksum.ShouldBe(second.Checksum);
        (await _documentManager.IsDuplicateAsync(first.Checksum)).ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Add_Minor_And_Major_Versions()
    {
        await UseLocalStorageAsync();

        var folder = await CreateFolderAsync();
        var document = await UploadAsync(folder.Id, "spec.txt", "version one");

        var minor = await _versionManager.AddAsync(document.Id, new VersionUploadInput
        {
            Content = new MemoryStream(Encoding.UTF8.GetBytes("version one point one")),
            FileName = "spec.txt",
            ContentType = "text/plain",
            ChangeNote = "typo fix",
            Scheme = (int)RhyoliteOrbitConsts.DocumentVersionScheme.Minor,
            CreatedById = AbpSession.GetUserId()
        });

        minor.VersionNumber.ShouldBe(1);
        minor.MinorNumber.ShouldBe(1);

        var major = await _versionManager.AddAsync(document.Id, new VersionUploadInput
        {
            Content = new MemoryStream(Encoding.UTF8.GetBytes("version two")),
            FileName = "spec.txt",
            ContentType = "text/plain",
            ChangeNote = "release",
            Scheme = (int)RhyoliteOrbitConsts.DocumentVersionScheme.Major,
            CreatedById = AbpSession.GetUserId()
        });

        major.VersionNumber.ShouldBe(2);
        major.MinorNumber.ShouldBe(0);

        var versions = await _versionManager.ListAsync(document.Id);
        versions.Count.ShouldBe(3);
    }

    [Fact]
    public async Task Should_Promote_An_Older_Version()
    {
        await UseLocalStorageAsync();

        var folder = await CreateFolderAsync();
        var document = await UploadAsync(folder.Id, "draft.txt", "v1");

        var second = await _versionManager.AddAsync(document.Id, new VersionUploadInput
        {
            Content = new MemoryStream(Encoding.UTF8.GetBytes("v2")),
            FileName = "draft.txt",
            ContentType = "text/plain",
            Scheme = (int)RhyoliteOrbitConsts.DocumentVersionScheme.Major,
            CreatedById = AbpSession.GetUserId()
        });

        var first = (await _versionManager.ListAsync(document.Id))
            .First(v => v.VersionNumber == 1);

        await _versionManager.PromoteAsync(first.Id, AbpSession.GetUserId());

        var current = await _versionManager.GetCurrentAsync(document.Id);
        current.Id.ShouldBe(first.Id);

        var refreshedSecond = await _versionManager.GetAsync(second.Id);
        refreshedSecond.IsCurrent.ShouldBeFalse();
    }

    [Fact]
    public async Task Should_Check_Out_And_Back_In()
    {
        await UseLocalStorageAsync();

        var folder = await CreateFolderAsync();
        var document = await UploadAsync(folder.Id, "locked.txt", "content");

        await _documentManager.CheckOutAsync(document.Id, AbpSession.GetUserId(), TimeSpan.FromHours(1));

        var checkedOut = await _documentManager.GetAsync(document.Id);
        checkedOut.IsCheckedOut.ShouldBeTrue();
        checkedOut.CheckedOutBy.ShouldBe(AbpSession.GetUserId());

        await _documentManager.CheckInAsync(document.Id, AbpSession.GetUserId());

        var checkedIn = await _documentManager.GetAsync(document.Id);
        checkedIn.IsCheckedOut.ShouldBeFalse();
        checkedIn.CheckedOutBy.ShouldBeNull();
    }

    [Fact]
    public async Task Should_Soft_Delete_Restore_And_Purge_A_Document()
    {
        await UseLocalStorageAsync();

        var folder = await CreateFolderAsync();
        var document = await UploadAsync(folder.Id, "temp.txt", "temporary");

        await _documentManager.DeleteAsync(document.Id, AbpSession.GetUserId());

        var deleted = await _documentManager.GetAsync(document.Id);
        deleted.IsDeleted.ShouldBeTrue();
        deleted.Status.ShouldBe((int)RhyoliteOrbitConsts.DocumentStatus.Deleted);

        await _documentManager.RestoreAsync(document.Id, AbpSession.GetUserId());

        var restored = await _documentManager.GetAsync(document.Id);
        restored.IsDeleted.ShouldBeFalse();

        await _documentManager.PurgeAsync(document.Id, AbpSession.GetUserId());

        var remaining = await UsingDbContextAsync(context => context.Set<DmsDocument>()
            .IgnoreQueryFilters()
            .CountAsync(d => d.Id == document.Id));

        remaining.ShouldBe(0);
    }

    [Fact]
    public async Task Should_Move_A_Document_To_Another_Folder()
    {
        await UseLocalStorageAsync();

        var source = await CreateFolderAsync("Source");
        var target = await CreateFolderAsync("Target");
        var document = await UploadAsync(source.Id, "move.txt", "movable");

        await _documentManager.MoveAsync(document.Id, target.Id, AbpSession.GetUserId());

        var moved = await _documentManager.GetAsync(document.Id);
        moved.FolderId.ShouldBe(target.Id);
        moved.FolderName.ShouldBe("Target");
    }

    [Fact]
    public async Task Should_Prevent_Moving_A_Folder_Into_Its_Own_Subtree()
    {
        await UseLocalStorageAsync();

        var parent = await CreateFolderAsync("Root");
        var child = await _folderManager.CreateAsync(new DmsFolder
        {
            Name = "Child",
            ParentId = parent.Id,
            CollectionId = parent.CollectionId,
            Depth = 0
        });

        await Should.ThrowAsync<UserFriendlyException>(async () =>
            await _folderManager.MoveAsync(parent.Id, child.Id, AbpSession.GetUserId()));
    }

    [Fact]
    public async Task Should_Assign_And_Replace_Tags()
    {
        await UseLocalStorageAsync();

        var folder = await CreateFolderAsync();
        var document = await UploadAsync(folder.Id, "tagged.txt", "content");

        var tagRepository = Resolve<IRepository<DmsTag, Guid>>();
        var first = await tagRepository.InsertAsync(new DmsTag { Name = "Finance" });
        var second = await tagRepository.InsertAsync(new DmsTag { Name = "2026" });

        await _documentManager.AssignTagsAsync(document.Id, new System.Collections.Generic.List<Guid> { first.Id, second.Id }, AbpSession.GetUserId());

        var tags = await UsingDbContextAsync(context => context.Set<DmsDocumentTag>()
            .Where(t => t.DocumentId == document.Id)
            .ToListAsync());

        tags.Count.ShouldBe(2);

        await _documentManager.AssignTagsAsync(document.Id, new System.Collections.Generic.List<Guid> { first.Id }, AbpSession.GetUserId());

        tags = await UsingDbContextAsync(context => context.Set<DmsDocumentTag>()
            .Where(t => t.DocumentId == document.Id)
            .ToListAsync());

        tags.Count.ShouldBe(1);
        tags[0].TagId.ShouldBe(first.Id);
    }

    [Fact]
    public async Task Should_Generate_Sequential_Identifiers()
    {
        var identifier = await _identifierManager.CreateAsync(new DmsCustomIdentifier
        {
            Name = "Invoice number",
            Prefix = "INV",
            PaddingLength = 5,
            IsActive = true
        });

        (await _identifierManager.GenerateNextAsync(identifier.Id)).ShouldBe("INV-00001");
        (await _identifierManager.GenerateNextAsync(identifier.Id)).ShouldBe("INV-00002");
        (await _identifierManager.PreviewNextAsync(identifier.Id)).ShouldBe("INV-00003");
    }

    [Fact]
    public async Task Should_Enforce_The_Quota_When_Uploading()
    {
        await UseLocalStorageAsync();

        var settings = await _settingsManager.EnsureCreatedAsync();
        settings.QuotaGb = 0; // unlimited
        await _settingsManager.UpdateAsync(settings);

        (await _quotaManager.GetQuotaBytesAsync()).ShouldBe(0);
        (await _quotaManager.HasSpaceAsync(1024)).ShouldBeTrue();

        settings.QuotaGb = 1;
        await _settingsManager.UpdateAsync(settings);

        (await _quotaManager.GetQuotaBytesAsync()).ShouldBe(1024L * 1024 * 1024);
        (await _quotaManager.HasSpaceAsync(1024)).ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Recompute_The_Storage_Usage()
    {
        await UseLocalStorageAsync();

        var folder = await CreateFolderAsync();
        await UploadAsync(folder.Id, "one.txt", "one");
        await UploadAsync(folder.Id, "two.txt", "two");

        await _quotaManager.RecomputeAsync();

        var usage = await _quotaManager.GetUsageAsync();
        var json = System.Text.Json.JsonSerializer.Serialize(usage);

        json.ShouldContain("BytesUsed");
    }
}
