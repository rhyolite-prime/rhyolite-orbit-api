using System;
using System.Collections.Generic;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.Services.SchoolManager.BillSetups.Dto;

public class CreateBillSetupInput
{
    public Guid AcademicYearId { get; set; }
    public string AcademicYearName { get; set; }
    public Guid ClassId { get; set; }
    public string ClassName { get; set; }
    public Guid TermId { get; set; }
    public string TermName { get; set; }
    public Guid BillTypeId { get; set; }
    public string BillTypeName { get; set; }
    public decimal TotalBillAmount { get; set; }
    public int Quantity { get; set; }
    public string Unit { get; set; }
    public List<BillSetupDetail> Details { get; set; }
}