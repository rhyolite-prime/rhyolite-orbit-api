using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Dms;
using RhyoliteOrbit.DomainServices.Dms.Versions;

namespace RhyoliteOrbit.DomainServices.Dms.Signatures;

public interface ISignatureService
{
    Task<DmsSignature> SignAsync(Guid documentId, Guid? versionId, long signerId, int signatureType, string signatureImageBase64, string reason, string ipAddress);

    Task<List<DmsSignature>> ListAsync(Guid documentId);

    Task<bool> VerifyAsync(Guid signatureId);

    Task<DmsSignature> GetAsync(Guid signatureId);
}

/// <summary>
/// Electronic signatures. The signature binds the signer, the document version and
/// the SHA-256 of the signed binary so it can be verified later.
/// </summary>
public class SignatureService: DomainService, ISignatureService
{
    private readonly IRepository<DmsSignature, Guid> _repository;
    private readonly IRepository<DmsDocument, Guid> _documentRepository;
    private readonly IVersionManager _versionManager;
    private readonly IAbpSession _abpSession;

    public SignatureService(
        IRepository<DmsSignature, Guid> repository,
        IRepository<DmsDocument, Guid> documentRepository,
        IVersionManager versionManager,
        IAbpSession abpSession)
    {
        _repository = repository;
        _documentRepository = documentRepository;
        _versionManager = versionManager;
        _abpSession = abpSession;
    }

    public async Task<DmsSignature> SignAsync(
        Guid documentId,
        Guid? versionId,
        long signerId,
        int signatureType,
        string signatureImageBase64,
        string reason,
        string ipAddress)
    {
        var document = await _documentRepository.FirstOrDefaultAsync(documentId);
        if (document == null)
        {
            throw new UserFriendlyException("DmsDocumentNotFound", "The requested document was not found.");
        }

        var version = versionId.HasValue
            ? await _versionManager.GetAsync(versionId.Value)
            : await _versionManager.GetCurrentAsync(documentId);

        if (version == null)
        {
            throw new UserFriendlyException("DmsVersionNotFound", "The requested document version was not found.");
        }

        var imagePath = await SaveSignatureImageAsync(signatureImageBase64, documentId, signerId);

        var signature = new DmsSignature
        {
            TenantId = _abpSession.TenantId ?? 0,
            DocumentId = documentId,
            VersionId = version.Id,
            SignerId = signerId,
            SignatureType = signatureType,
            SignatureImagePath = imagePath,
            SignedHash = version.Checksum,
            SignedAt = DateTime.UtcNow,
            IpAddress = ipAddress,
            Reason = reason
        };

        return await _repository.InsertAsync(signature);
    }

    public async Task<List<DmsSignature>> ListAsync(Guid documentId)
    {
        return await (await _repository.GetAllAsync())
            .Where(s => s.DocumentId == documentId)
            .OrderByDescending(s => s.SignedAt)
            .ToListAsync();
    }

    public async Task<bool> VerifyAsync(Guid signatureId)
    {
        var signature = await GetAsync(signatureId);
        if (signature == null)
        {
            return false;
        }

        var version = await _versionManager.GetAsync(signature.VersionId);

        return version != null && version.Checksum == signature.SignedHash;
    }

    public async Task<DmsSignature> GetAsync(Guid signatureId)
    {
        return await _repository.FirstOrDefaultAsync(signatureId);
    }

    private static async Task<string> SaveSignatureImageAsync(string signatureImageBase64, Guid documentId, long signerId)
    {
        if (string.IsNullOrEmpty(signatureImageBase64))
        {
            return null;
        }

        try
        {
            var payload = signatureImageBase64;
            var commaIndex = payload.IndexOf(',');
            if (commaIndex > 0)
            {
                payload = payload.Substring(commaIndex + 1);
            }

            var bytes = Convert.FromBase64String(payload);
            var folder = Path.Combine(Path.GetTempPath(), "dms-signatures", documentId.ToString("N"));

            Directory.CreateDirectory(folder);

            var path = Path.Combine(folder, $"{signerId}_{Guid.NewGuid():N}.png");
            await File.WriteAllBytesAsync(path, bytes);

            return path;
        }
        catch
        {
            return null;
        }
    }
}
