using Abp.Modules;
using Abp.Reflection.Extensions;
using RhyoliteOrbit.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace RhyoliteOrbit.Web.Startup;

[DependsOn(typeof(RhyoliteOrbitWebCoreModule))]
public class RhyoliteOrbitWebMvcModule : AbpModule
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfigurationRoot _appConfiguration;

    public RhyoliteOrbitWebMvcModule(IWebHostEnvironment env)
    {
        _env = env;
        _appConfiguration = env.GetAppConfiguration();
    }

    public override void PreInitialize()
    {
        Configuration.Navigation.Providers.Add<RhyoliteOrbitNavigationProvider>();
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(RhyoliteOrbitWebMvcModule).GetAssembly());
    }
}
