using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Dms;

namespace RhyoliteOrbit.DomainServices.Dms.Audit;

/// <summary>
/// Append only audit trail with a SHA-256 hash chain per tenant, so any later
/// modification of the log can be detected.
/// </summary>
public interface IAuditTrail
{
    Task RecordAsync(
        RhyoliteOrbitConsts.AuditAction action,
        Guid? documentId = null,
        Guid? folderId = null,
        string detailsJson = null,
        long? actorId = null);

    Task<PagedAuditResult> ListAsync(Guid? documentId, Guid? folderId, int action, int pageNo, int pageSize);

    Task<AuditChainVerification> VerifyChainAsync();
}

public class PagedAuditResult
{
    public int PageNo { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }

    public List<DmsAuditEntry> Data { get; set; }
}

public class AuditChainVerification
{
    public bool IsValid { get; set; }

    public long VerifiedEntries { get; set; }

    public long FirstBrokenSequence { get; set; }

    public string Message { get; set; }
}
