using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Dms;

namespace RhyoliteOrbit.DomainServices.Dms.Audit;

public class HashChainAuditTrail: DomainService, IAuditTrail
{
    private readonly IRepository<DmsAuditEntry, Guid> _repository;
    private readonly IAbpSession _abpSession;
    private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;

    public HashChainAuditTrail(
        IRepository<DmsAuditEntry, Guid> repository,
        IAbpSession abpSession,
        Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _abpSession = abpSession;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task RecordAsync(
        RhyoliteOrbitConsts.AuditAction action,
        Guid? documentId = null,
        Guid? folderId = null,
        string detailsJson = null,
        long? actorId = null)
    {
        var tenantId = _abpSession.TenantId ?? 0;
        var httpContext = _httpContextAccessor?.HttpContext;

        var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString();
        var userAgent = httpContext?.Request?.Headers?["User-Agent"].ToString();

        var lastEntry = await (await _repository.GetAllAsync())
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.Sequence)
            .FirstOrDefaultAsync();

        var sequence = (lastEntry?.Sequence ?? 0) + 1;
        var previousHash = lastEntry?.EntryHash ?? new string('0', 64);

        var entry = new DmsAuditEntry
        {
            TenantId = tenantId,
            Action = (int)action,
            DocumentId = documentId,
            FolderId = folderId,
            DetailsJson = detailsJson,
            ActorId = actorId ?? _abpSession.UserId,
            IpAddress = Truncate(ipAddress, 100),
            UserAgent = Truncate(userAgent, 300),
            Sequence = sequence,
            PreviousHash = previousHash,
            CreationTime = DateTime.UtcNow,
            CreatorUserId = actorId ?? _abpSession.UserId
        };

        entry.EntryHash = ComputeHash(entry);

        await _repository.InsertAsync(entry);
    }

    public async Task<PagedAuditResult> ListAsync(Guid? documentId, Guid? folderId, int action, int pageNo, int pageSize)
    {
        if (pageNo < 1)
        {
            pageNo = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 20;
        }

        var query = (await _repository.GetAllAsync()).AsQueryable();

        if (documentId.HasValue)
        {
            query = query.Where(x => x.DocumentId == documentId.Value);
        }

        if (folderId.HasValue)
        {
            query = query.Where(x => x.FolderId == folderId.Value);
        }

        if (action > 0)
        {
            query = query.Where(x => x.Action == action);
        }

        var totalCount = await query.CountAsync();

        var data = await query
            .OrderByDescending(x => x.Sequence)
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedAuditResult
        {
            PageNo = pageNo,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalCount > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0,
            Data = data
        };
    }

    public async Task<AuditChainVerification> VerifyChainAsync()
    {
        var tenantId = _abpSession.TenantId ?? 0;

        var entries = await (await _repository.GetAllAsync())
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Sequence)
            .ToListAsync();

        string previousHash = new string('0', 64);

        foreach (var entry in entries)
        {
            if (entry.PreviousHash != previousHash || entry.EntryHash != ComputeHash(entry))
            {
                return new AuditChainVerification
                {
                    IsValid = false,
                    VerifiedEntries = entry.Sequence - 1,
                    FirstBrokenSequence = entry.Sequence,
                    Message = $"Audit chain broken at sequence {entry.Sequence}."
                };
            }

            previousHash = entry.EntryHash;
        }

        return new AuditChainVerification
        {
            IsValid = true,
            VerifiedEntries = entries.Count,
            FirstBrokenSequence = 0,
            Message = "Audit chain is intact."
        };
    }

    private static string ComputeHash(DmsAuditEntry entry)
    {
        var canonical = string.Join("|",
            entry.Sequence,
            entry.Action,
            entry.DocumentId?.ToString() ?? string.Empty,
            entry.FolderId?.ToString() ?? string.Empty,
            entry.ActorId?.ToString() ?? string.Empty,
            entry.DetailsJson ?? string.Empty,
            entry.CreationTime.ToString("O"),
            entry.PreviousHash);

        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(canonical));

        var builder = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }
}
