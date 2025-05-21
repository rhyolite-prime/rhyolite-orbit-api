using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using RhyoliteOrbit.Authorization;

namespace RhyoliteOrbit;

[DependsOn(
    typeof(RhyoliteOrbitCoreModule),
    typeof(AbpAutoMapperModule))]
public class RhyoliteOrbitApplicationModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Authorization.Providers.Add<RhyoliteOrbitAuthorizationProvider>();
    }

    public override void Initialize()
    {
        var thisAssembly = typeof(RhyoliteOrbitApplicationModule).GetAssembly();

        IocManager.RegisterAssemblyByConvention(thisAssembly);

        Configuration.Modules.AbpAutoMapper().Configurators.Add(
            // Scan the assembly for classes which inherit from AutoMapper.Profile
            cfg => cfg.AddMaps(thisAssembly)
        );
    }
}
