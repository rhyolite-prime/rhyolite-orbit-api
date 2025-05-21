using Abp.AspNetCore.Mvc.ViewComponents;

namespace RhyoliteOrbit.Web.Views;

public abstract class RhyoliteOrbitViewComponent : AbpViewComponent
{
    protected RhyoliteOrbitViewComponent()
    {
        LocalizationSourceName = RhyoliteOrbitConsts.LocalizationSourceName;
    }
}
