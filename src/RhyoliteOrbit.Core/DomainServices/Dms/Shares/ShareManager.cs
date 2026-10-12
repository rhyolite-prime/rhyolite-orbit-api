using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Dms;

namespace RhyoliteOrbit.DomainServices.Dms.Shares;

public interface IShareManager
{
    Task<DmsShare> CreateAsync(DmsShare share);

    Task RevokeAsync(Guid shareId, long revokedBy);

    Task<List<DmsShare>> ListAsync(Guid? documentId = null, Guid? folderId = null);

    /// <summary>Validates a public token and returns the share (throws when invalid).</summary>
    Task<DmsShare> ResolveTokenAsync(string token, string password = null);

    Task RegisterDownloadAsync(Guid shareId);

    Task<bool> IsAccessibleAsync(DmsShare share);
}

public class ShareManager: DomainService, IShareManager
{
    private readonly IRepository<DmsShare, Guid> _repository;
    private readonly IAbpSession _abpSession;

    public ShareManager(IRepository<DmsShare, Guid> repository, IAbpSession abpSession)
    {
        _repository = repository;
        _abpSession = abpSession;
    }

    public async Task<DmsShare> CreateAsync(DmsShare share)
    {
        share.TenantId = _abpSession.TenantId ?? 0;
        share.DownloadCount = 0;
        share.IsRevoked = false;
        share.CreatedById = _abpSession.UserId ?? 0;

        // The caller may pre-compute the hashed token (so it can return the raw
        // token to the user once); only generate one when it is missing.
        if (share.ShareType == (int)RhyoliteOrbitConsts.ShareType.PublicLink
            && string.IsNullOrEmpty(share.Token))
        {
            share.Token = HashToken(GenerateToken());
        }

        return await _repository.InsertAsync(share);
    }

    public async Task RevokeAsync(Guid shareId, long revokedBy)
    {
        var share = await _repository.FirstOrDefaultAsync(shareId);
        if (share == null)
        {
            throw new UserFriendlyException("The share was not found.");
        }

        share.IsRevoked = true;
        await _repository.UpdateAsync(share);
    }

    public async Task<List<DmsShare>> ListAsync(Guid? documentId = null, Guid? folderId = null)
    {
        var query = await _repository.GetAllAsync();

        if (documentId.HasValue)
        {
            query = query.Where(s => s.DocumentId == documentId.Value);
        }

        if (folderId.HasValue)
        {
            query = query.Where(s => s.FolderId == folderId.Value);
        }

        return await query
            .OrderByDescending(s => s.CreationTime)
            .ToListAsync();
    }

    public async Task<DmsShare> ResolveTokenAsync(string token, string password = null)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new UserFriendlyException("A share token is required.");
        }

        var hashed = HashToken(token);

        var share = await (await _repository.GetAllAsync())
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Token == hashed);

        if (share == null)
        {
            throw new UserFriendlyException("The share link is not valid.");
        }

        if (share.IsRevoked)
        {
            throw new UserFriendlyException("DmsShareRevoked", "This share link has been revoked.");
        }

        if (share.ExpiresAt.HasValue && share.ExpiresAt.Value < DateTime.UtcNow)
        {
            throw new UserFriendlyException("DmsShareExpired", "This share link has expired.");
        }

        if (share.MaxDownloads.HasValue && share.DownloadCount >= share.MaxDownloads.Value)
        {
            throw new UserFriendlyException("DmsShareDownloadLimitReached", "This share link has reached its download limit.");
        }

        if (!string.IsNullOrEmpty(share.PasswordHash) &&
            !VerifyPassword(password, share.PasswordHash))
        {
            throw new UserFriendlyException("DmsInvalidSharePassword", "The share password is incorrect.");
        }

        share.LastAccessedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(share);

        return share;
    }

    public async Task RegisterDownloadAsync(Guid shareId)
    {
        var share = await _repository.FirstOrDefaultAsync(shareId);
        if (share == null)
        {
            return;
        }

        share.DownloadCount += 1;
        await _repository.UpdateAsync(share);
    }

    public Task<bool> IsAccessibleAsync(DmsShare share)
    {
        if (share == null || share.IsRevoked)
        {
            return Task.FromResult(false);
        }

        if (share.ExpiresAt.HasValue && share.ExpiresAt.Value < DateTime.UtcNow)
        {
            return Task.FromResult(false);
        }

        if (share.MaxDownloads.HasValue && share.DownloadCount >= share.MaxDownloads.Value)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    public static string GenerateToken()
    {
        var bytes = new byte[32];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);

        return Convert.ToBase64String(bytes)
            .Replace("+", string.Empty)
            .Replace("/", string.Empty)
            .Replace("=", string.Empty)
            .Substring(0, 32);
    }

    public static string HashToken(string token)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token ?? string.Empty));

        var builder = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }

    /// <summary>
    /// Hashes a share password with a random salt. The result is stored as
    /// <c>salt.hash</c> so the same password never produces the same value twice.
    /// </summary>
    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return null;
        }

        var salt = new byte[16];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        return $"{Convert.ToBase64String(salt)}.{ComputeSaltedHash(password, salt)}";
    }

    /// <summary>Verifies a password against a value produced by <see cref="HashPassword"/>.</summary>
    public static bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrEmpty(storedHash))
        {
            return string.IsNullOrEmpty(password);
        }

        var separator = storedHash.IndexOf('.');
        if (separator <= 0)
        {
            return false;
        }

        byte[] salt;

        try
        {
            salt = Convert.FromBase64String(storedHash.Substring(0, separator));
        }
        catch (FormatException)
        {
            return false;
        }

        var expected = storedHash.Substring(separator + 1);
        var actual = ComputeSaltedHash(password, salt);

        // constant time comparison to avoid leaking information through timing
        if (expected.Length != actual.Length)
        {
            return false;
        }

        var difference = 0;

        for (var i = 0; i < expected.Length; i++)
        {
            difference |= expected[i] ^ actual[i];
        }

        return difference == 0;
    }

    private static string ComputeSaltedHash(string password, byte[] salt)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes($"dms-share::{Convert.ToBase64String(salt)}::{password}"));

        var builder = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }
}
