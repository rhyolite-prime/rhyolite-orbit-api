using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Application.Services.SchoolManager.SubjectRemarks.Dto
{
    public class CreateSubjectRemarkInput
    {
        public decimal MinimumMarks { get; set; }
        public decimal MaximumMarks { get; set; }
        public string Remarks { get; set; }
    }

    public class UpdateSubjectRemarkInput : EntityDto<Guid>
    {
        public decimal MinimumMarks { get; set; }
        public decimal MaximumMarks { get; set; }
        public string Remarks { get; set; }
    }
}
