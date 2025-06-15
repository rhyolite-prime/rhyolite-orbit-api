using System;
using System.Collections.Generic;

namespace RhyoliteOrbit.Models.SchoolManager;

public class BillSetupInfo
{
    public Guid Id { get; set; }
    public Guid AcademicYearId { get; set; }
    public string AcademicYearName { get; set; }
    public Guid ClassId { get; set; }
    public string ClassName { get; set; }
    public Guid TermId { get; set; }
    public string TermName { get; set; }
    public Guid BillTypeId { get; set; }
    public string BillTypeName { get; set; }
    public decimal TotalBillAmount { get; set; }
    public List<BillSetupDetail> Details { get; set; }
}