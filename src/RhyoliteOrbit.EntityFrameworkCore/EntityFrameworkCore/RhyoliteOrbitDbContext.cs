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
    public DbSet<AssignedClass> AssignedClasses { get; set; }
    public DbSet<StudentBehaviorRecord> StudentBehaviorRecords { get; set; }
    public DbSet<StaffDesignation> StaffDesignations { get; set; }
    public DbSet<StudentAttendance> StudentAttendances { get; set; }
    public DbSet<StudentBillReceipt> StudentBillReceipts { get; set; }
    public DbSet<StudentBill> StudentBills { get; set; }
    public DbSet<BillPayment> BillPayments { get; set; }
    public DbSet<BillSetup> BillSetups { get; set; }
    public DbSet<BillType> BillTypes { get; set; }
    public DbSet<CancelledBill> CancelledBills { get; set; }
    public DbSet<CancelledPayment> CancelledPayments { get; set; }
    public DbSet<ClassStream> ClassStreams { get; set; }
    public DbSet<FeesDescription> FeesDescriptions { get; set; }
    public DbSet<GradingScheme> GradingSchemes { get; set; }
    public DbSet<Level> Levels { get; set; }
    public DbSet<Parent> Parents { get; set; }
    public DbSet<PaymentType> PaymentTypes { get; set; }
    public DbSet<PastQuestion> PastQuestions { get; set; }
    public DbSet<PromotionHistory> PromotionHistory { get; set; }
    public DbSet<ResultsUpload> ResultsUploads { get; set; }
    public DbSet<ResultType> ResultTypes { get; set; }
    public DbSet<SchClass> SchClasses { get; set; }
    public DbSet<SchoolProfile> SchoolProfiles { get; set; }
    public DbSet<Sibling> Siblings { get; set; }
    public DbSet<Staff> Staffs { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<StudentParent> StudentParents { get; set; }
    public DbSet<StudentStatement> StudentStatements { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<SubjectRemark> SubjectRemarks { get; set; }
    public DbSet<TerminalReport> TerminalReports { get; set; }
    public DbSet<MiscellaneousPayment> MiscellaneousPayments { get; set; }
   
}
