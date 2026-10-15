using System.Threading.Tasks;

namespace RhyoliteOrbit.DomainServices.Dms.Storage;

/// <summary>Resolves the configured <see cref="IStorageProvider"/> for a tenant.</summary>
public interface IStorageProviderFactory
{
    Task<IStorageProvider> GetProviderAsync(int tenantId);

    IStorageProvider GetProvider(string providerName);
}
