using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace RhyoliteOrbit.Controllers
{
    public abstract class RhyoliteOrbitControllerBase : AbpController
    {
        protected RhyoliteOrbitControllerBase()
        {
            LocalizationSourceName = RhyoliteOrbitConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
