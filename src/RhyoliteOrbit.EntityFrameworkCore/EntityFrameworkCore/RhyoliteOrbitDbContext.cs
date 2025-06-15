using System;
using Abp.Zero.EntityFrameworkCore;
using RhyoliteOrbit.Authorization.Roles;
using RhyoliteOrbit.Authorization.Users;
using RhyoliteOrbit.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;
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
    public DbSet<Country> Countries { get; set; }
    
    //school manager...
    public DbSet<AcademicYear> AcademicYears { get; set; }
    public DbSet<AssignedClass> AssignedClasses { get; set; }
    public DbSet<StudentBehaviourRecord> StudentBehaviorRecords { get; set; }
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
    
    //payroll
    public DbSet<EmployeeCategory> EmployeeCategories { get; set; }
    public DbSet<SalaryAdvanceApplication> SalaryAdvanceApplications { get; set; }
    public DbSet<AllowanceType> AllowanceTypes { get; set; }
    public DbSet<DeductionType> DeductionTypes { get; set; }
    public DbSet<LoanType> LoanTypes { get; set; }
    public DbSet<TaxRelief> TaxReliefs { get; set; }
    public DbSet<TaxTable> TaxTables { get; set; }
    public DbSet<TaxTableHistory> TaxTableHistory { get; set; }
    public DbSet<EmployeeRank> EmployeeRanks { get; set; }
    public DbSet<Gratuity> Gratuities { get; set; }
    public DbSet<BikType> BikTypes { get; set; }
    public DbSet<PayCalendar> PayCalendars { get; set; }
    public DbSet<SalaryGrade> SalaryGrades { get; set; }
    public DbSet<OvertimeType> OvertimeTypes { get; set; }
    public DbSet<EmployeeDaysWorked> EmployeeDaysWorked { get; set; }
    public DbSet<BonusAndOnetimeAllowance> BonusAndOnetimeAllowances { get; set; }
    public DbSet<EmployeeBioData> EmployeeBioDatas { get; set; }
    public DbSet<EmployeeSalaryData> EmployeeSalaryData { get; set; }
    public DbSet<EmployeeSnit> EmployeeSnits { get; set; }
    public DbSet<EmployeeSalaryAdvance> EmployeeSalaryAdvances { get; set; }
    public DbSet<EmployeeOnetimeDeduction> EmployeeOnetimeDeductions { get; set; }
    public DbSet<LoanApplication> LoanApplications { get; set; }
    public DbSet<EmployeeAllowance> EmployeeAllowances { get; set; }
    public DbSet<EmployeeDeduction> EmployeeDeductions { get; set; }
    public DbSet<EmployeeBenefitInKind> EmployeeBenefitInKinds { get; set; }
    public DbSet<EmployeeRelief> EmployeeReliefs { get; set; }
    public DbSet<OvertimeTimeSheet> OvertimeTimeSheets { get; set; }
    public DbSet<EmployeeLoan> EmployeeLoans { get; set; }
    public DbSet<LoanAmortizationHistory> LoanAmortizationHistory { get; set; }
    public DbSet<InitializePayMonth> InitializePayMonths { get; set; }
    public DbSet<EmployeeLoanRepaymentSchedule> EmployeeLoanRepaymentSchedules { get; set; }
    public DbSet<MonthlyAllowance> MonthlyAllowances { get; set; }
    public DbSet<MonthlyDeduction> MonthlyDeductions { get; set; }
    public DbSet<MonthlyBenefitsInKind> MonthlyBenefitsInKinds { get; set; }
    public DbSet<MonthlyRelief> MonthlyReliefs { get; set; }
    public DbSet<Paymaster> Paymasters { get; set; }
    public DbSet<MonthlyAllowanceHist> MonthlyAllowanceHistory { get; set; }
    public DbSet<MonthlyDeductionHist> MonthlyDeductionHistory { get; set; }
    public DbSet<MonthlyBenefitsInKindHist> MonthlyBenefitsInKindHistory { get; set; }
    public DbSet<MonthlyReliefHist> MonthlyReliefHistory { get; set; }
    public DbSet<PaymasterHist> PaymasterHistory { get; set; }
    public DbSet<MonthlyCumulativeDeductionHist> MonthlyCumulativeDeductionHistory { get; set; }
    public DbSet<MonthlyCumulativeDeduction> MonthlyCumulativeDeductions { get; set; }
    public DbSet<MonthlySsnitDeduction> MonthlySsnitDeductions { get; set; }
    public DbSet<MonthlySsnitDeductionHist> MonthlySsnitDeductionHistory { get; set; }
    public DbSet<MonthlyOnetimeDeduction> MonthlyOnetimeDeductions { get; set; }
    public DbSet<MonthlyOnetimeDeductionHist> MonthlyOnetimeDeductionHistory { get; set; }
    public DbSet<SalaryIncrementHistory> SalaryIncrementHistory { get; set; }
    public DbSet<MonthlyPfDeductionHist> MonthlyPfDeductionHistory { get; set; }
    public DbSet<MonthlyPfDeduction> MonthlyPfDeductions { get; set; }
    public DbSet<MonthlyLoanDeductionHist> MonthlyLoanDeductionHistory { get; set; }
    public DbSet<MonthlyLoanDeduction> MonthlyLoanDeductions { get; set; }
    public DbSet<MonthlyIrsTaxHist> MonthlyIrsTaxHistory { get; set; }
    public DbSet<MonthlyIrsTax> MonthlyIrsTaxes { get; set; }
    public DbSet<MonthlySalaryAdvance> MonthlySalaryAdvances { get; set; }
    public DbSet<MonthlySecPfDeduction> MonthlySecPfDeductions { get; set; }
    public DbSet<MonthlySalaryAdvanceHist> MonthlySalaryAdvanceHistory { get; set; }
    public DbSet<MonthlySecPfDeductionHist> MonthlySecPfDeductionHistory { get; set; }
    public DbSet<MonthlyOvertimeHistory> MonthlyOvertimeHistory { get; set; }
    public DbSet<MonthlyOvertime> MonthlyOvertimes { get; set; }
    public DbSet<MonthlyBonus> MonthlyBonuses { get; set; }
    public DbSet<MonthlyBonusHistory> MonthlyBonusHistory { get; set; }
    public DbSet<IrsSignature> IrsSignatures { get; set; }
   
}
