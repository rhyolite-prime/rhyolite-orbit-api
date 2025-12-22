using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.Subjects.Dto
{
    public class CreateSubjectInput
    {
        public string Name { get; set; }
        public Guid SubjectCategoryId { get; set; }
        public string SubjectCategoryName { get; set; }
    }

    public class UpdateSubjectInput : EntityDto<Guid>
    {
        public string Name { get; set; }
        public Guid SubjectCategoryId { get; set; }
        public string SubjectCategoryName { get; set; }
    }
   
}



