using System.Threading.Tasks;

namespace RhyoliteOrbit.DomainServices.Dms.Subscription;

/// <summary>
/// Enforces the Document Management subscription (module id 13) and the tenant
/// storage quota on every DMS request.
/// </summary>
public interface IDmsSubscriptionGuard
{
    /// <summary>Throws when the tenant has no active DMS subscription.</summary>
    Task EnsureActiveAsync();

    /// <summary>Throws when storing <paramref name="additionalBytes"/> would exceed the quota.</summary>
    Task EnsureQuotaAsync(long additionalBytes);

    Task<bool> IsActiveAsync();
}
