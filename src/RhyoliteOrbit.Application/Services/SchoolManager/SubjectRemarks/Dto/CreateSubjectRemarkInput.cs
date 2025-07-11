using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Application.Services.SchoolManager.SubjectRemarks.Dto
{
    public class CreateSubjectRemarkInput
    {
        public string Remark { get; set; }
        public Guid StudentId { get; set; }
        public Guid SubjectId { get; set; }
    }

    public class UpdateSubjectRemarkInput : EntityDto<Guid>
    {
        public string Remark { get; set; }
        public Guid StudentId { get; set; }
        public Guid SubjectId { get; set; }
    }
}
