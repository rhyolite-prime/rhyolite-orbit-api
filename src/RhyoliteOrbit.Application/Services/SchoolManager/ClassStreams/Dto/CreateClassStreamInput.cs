using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.ClassStreams.Dto;

public class CreateClassStreamInput
{
    public string Name { get; set; }
}

public class UpdateClassStreamInput: EntityDto<Guid>
{
    public string Name { get; set; }
}