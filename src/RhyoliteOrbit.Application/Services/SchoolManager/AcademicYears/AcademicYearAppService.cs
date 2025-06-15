using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.SchoolManager.AcademicYears;
using RhyoliteOrbit.DomainServices.SchoolManager.AcademicYears.Dto;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.SchoolManager.AcademicYears.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.AcademicYears;

public class AcademicYearAppService : RhyoliteOrbitAppServiceBase
{
    private readonly IAcademicYearManager _academicYearManager;
    private readonly IMapper _mapper;

    public AcademicYearAppService(IAcademicYearManager academicYearManager, IMapper mapper)
    {
        _academicYearManager = academicYearManager;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<object> Create(CreateAcademicYearInput input)
    {
        var obj = _mapper.Map<AcademicYear>(input);
        return await _academicYearManager.Create(obj);
    }

    [HttpGet]
    public async Task<object> GetAsync(Guid id)
    {
        return await _academicYearManager.GetAsync(id);
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _academicYearManager.GetAll(pageNo,pageSize,query);

    }

    [HttpPost]
    public async Task Update(UpdateAcademicYearInput input)
    {
        var obj = _mapper.Map<AcademicYear>(input);
        await _academicYearManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _academicYearManager.Delete(id);
    }

    //terms
    [HttpPost]
    public async Task<object> CreateTerm(TermDto input)
    {
        return await _academicYearManager.CreateTerm(input);
    }

    [HttpPost]
    public async Task UpdateTerm(TermDto input)
    {
        await _academicYearManager.UpdateTerm(input);
    }

    [HttpDelete]
    public async Task DeleteTerm(TermDto input)
    {
        await _academicYearManager.DeleteTerm(input);
    }
}