using RhyoliteOrbit.Models.SchoolManager;
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
    }
}