using System;
using System.Collections.Generic;

namespace RhyoliteOrbit.Services.SchoolManager.StudentAttendances.Dto;

    public class CreateStudentAttendanceInput
    {
        public string Teachers { get; set; }
        public string Subjects { get; set; }
        public string Students { get; set; }
    }
    public class UpdateStudentAttendanceInput: CreateStudentAttendanceInput
    {
        public Guid Id { get; set; }
        public List<Guid> Teachers { get; set; }
        public List<Guid> Subjects { get; set; }
        public List<Guid> Students { get; set; }
    }
