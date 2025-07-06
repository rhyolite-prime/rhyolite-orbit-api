using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeCategories.Dto;

public class CreateEmployeeCategoryInput
{
    public string Name { get; set; }
}

public class UpdateEmployeeCategoryInput: EntityDto<Guid>
{
    public string Name { get; set; }
}