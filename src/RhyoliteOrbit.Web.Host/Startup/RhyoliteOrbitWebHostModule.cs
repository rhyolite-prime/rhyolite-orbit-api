using Abp.Modules;
using Abp.Reflection.Extensions;
using RhyoliteOrbit.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace RhyoliteOrbit.Web.Host.Startup
{
    [DependsOn(
       typeof(RhyoliteOrbitWebCoreModule))]
    public class RhyoliteOrbitWebHostModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public RhyoliteOrbitWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(RhyoliteOrbitWebHostModule).GetAssembly());
        }
    }
}
