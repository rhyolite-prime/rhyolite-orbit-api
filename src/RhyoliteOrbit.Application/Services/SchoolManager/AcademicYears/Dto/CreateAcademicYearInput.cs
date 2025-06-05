using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.Services.SchoolManager.AcademicYears.Dto;

public class CreateAcademicYearInput
{
    public string Name { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TenantId { get; set; }
    public int PrecedenceNo { get; set; }
    public List<Term> Terms { get; set; }
}

public class UpdateAcademicYearInput:EntityDto<Guid>
{
    public string Name { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TenantId { get; set; }
    public int PrecedenceNo { get; set; }
    public List<Term> Terms { get; set; }
}