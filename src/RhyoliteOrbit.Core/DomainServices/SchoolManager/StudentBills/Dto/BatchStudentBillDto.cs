using System;
using System.Collections.Generic;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.StudentBills.Dto;

public class StudentBillBatchDto
{
    public Guid BillSetupId { get; set; }
    public Guid AcademicYearId { get; set; }
    public Guid TermId { get; set; }
    public Guid ClassId { get; set; }
    public Guid BillTypeId { get; set; }
    public DateTime BillDate { get; set; }
    public string BillNo { get; set; }
}