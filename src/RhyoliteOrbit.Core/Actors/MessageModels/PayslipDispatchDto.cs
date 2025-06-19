using System;
using System.Collections.Generic;
using System.IO;
using RhyoliteOrbit.DomainServices.Payroll.Reports.Dto;

namespace RhyoliteOrbit.Actors.MessageModels;

public class PayslipDispatchDto
{
    public Guid[] EmployeeIds { get; set; }
    public List<EmployeePayslip> EmployeePayslip { get; set; }
    public int IntervalCount { get; set; }
    public string IntervalType { get; set; }
    public int BatchSize { get; set; }
}

public class ProcessBatch
{
    
    
}

public class EmailAttachment
{
    public string FileName { get; set; }
    public Stream Content { get; set; }
    public string MimeType { get; set; }
}