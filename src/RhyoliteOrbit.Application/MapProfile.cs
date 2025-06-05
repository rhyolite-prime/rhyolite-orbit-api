using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.SchoolManager.AcademicYears.Dto;

namespace RhyoliteOrbit;

public class MapProfile: AutoMapper.Profile
{
    public MapProfile()
    {
        CreateMap<CreateAcademicYearInput, AcademicYear>();
        CreateMap<UpdateAcademicYearInput, AcademicYear>();
    }
}