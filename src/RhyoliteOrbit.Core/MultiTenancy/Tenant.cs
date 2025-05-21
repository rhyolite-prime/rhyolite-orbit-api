using Abp.MultiTenancy;
using RhyoliteOrbit.Authorization.Users;

namespace RhyoliteOrbit.MultiTenancy;

public class Tenant : AbpTenant<User>
{
    public Tenant()
    {
    }

    public Tenant(string tenancyName, string name)
        : base(tenancyName, name)
    {
    }
}
