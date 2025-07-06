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


public class PayslipEmailRequestDto
{
    public Guid[] EmployeeIds { get; set; }
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

public class EmailNotification
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public bool EnableSsl { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string SenderName { get; set; }
    public bool IsBodyHtml { get; set; }
}