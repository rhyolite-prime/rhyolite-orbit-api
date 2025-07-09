using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.ResultTypes.Dto
{
    public class CreateResultTypeInput 
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UpdateResultTypeInput : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}




