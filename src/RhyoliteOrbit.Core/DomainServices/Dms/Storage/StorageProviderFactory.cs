using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Dependency;
using RhyoliteOrbit.DomainServices.Dms.Settings;

namespace RhyoliteOrbit.DomainServices.Dms.Storage;

/// <summary>
/// Selects the storage provider for a tenant. Providers are resolved from the IoC
/// container by name; unknown names fall back to Cloudinary.
/// </summary>
public class StorageProviderFactory: IStorageProviderFactory, ITransientDependency
{
    private readonly IIocResolver _iocResolver;
    private readonly IDmsSettingsManager _settingsManager;
    private readonly Dictionary<string, IStorageProvider> _providersByName;

    public StorageProviderFactory(
        IIocResolver iocResolver,
        IDmsSettingsManager settingsManager,
        IEnumerable<IStorageProvider> providers)
    {
        _iocResolver = iocResolver;
        _settingsManager = settingsManager;
        _providersByName = new Dictionary<string, IStorageProvider>(StringComparer.OrdinalIgnoreCase);

        foreach (var provider in providers)
        {
            _providersByName[provider.Name] = provider;
        }
    }

    public IStorageProvider GetProvider(string providerName)
    {
        if (!string.IsNullOrWhiteSpace(providerName) && _providersByName.TryGetValue(providerName, out var provider))
        {
            return provider;
        }

        return _iocResolver.Resolve<CloudinaryStorageProvider>();
    }

    public async Task<IStorageProvider> GetProviderAsync(int tenantId)
    {
        // The tenant settings decide where the binaries live; an unknown or empty
        // name falls back to the configured default provider.
        DmsSettings settings = null;

        try
        {
            settings = await _settingsManager.GetAsync();
        }
        catch (Exception)
        {
            // settings must never block storage resolution
        }

        return GetProvider(settings?.StorageProvider);
    }
}
