using RhyoliteOrbit.Application.Services.SchoolManager.SubjectRemarks.Dto;
using RhyoliteOrbit.Models.GeneralLedger;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Models.ProductionPlanning;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Models.Shared;
using RhyoliteOrbit.Services.GeneralLedger.AccountHeads.Dto;
using RhyoliteOrbit.Services.GeneralLedger.CoaControls.Dto;
using RhyoliteOrbit.Services.GeneralLedger.CoaDetails.Dto;
using RhyoliteOrbit.Services.Payroll.AllowanceTypes.Dto;
using RhyoliteOrbit.Services.Payroll.BikTypes.Dto;
using RhyoliteOrbit.Services.Payroll.DeductionTypes.Dto;
using RhyoliteOrbit.Services.Payroll.EmployeeCategories.Dto;
using RhyoliteOrbit.Services.Payroll.EmployeeRanks.Dto;
using RhyoliteOrbit.Services.Payroll.FeesDescriptions.Dto;
using RhyoliteOrbit.Services.Payroll.Gratuities.Dto;
using RhyoliteOrbit.Services.Payroll.LoanTypes.Dto;
using RhyoliteOrbit.Services.Payroll.OvertimeTypes.Dto;
using RhyoliteOrbit.Services.Payroll.PayCalendars.Dto;
using RhyoliteOrbit.Services.Payroll.TaxReliefs.Dto;
using RhyoliteOrbit.Services.Payroll.TaxTables.Dto;
using RhyoliteOrbit.Services.ProductionPlanning.RawMaterials.Dto;
using RhyoliteOrbit.Services.SchoolManager.AcademicYears.Dto;
using RhyoliteOrbit.Services.SchoolManager.BillSetups.Dto;
using RhyoliteOrbit.Services.SchoolManager.BillTypes.Dto;
using RhyoliteOrbit.Services.SchoolManager.ClassStreams.Dto;
using RhyoliteOrbit.Services.SchoolManager.Levels.Dto;
using RhyoliteOrbit.Services.SchoolManager.Parents.Dto;
using RhyoliteOrbit.Services.SchoolManager.ResultTypes.Dto;
using RhyoliteOrbit.Services.SchoolManager.SchClasses.Dto;
using RhyoliteOrbit.Services.SchoolManager.Staffs.Dto;
using RhyoliteOrbit.Services.SchoolManager.Students.Dto;
using RhyoliteOrbit.Services.SchoolManager.SubjectCategories.Dto;
using RhyoliteOrbit.Services.SchoolManager.Subjects.Dto;
using RhyoliteOrbit.Services.Shared.Currencies.Dto;
using RhyoliteOrbit.Services.Shared.MessageTemplates.Dto;
using RhyoliteOrbit.Services.Shared.UnitOfMeasurements.Dto;

namespace RhyoliteOrbit;

public class MapProfile: AutoMapper.Profile
{
    public MapProfile()
    {
        
        //shared
        CreateMap<CreateCurrencyInput, Currency>();
        CreateMap<UpdateCurrencyInput, Currency>();
        
        CreateMap<CreateMessageTemplateInput, MessageTemplate>();
        CreateMap<UpdateMessageTemplateInput, MessageTemplate>();
        
        //school manager
        CreateMap<CreateAcademicYearInput, AcademicYear>();
        CreateMap<UpdateAcademicYearInput, AcademicYear>();

        CreateMap<CreateBillTypeInput, BillType>();
        CreateMap<UpdateBillTypeInput, BillType>();
        
        CreateMap<CreateBillSetupInput, BillSetup>();
        CreateMap<BillSetup, BillSetupInfo>();

        CreateMap<CreateFeesDescriptionInput, FeesDescription>();
        CreateMap<UpdateFeesDescriptionInput, FeesDescription>();
        
        CreateMap<CreateClassStreamInput, ClassStream>();
        CreateMap<UpdateClassStreamInput, ClassStream>();

        CreateMap<CreateSchClassInput, SchClass>();
        CreateMap<UpdateSchClassInput, SchClass>();

        CreateMap<CreateResultTypeInput, ResultType>();
        CreateMap<UpdateResultTypeInput, ResultType>();

        CreateMap<CreateLevelInput, Level>();
        CreateMap<UpdateLevelInput, Level>();

        CreateMap<CreateSubjectInput, Subject>();
        CreateMap<UpdateSubjectInput, Subject>();
        
        CreateMap<CreateSubjectRemarkInput, SubjectRemark>();
        CreateMap<UpdateSubjectRemarkInput, SubjectRemark>();
        
        CreateMap<CreateSubjectCategoryInput, SubjectCategory>();
        CreateMap<UpdateSubjectCategoryInput, SubjectCategory>();
        
        CreateMap<CreateStudentInput, Student>();
        CreateMap<UpdateStudentInput, Student>();
        
        CreateMap<CreateStaffInput, Staff>();
        CreateMap<UpdateStaffInput, Staff>();
        
        CreateMap<CreateParentInput, Parent>();
        CreateMap<UpdateParentInput, Parent>();
        
        
        //payroll
        CreateMap<CreateAllowanceTypeInput, AllowanceType>();
        CreateMap<UpdateAllowanceTypeInput, AllowanceType>();
        
        CreateMap<CreateBikTypeInput, BikType>();
        CreateMap<UpdateBikTypeInput, BikType>();
        
        CreateMap<CreateDeductionTypeInput, DeductionType>();
        CreateMap<UpdateDeductionTypeInput, DeductionType>();
        
        CreateMap<CreateEmployeeCategoryInput, EmployeeCategory>();
        CreateMap<UpdateEmployeeCategoryInput, EmployeeCategory>();
        
        CreateMap<CreateOvertimeTypeInput, OvertimeType>();
        CreateMap<UpdateOvertimeTypeInput, OvertimeType>();
        
        CreateMap<CreateLoanTypeInput, LoanType>();
        CreateMap<UpdateLoanTypeInput, LoanType>();
        
        CreateMap<CreateEmployeeRankInput, EmployeeRank>();
        CreateMap<UpdateEmployeeRankInput, EmployeeRank>();
        
        CreateMap<CreateTaxReliefInput, TaxRelief>();
        CreateMap<UpdateTaxReliefInput, TaxRelief>();
        
        CreateMap<CreateTaxTableInput, TaxTable>();
        CreateMap<UpdateTaxTableInput, TaxTable>();
        
        CreateMap<CreateGratuityInput, Gratuity>();
        CreateMap<UpdateGratuityInput, Gratuity>();
        
        CreateMap<CreatePayCalendarInput, PayCalendar>();
        CreateMap<UpdatePayCalendarInput, PayCalendar>();
        
        
        
        
        //gl
        CreateMap<CreateAccountHeadInput, AccountHead>();
        CreateMap<UpdateAccountHeadInput, AccountHead>();
        
        CreateMap<CreateCoaControlInput, CoaControl>();
        CreateMap<UpdateCoaControlInput, CoaControl>();
        
        CreateMap<CreateCoaDetailInput, CoaDetail>();
        CreateMap<UpdateCoaDetailInput, CoaDetail>();
        
        //manufacturing...
        
        CreateMap<CreateUnitOfMeasureInput, UnitOfMeasure>();
        CreateMap<UpdateUnitOfMeasureInput, UnitOfMeasure>();
        
        CreateMap<CreateRawMaterialInput, RawMaterial>();
        CreateMap<UpdateRawMaterialInput, RawMaterial>();
    }
}