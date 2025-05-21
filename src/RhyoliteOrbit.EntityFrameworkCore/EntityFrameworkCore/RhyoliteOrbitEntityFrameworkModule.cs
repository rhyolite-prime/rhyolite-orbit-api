using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using RhyoliteOrbit.EntityFrameworkCore.Seed;

namespace RhyoliteOrbit.EntityFrameworkCore;

[DependsOn(
    typeof(RhyoliteOrbitCoreModule),
    typeof(AbpZeroCoreEntityFrameworkCoreModule))]
public class RhyoliteOrbitEntityFrameworkModule : AbpModule
{
    /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
    public bool SkipDbContextRegistration { get; set; }

    public bool SkipDbSeed { get; set; }

    public override void PreInitialize()
    {
        if (!SkipDbContextRegistration)
        {
            Configuration.Modules.AbpEfCore().AddDbContext<RhyoliteOrbitDbContext>(options =>
            {
                if (options.ExistingConnection != null)
                {
                    RhyoliteOrbitDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                }
                else
                {
                    RhyoliteOrbitDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                }
            });
        }
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(RhyoliteOrbitEntityFrameworkModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        if (!SkipDbSeed)
        {
            SeedHelper.SeedHostDb(IocManager);
        }
    }
}
