using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using RhyoliteOrbit.Configuration;
using RhyoliteOrbit.EntityFrameworkCore;
using RhyoliteOrbit.Migrator.DependencyInjection;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;

namespace RhyoliteOrbit.Migrator;

[DependsOn(typeof(RhyoliteOrbitEntityFrameworkModule))]
public class RhyoliteOrbitMigratorModule : AbpModule
{
    private readonly IConfigurationRoot _appConfiguration;

    public RhyoliteOrbitMigratorModule(RhyoliteOrbitEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

        _appConfiguration = AppConfigurations.Get(
            typeof(RhyoliteOrbitMigratorModule).GetAssembly().GetDirectoryPathOrNull()
        );
    }

    public override void PreInitialize()
    {
        Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
            RhyoliteOrbitConsts.ConnectionStringName
        );

        Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        Configuration.ReplaceService(
            typeof(IEventBus),
            () => IocManager.IocContainer.Register(
                Component.For<IEventBus>().Instance(NullEventBus.Instance)
            )
        );
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(RhyoliteOrbitMigratorModule).GetAssembly());
        ServiceCollectionRegistrar.Register(IocManager);
    }
}
