using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;

namespace RhyoliteOrbit.Models.Shared;

public class ReportDownload:FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(100)]
    public string ReportKey { get; set; }
    [MaxLength(100)]
    public string ReportOptions { get; set; }
    public DateTime Date { get; set; }
    [MaxLength(100)]
    public string RequestedBy { get; set; }
    [MaxLength(100)]
    public string Status { get; set; }
    [MaxLength(100)]
    public string AccountSource { get; set; }
    [MaxLength(100)]
    public string ReportFilePath { get; set; }
    [MaxLength(350)]
    public string PublicAccessUrl { get; set; }
    [MaxLength(350)]
    public string ReportServiceBaseUrl { get; set; }
    public int TenantId { get; set; }
}