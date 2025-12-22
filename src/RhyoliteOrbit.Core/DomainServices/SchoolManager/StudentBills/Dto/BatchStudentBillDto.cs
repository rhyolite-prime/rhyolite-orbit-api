using System;
using System.Collections.Generic;
using RhyoliteOrbit.Actors.MessageModels;
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
    public bool IsBalanceBfHandledManually { get; set; }
}

public class StudentBillReportUpdateDto
{
 
    public List<StudentBillReport> StudentBillReportList { get; set; }
    public BillMetaData BillMetaData { get; set; }

    public string BatchDocumentUrl { get; set; }
    public int TenantId { get; set; }
}

public class StudentBillReport
{
    public Guid StudentId { get; set; }
    public string BillReportUrl { get; set; } = string.Empty;
}