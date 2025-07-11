using AutoMapper;
using RhyoliteOrbit.Application.Services.SchoolManager.SubjectRemarks.Dto;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.Payroll.FeesDescriptions.Dto;
using RhyoliteOrbit.Services.SchoolManager.AcademicYears.Dto;
using RhyoliteOrbit.Services.SchoolManager.BillTypes.Dto;
using RhyoliteOrbit.Services.SchoolManager.SchClasses.Dto;
using RhyoliteOrbit.Services.SchoolManager.ResultTypes.Dto;
using RhyoliteOrbit.Services.SchoolManager.Levels.Dto;
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
    }
}