using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.Levels.Dto
{
    public class CreateLevelInput 
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UpdateLevelInput : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}



