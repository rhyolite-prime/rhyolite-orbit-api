using System;
using System.Collections.Generic;

namespace RhyoliteOrbit.Actors.MessageModels;

public class StudentBillDto
{
    public BillReportHeader ReportHeader { get; set; }
    public StudentBillDetail StudentBill { get; set; }
    public StudentInfo StudentInfo { get; set; }
    public BillMetaData BillMetaData { get; set; }
}

public class BillMetaData
{
    public Guid BillSetupId { get; set; }
    public Guid AcademicYearId { get; set; }
    public Guid TermId { get; set; }
    public Guid ClassId { get; set; }
    public Guid BillTypeId { get; set; }
    public DateTime BillDate { get; set; }
    public string BillNo { get; set; }
}

public class BillReportHeader
{
    public string SchoolName { get; set; }
    public string PrimaryPhoneNo { get; set; }
    public string SchAddress { get; set; }
    public string ReportTitle { get; set; } = "https://res.cloudinary.com/rhyoliteprime/image/upload/v1673255393/school_manager_repository/cris-logo.jpg";
    public string LogoUrl { get; set; }
    public string Email { get; set; }

}

public class StudentInfo
{
    public string FullName { get; set; }
    public string EnrollmentDate { get; set; }
    public string StudentIdentifier { get; set; }
    public string Gender { get; set; }
    public string ClassName { get; set; }
    public string DateOfBirth { get; set; }
    public Guid Id { get; set; }
}

public class StudentBillDetail
{
    public List<BillItem> Details { get; set; }
    public decimal BillBalance { get; set; }
    public decimal BillAmount { get; set; }
    public string BillNo { get; set; }
    public DateTime BillDate { get; set; }
    public string Period { get; set; }
    public decimal BalanceBF { get; set; }
    public string CurrencyName { get; set; }
    public string CurrencyCode { get; set; }
    
    public List<string> PaymentChannels { get; set; }
}

public class BillItem
{
    public Guid Id { get; set; }
    public Guid FeeId { get; set; }
    public string FeeName { get; set; }
    public Guid BillTypeId { get; set; }
    public string BillTypeName { get; set; }
    public decimal FeeAmount { get; set; }

}