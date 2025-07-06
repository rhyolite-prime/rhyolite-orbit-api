using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.Services.Payroll.SalaryGrades.Dto;

public class CreateSalaryGradeInput
{
    public string Name { get; set; }
    public int NumericalOrder { get; set; }
    public List<SalaryNotch> SalaryNotches { get; set; }
}

public class UpdateSalaryGradeInput: EntityDto<Guid>
{
    public string Name { get; set; }
    public int NumericalOrder { get; set; }
    public List<SalaryNotch> SalaryNotches { get; set; }
}