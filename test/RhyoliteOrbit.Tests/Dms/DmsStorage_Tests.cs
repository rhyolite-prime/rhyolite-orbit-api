using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using RhyoliteOrbit.CloudinaryApi;
using RhyoliteOrbit.DomainServices.Dms.Storage;
using Shouldly;
using Xunit;

namespace RhyoliteOrbit.Tests.Dms;

/// <summary>Unit tests for the local file system storage provider.</summary>
public class DmsLocalStorage_Tests: IDisposable
{
    private readonly string _root;
    private readonly LocalStorageProvider _provider;

    public DmsLocalStorage_Tests()
    {
        _root = Path.Combine(Path.GetTempPath(), "dms-tests-" + Guid.NewGuid().ToString("n"));

        var configuration = Substitute.For<IConfiguration>();
        configuration["DmsStorage:LocalRootPath"].Returns(_root);

        _provider = new LocalStorageProvider(configuration);
    }

    [Fact]
    public async Task Should_Round_Trip_Content()
    {
        var key = "1/col/folder/doc/version/report.pdf";
        var content = Encoding.UTF8.GetBytes("hello document");

        await using (var stream = new MemoryStream(content))
        {
            var stored = await _provider.PutAsync(stream, key, "application/pdf");
            stored.Key.ShouldBe(key);
            stored.Size.ShouldBe(content.Length);
        }

        (await _provider.ExistsAsync(key)).ShouldBeTrue();

        await using var read = await _provider.GetAsync(key);
        using var reader = new StreamReader(read);
        (await reader.ReadToEndAsync()).ShouldBe("hello document");
    }

    [Fact]
    public async Task Should_Overwrite_Existing_Content()
    {
        var key = "1/doc/file.txt";

        await using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("first")))
        {
            await _provider.PutAsync(stream, key, "text/plain");
        }

        await using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("second version")))
        {
            await _provider.PutAsync(stream, key, "text/plain");
        }

        await using var read = await _provider.GetAsync(key);
        using var reader = new StreamReader(read);
        (await reader.ReadToEndAsync()).ShouldBe("second version");
    }

    [Fact]
    public async Task Should_Delete_Content()
    {
        var key = "1/doc/to-delete.txt";

        await using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("bye")))
        {
            await _provider.PutAsync(stream, key, "text/plain");
        }

        await _provider.DeleteAsync(key);

        (await _provider.ExistsAsync(key)).ShouldBeFalse();
    }

    [Fact]
    public async Task Should_Reject_Path_Traversal()
    {
        await Should.ThrowAsync<Exception>(async () =>
        {
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("bad"));
            await _provider.PutAsync(stream, "../../etc/passwd", "text/plain");
        });
    }

    [Fact]
    public void Should_Expose_The_Provider_Name()
    {
        _provider.Name.ShouldBe("Local");
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, true);
        }
    }
}

/// <summary>Unit tests for the storage provider factory routing.</summary>
public class DmsStorageProviderFactory_Tests
{
    private static StorageProviderFactory CreateFactory(IIocResolver iocResolver)
    {
        var providers = new IStorageProvider[]
        {
            new LocalStorageProvider(new EmptyConfiguration()),
            new S3StorageProvider(new EmptyConfiguration()),
            new CloudinaryStorageProvider(Substitute.For<RhyoliteOrbit.CloudinaryApi.IFileUploadApi>(), new EmptyConfiguration())
        };

        var settingsManager = Substitute.For<RhyoliteOrbit.DomainServices.Dms.Settings.IDmsSettingsManager>();
        settingsManager.GetAsync().Returns(Task.FromResult<RhyoliteOrbit.Models.Dms.DmsSettings>(null));

        return new StorageProviderFactory(iocResolver, settingsManager, providers);
    }

    [Fact]
    public void Should_Resolve_Provider_By_Name_Case_Insensitive()
    {
        var factory = CreateFactory(Substitute.For<IIocResolver>());

        factory.GetProvider("local").Name.ShouldBe("Local");
        factory.GetProvider("LOCAL").Name.ShouldBe("Local");
        factory.GetProvider("S3").Name.ShouldBe("S3");
        factory.GetProvider("Cloudinary").Name.ShouldBe("Cloudinary");
    }

    [Fact]
    public void Should_Fall_Back_To_Cloudinary_For_Unknown_Providers()
    {
        var iocResolver = Substitute.For<IIocResolver>();
        var cloudinary = new CloudinaryStorageProvider(
            Substitute.For<RhyoliteOrbit.CloudinaryApi.IFileUploadApi>(), new EmptyConfiguration());

        iocResolver.Resolve<CloudinaryStorageProvider>().Returns(cloudinary);

        var factory = CreateFactory(iocResolver);

        factory.GetProvider("SomethingElse").Name.ShouldBe("Cloudinary");
        factory.GetProvider(null).Name.ShouldBe("Cloudinary");
    }

    private class EmptyConfiguration: IConfiguration
    {
        public string this[string key]
        {
            get => null;
            set { }
        }

        public System.Collections.Generic.IEnumerable<IConfigurationSection> GetChildren()
        {
            return Enumerable.Empty<IConfigurationSection>();
        }

        public Microsoft.Extensions.Primitives.IChangeToken GetReloadToken()
        {
            return new StubChangeToken();
        }

        public IConfigurationSection GetSection(string key)
        {
            return null;
        }
    }

    private class StubChangeToken: Microsoft.Extensions.Primitives.IChangeToken
    {
        public bool HasChanged => false;

        public bool ActiveChangeCallbacks => false;

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            return new NoopDisposable();
        }

        private class NoopDisposable: IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
