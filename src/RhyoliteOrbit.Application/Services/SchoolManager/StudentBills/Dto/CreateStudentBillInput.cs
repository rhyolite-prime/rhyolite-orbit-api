using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.StudentBills.Dto;

public class CreateStudentBillInput
{
    public string Name { get; set; }
}
public class UpdateStudentBillInput: EntityDto<Guid>
{
    public string Name { get; set; }
}