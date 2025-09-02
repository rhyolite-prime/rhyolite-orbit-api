using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.SchClasses.Dto;

    public class CreateSchClassInput
    {
        public Guid LevelId { get; set; }
        public string LevelName { get; set; }
        public string ClassName { get; set; }
        public Guid StreamId { get; set; }
        public string StreamName { get; set; }
    }
    public class UpdateSchClassInput: EntityDto<Guid>
    {
        public Guid LevelId { get; set; }
        public string LevelName { get; set; }
        public string ClassName { get; set; }
        public Guid StreamId { get; set; }
        public string StreamName { get; set; }
    }
