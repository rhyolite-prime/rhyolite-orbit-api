using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Services.SchoolManager.Subjects.Dto
{
    public class CreateSubjectInput
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Code { get; set; }

        public string Description { get; set; }
    }

    public class UpdateSubjectInput : CreateSubjectInput
    {
        public Guid Id { get; set; }
    }
   
}



