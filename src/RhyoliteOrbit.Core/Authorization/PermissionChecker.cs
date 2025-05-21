using Abp.Authorization;
using RhyoliteOrbit.Authorization.Roles;
using RhyoliteOrbit.Authorization.Users;

namespace RhyoliteOrbit.Authorization;

public class PermissionChecker : PermissionChecker<Role, User>
{
    public PermissionChecker(UserManager userManager)
        : base(userManager)
    {
    }
}
