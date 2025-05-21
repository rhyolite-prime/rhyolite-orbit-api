using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace RhyoliteOrbit.Web.Views;

public abstract class RhyoliteOrbitRazorPage<TModel> : AbpRazorPage<TModel>
{
    [RazorInject]
    public IAbpSession AbpSession { get; set; }

    protected RhyoliteOrbitRazorPage()
    {
        LocalizationSourceName = RhyoliteOrbitConsts.LocalizationSourceName;
    }
}
