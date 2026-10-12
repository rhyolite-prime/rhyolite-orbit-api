using System;

namespace RhyoliteOrbit.Models.Dms;

/// <summary>Long running bulk import/export job.</summary>
public class DmsIngestJob: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    /// <summary>0 = ImportZip, 1 = ExportZip, 2 = EmailIngest (see <see cref="RhyoliteOrbitConsts.IngestJobType"/>).</summary>
    public int JobType { get; set; }

    /// <summary>0 = Queued, 1 = Running, 2 = Completed, 3 = Failed, 4 = Cancelled (see <see cref="RhyoliteOrbitConsts.IngestJobStatus"/>).</summary>
    public int Status { get; set; }

    public int TotalItems { get; set; }

    public int ProcessedItems { get; set; }

    public int FailedItems { get; set; }

    public string SourcePath { get; set; }

    public string ResultPath { get; set; }

    public string ErrorLog { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public long CreatedById { get; set; }

    public int TenantId { get; set; }
}
