using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.GeneralLedger.ImprestCategories.Dto;

public class CreateImprestCategoryInput
{
    public string Name { get; set; }
}

public class UpdateImprestCategoryInput : EntityDto<Guid>
{
    public string Name { get; set; }
}