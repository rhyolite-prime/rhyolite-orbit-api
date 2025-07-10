using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.Payroll.AllowanceTypes.Dto;
using RhyoliteOrbit.Services.Payroll.BikTypes.Dto;
using RhyoliteOrbit.Services.Payroll.DeductionTypes.Dto;
using RhyoliteOrbit.Services.Payroll.EmployeeCategories.Dto;
using RhyoliteOrbit.Services.Payroll.OvertimeTypes.Dto;
using RhyoliteOrbit.Services.SchoolManager.AcademicYears.Dto;
using RhyoliteOrbit.Services.SchoolManager.BillTypes.Dto;

namespace RhyoliteOrbit;

public class MapProfile: AutoMapper.Profile
{
    public MapProfile()
    {
        CreateMap<CreateAcademicYearInput, AcademicYear>();
        CreateMap<UpdateAcademicYearInput, AcademicYear>();
        
        CreateMap<CreateBillTypeInput, BillType>();
        CreateMap<UpdateBillTypeInput, BillType>();
        
        //payroll
        CreateMap<CreateAllowanceTypeInput, AllowanceType>();
        CreateMap<UpdateAllowanceTypeInput, AllowanceType>();
        
        CreateMap<CreateEmployeeCategoryInput, EmployeeCategory>();
        CreateMap<UpdateEmployeeCategoryInput, EmployeeCategory>();
        
        CreateMap<CreateBikTypeInput, BikType>();
        CreateMap<UpdateBikTypeInput, BikType>();
        
        CreateMap<CreateDeductionTypeInput, DeductionType>();
        CreateMap<UpdateDeductionTypeInput, DeductionType>();
        
        CreateMap<CreateOvertimeTypeInput, OvertimeType>();
        CreateMap<UpdateOvertimeTypeInput, OvertimeType>();
        
        
        
    }
}