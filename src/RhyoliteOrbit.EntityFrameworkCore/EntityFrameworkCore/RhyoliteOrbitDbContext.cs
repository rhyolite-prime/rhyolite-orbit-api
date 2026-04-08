using System;
using Abp.Zero.EntityFrameworkCore;
using RhyoliteOrbit.Authorization.Roles;
using RhyoliteOrbit.Authorization.Users;
using RhyoliteOrbit.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Dms;
using RhyoliteOrbit.Models.GeneralLedger;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Models.ProductionPlanning;
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
    public DbSet<Department> Departments { get; set; }
    public DbSet<SystemSetting> SystemSetting { get; set; }
    public DbSet<ReportDownload> ReportDownloads { get; set; }  
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<PartnerCategory> PartnerCategories { get; set; }
    public DbSet<MessageTemplate> MessageTemplates { get; set; }
    public DbSet<SystemNumber> SystemNumbers { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<BusinessDate> BusinessDates { get; set; }
    public DbSet<UnitOfMeasure> UnitOfMeasurements { get; set; }
    public DbSet<Tag> Tags { get; set; }
    
    
    //school manager...
    public DbSet<AcademicYear> AcademicYears { get; set; }
    public DbSet<AssignedClass> AssignedClasses { get; set; }
    public DbSet<StudentBehaviourRecord> StudentBehaviorRecords { get; set; }
    public DbSet<StaffDesignation> StaffDesignations { get; set; }
    public DbSet<StudentAttendance> StudentAttendances { get; set; }
    public DbSet<StudentBillReceipt> StudentBillReceipts { get; set; }
    public DbSet<StudentBill> StudentBills { get; set; }
    public DbSet<StudentBillReportHistory> StudentBillReportHistory { get; set; }
    public DbSet<BillPayment> BillPayments { get; set; }
    public DbSet<BillSetup> BillSetups { get; set; }
    public DbSet<BillType> BillTypes { get; set; }
    public DbSet<CancelledBill> CancelledBills { get; set; }
    public DbSet<CancelledPayment> CancelledPayments { get; set; }
    public DbSet<ClassStream> ClassStreams { get; set; }
    public DbSet<FeesDescription> FeesDescriptions { get; set; }
    public DbSet<GradingScheme> GradingSchemes { get; set; }
    public DbSet<Level> Levels { get; set; }
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
    public DbSet<StudentStatement> StudentStatements { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<SubjectRemark> SubjectRemarks { get; set; }
    public DbSet<TerminalReport> TerminalReports { get; set; }
    public DbSet<MiscellaneousPayment> MiscellaneousPayments { get; set; }
    
    //Academic Management
    public DbSet<Department> AcademicDepartments { get; set; }
    public DbSet<Curriculum> Curricula { get; set; }
    public DbSet<Syllabus> Syllabi { get; set; }
    public DbSet<LessonPlan> LessonPlans { get; set; }
    public DbSet<TeacherSubjectAssignment> TeacherSubjectAssignments { get; set; }
    public DbSet<ElectiveSubject> ElectiveSubjects { get; set; }
    public DbSet<SubjectCategory> SubjectCategories { get; set; }
    public DbSet<ExamSchedule> ExamSchedules { get; set; }
    public DbSet<AssessmentType> AssessmentTypes { get; set; }
    
    //Student Management
    public DbSet<StudentClubMembership> StudentClubMemberships { get; set; }
    public DbSet<Club> Clubs { get; set; }
    public DbSet<StudentCounseling> StudentCounselings { get; set; }
    public DbSet<Discipline> DisciplinaryActions { get; set; }
    public DbSet<StudentHealthRecord> StudentHealthRecords { get; set; }
    public DbSet<SpecialNeed> SpecialNeeds { get; set; }
    public DbSet<Scholarship> Scholarships { get; set; }
    public DbSet<StudentAward> StudentAwards { get; set; }
    
    // facilities and resources
    public DbSet<Dormitory> Dormitories { get; set; }
    public DbSet<DormitoryRoom> DormitoryRooms { get; set; }
    public DbSet<StudentDormitoryAssignment> StudentDormitoryAssignments { get; set; }
    public DbSet<SchoolLibrary> Libraries { get; set; }
    public DbSet<LibraryBook> LibraryBooks { get; set; }
    public DbSet<BookLending> BookLendings { get; set; }
    public DbSet<Laboratory> Laboratories { get; set; }
    public DbSet<LaboratoryEquipment> LaboratoryEquipments { get; set; }
    
    //Events and Activities
    public DbSet<SchoolEvent> Events { get; set; }
    public DbSet<SportsTeam> SportsTeams { get; set; }
    public DbSet<SportsCompetition> SportsCompetitions { get; set; }
    public DbSet<FieldTrip> FieldTrips { get; set; }
    
    //Communication
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<ParentTeacherMeeting> ParentTeacherMeetings { get; set; }
    public DbSet<StaffMeeting> StaffMeetings { get; set; }
    
    //general ledger
    public DbSet<AccountHead> AccountHeads { get; set; }
    public DbSet<CoaControl> CoaControls { get; set; }
    public DbSet<CoaDetail> CoaDetails { get; set; }
    public DbSet<PettyCashRecipient> PettyCashRecipients { get; set; }
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<Journal> Journals { get; set; }
    public DbSet<PvRetention> PvRetentions { get; set; }
    public DbSet<ImprestCategory> ImprestCategories { get; set; }
    public DbSet<Imprest> Imprests { get; set; }
    public DbSet<AccountingPeriod> AccountingPeriods { get; set; }
    public DbSet<BankAccount> BankAccounts { get; set; }
    public DbSet<AccountBalance> AccountBalances { get; set; }
    public DbSet<DefaultGlNumber> DefaultGlNumbers { get; set; }
    
    
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
    public DbSet<EmployeeBioData> EmployeeBioDatasx { get; set; }
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
    public DbSet<PayrollPeriod> PayrollPeriods { get; set; }
    public DbSet<EmployeeLoanRepaymentSchedule> EmployeeLoanRepaymentSchedules { get; set; }
    public DbSet<MonthlyAllowance> MonthlyAllowances { get; set; }
    public DbSet<MonthlyDeduction> MonthlyDeductions { get; set; }
    public DbSet<MonthlyBenefitsInKind> MonthlyBenefitsInKinds { get; set; }
    public DbSet<MonthlyRelief> MonthlyReliefs { get; set; }
    public DbSet<Paymaster> Paymasters { get; set; }
    public DbSet<MonthlyAllowanceHistory> MonthlyAllowanceHistory { get; set; }
    public DbSet<MonthlyDeductionHistory> MonthlyDeductionHistory { get; set; }
    public DbSet<MonthlyBenefitsInKindHistory> MonthlyBenefitsInKindHistory { get; set; }
    public DbSet<MonthlyReliefHistory> MonthlyReliefHistory { get; set; }
    public DbSet<PaymasterHistory> PaymasterHistory { get; set; }
    public DbSet<MonthlyCumulativeDeductionHistory> MonthlyCumulativeDeductionHistory { get; set; }
    public DbSet<MonthlyCumulativeDeduction> MonthlyCumulativeDeductions { get; set; }
    public DbSet<MonthlySsnitDeduction> MonthlySsnitDeductions { get; set; }
    public DbSet<MonthlySsnitDeductionHistory> MonthlySsnitDeductionHistory { get; set; }
    public DbSet<MonthlyOnetimeDeduction> MonthlyOnetimeDeductions { get; set; }
    public DbSet<MonthlyOnetimeDeductionHistory> MonthlyOnetimeDeductionHistory { get; set; }
    public DbSet<SalaryIncrementHistory> SalaryIncrementHistory { get; set; }
    public DbSet<MonthlyPfDeductionHistory> MonthlyPfDeductionHistory { get; set; }
    public DbSet<MonthlyPfDeduction> MonthlyPfDeductions { get; set; }
    public DbSet<MonthlyLoanDeductionHistory> MonthlyLoanDeductionHistory { get; set; }
    public DbSet<MonthlyLoanDeduction> MonthlyLoanDeductions { get; set; }
    public DbSet<MonthlyIrsTax> MonthlyIrsTaxes { get; set; }
    public DbSet<MonthlyIrsTaxHistory> MonthlyIrsTaxHistory { get; set; }
    public DbSet<MonthlySalaryAdvance> MonthlySalaryAdvances { get; set; }
    public DbSet<MonthlySalaryAdvanceHistory> MonthlySalaryAdvanceHistory { get; set; }
    public DbSet<MonthlySecPfDeduction> MonthlySecPfDeductions { get; set; }
    public DbSet<MonthlySecPfDeductionHistory> MonthlySecPfDeductionHistory { get; set; }
    public DbSet<MonthlyOvertime> MonthlyOvertimes { get; set; }
    public DbSet<MonthlyOvertimeHistory> MonthlyOvertimeHistory { get; set; }
    public DbSet<MonthlyBonus> MonthlyBonuses { get; set; }
    public DbSet<MonthlyBonusHistory> MonthlyBonusHistory { get; set; }
    
    //production planning and process control ...
    public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductionOrder> ProductionOrders { get; set; }
    public DbSet<Operation> Operations { get; set; }
    public DbSet<ProductionWorkflow> ProductionWorkflows { get; set; }
    public DbSet<WorkCenter> WorkCenters { get; set; }
    public DbSet<FinishedGood> FinishedGoods { get; set; }
    public DbSet<FinishedGoodAllocation> FinishedGoodAllocation { get; set; }
    public DbSet<ScrapAndWaste> ScrapAndWastes { get; set; }
    public DbSet<MaterialConsumption> MaterialConsumptions { get; set; }
    public DbSet<RawMaterial> RawMaterials { get; set; }
    public DbSet<BillOfMaterialItem> BillOfMaterialItems { get; set; }
    public DbSet<QcChecklist> QcChecklists { get; set; }
    public DbSet<InProcessInspection> InProcessInspections { get; set; }
    public DbSet<FinalProductTest> FinalProductTests { get; set; }
    public DbSet<NonConformance> NonConformance { get; set; }   
    //licensing
    public DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }
    public DbSet<SubscriptionRenewalHistory> SubscriptionRenewalHistory { get; set; }
    
    // document management system
    public DbSet<DmsFolder> Folders { get; set; }
    public DbSet<DmsDocument> Documents { get; set; }
    public DbSet<DmsTag> DmsTags { get; set; }
    public DbSet<DmsDocumentTemplate> DmsDocumentTemplates { get; set; }
    public DbSet<DmsCustomAttributes> DmsCustomAttributes { get; set; }
    public DbSet<DmsDocumentMetadataValues> DmsDocumentMetadataValues { get; set; }
    public DbSet<DmsCustomIdentifier> DmsCustomIdentifiers { get; set; }
    public DbSet<DmsDocumentTag> DmsDocumentTags { get; set; }
    public DbSet<DmsWorkflowTemplate> DmsWorkflowTemplates { get; set; }
    public DbSet<DmsWorkflowInstance> DmsWorkflowInstances { get; set; }
    public DbSet<DmsWorkflowTask> DmsWorkflowTasks { get; set; }
    public DbSet<DmsComment> DmsComments { get; set; }
    //public DbSet<DmsAnnotation> DmsAnnotations { get; set; }
    
    
   
}
