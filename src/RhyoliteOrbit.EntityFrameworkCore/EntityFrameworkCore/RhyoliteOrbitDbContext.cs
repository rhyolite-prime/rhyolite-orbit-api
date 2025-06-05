using System;
using Abp.Zero.EntityFrameworkCore;
using RhyoliteOrbit.Authorization.Roles;
using RhyoliteOrbit.Authorization.Users;
using RhyoliteOrbit.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.EntityFrameworkCore;

public class RhyoliteOrbitDbContext : AbpZeroDbContext<Tenant, Role, User, RhyoliteOrbitDbContext>
{
    /* Define a DbSet for each entity of the application */

    public RhyoliteOrbitDbContext(DbContextOptions<RhyoliteOrbitDbContext> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        MigrateDatabase();
    }
    
    public void MigrateDatabase()
    {
        Database.Migrate();
    }
    
    //shared resources
    public DbSet<Bank> Banks { get; set; }
    public DbSet<BusinessProfile> BusinessProfiles { get; set; }
    public DbSet<CostCenter> CostCenters { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerGroup> CustomerGroups { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<SystemSetting> SystemSetting { get; set; }
    public DbSet<PersonnelStatus> PersonnelStatuses { get; set; }
    public DbSet<Relationship> Relationships { get; set; }
    public DbSet<Religion> Religions { get; set; }
    public DbSet<ReportDownload> ReportDownloads { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<SupplierGroup> SupplierGroups { get; set; }
    public DbSet<MessageTemplate> MessageTemplates { get; set; }
    public DbSet<SystemNumber> SystemNumbers { get; set; }
    
    //school manager...
    public DbSet<AcademicYear> AcademicYears { get; set; }
    public DbSet<AlumniHistory> AlumniHistories { get; set; }
}
