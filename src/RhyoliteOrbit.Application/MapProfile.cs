using RhyoliteOrbit.Application.Services.SchoolManager.SubjectRemarks.Dto;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.Payroll.AllowanceTypes.Dto;
using RhyoliteOrbit.Services.Payroll.BikTypes.Dto;
using RhyoliteOrbit.Services.Payroll.DeductionTypes.Dto;
using RhyoliteOrbit.Services.Payroll.EmployeeCategories.Dto;
using RhyoliteOrbit.Services.Payroll.FeesDescriptions.Dto;
using RhyoliteOrbit.Services.Payroll.LoanTypes.Dto;
using RhyoliteOrbit.Services.Payroll.OvertimeTypes.Dto;
using RhyoliteOrbit.Services.SchoolManager.AcademicYears.Dto;
using RhyoliteOrbit.Services.SchoolManager.BillTypes.Dto;
using RhyoliteOrbit.Services.SchoolManager.ClassStreams.Dto;
using RhyoliteOrbit.Services.SchoolManager.Levels.Dto;
using RhyoliteOrbit.Services.SchoolManager.ResultTypes.Dto;
using RhyoliteOrbit.Services.SchoolManager.SchClasses.Dto;
using RhyoliteOrbit.Services.SchoolManager.Subjects.Dto;

namespace RhyoliteOrbit;

public class MapProfile: AutoMapper.Profile
{
    public MapProfile()
    {
        CreateMap<CreateAcademicYearInput, AcademicYear>();
        CreateMap<UpdateAcademicYearInput, AcademicYear>();

        CreateMap<CreateBillTypeInput, BillType>();
        CreateMap<UpdateBillTypeInput, BillType>();

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
        
        
        
    }
}