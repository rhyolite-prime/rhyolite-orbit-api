using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.Students.Dto;

public class CreateStudentInput
{
    public string Name { get; set; }
}
public class UpdateStudentInput: EntityDto<Guid>
{
    public string Name { get; set; }
}