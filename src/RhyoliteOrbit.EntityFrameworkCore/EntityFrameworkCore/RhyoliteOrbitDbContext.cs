using System;
using Abp.Zero.EntityFrameworkCore;
using RhyoliteOrbit.Authorization.Roles;
using RhyoliteOrbit.Authorization.Users;
using RhyoliteOrbit.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace RhyoliteOrbit.EntityFrameworkCore;

public class RhyoliteOrbitDbContext : AbpZeroDbContext<Tenant, Role, User, RhyoliteOrbitDbContext>
{
    /* Define a DbSet for each entity of the application */

    public RhyoliteOrbitDbContext(DbContextOptions<RhyoliteOrbitDbContext> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        //MigrateDatabase();
    }
    
    public void MigrateDatabase()
    {
        Database.Migrate();
    }
}
