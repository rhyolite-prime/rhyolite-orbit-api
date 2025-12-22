using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.SubjectCategories.Dto;

public class CreateSubjectCategoryInput
{
    public string Name { get; set; }  
    public string Description { get; set; }
}

public class UpdateSubjectCategoryInput: EntityDto<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
}