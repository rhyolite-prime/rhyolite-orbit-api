using System.Collections.Generic;

namespace RhyoliteOrbit.Authentication.External
{
    public interface IExternalAuthConfiguration
    {
        List<ExternalLoginProviderInfo> Providers { get; }
    }
}
