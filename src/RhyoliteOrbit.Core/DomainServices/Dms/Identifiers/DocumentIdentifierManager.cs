using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Dms;

namespace RhyoliteOrbit.DomainServices.Dms.Identifiers;

public interface IDocumentIdentifierManager
{
    Task<DmsCustomIdentifier> CreateAsync(DmsCustomIdentifier identifier);

    Task<DmsCustomIdentifier> UpdateAsync(DmsCustomIdentifier identifier);

    Task DeleteAsync(Guid identifierId);

    Task<List<DmsCustomIdentifier>> ListAsync();

    Task<string> GenerateNextAsync(Guid identifierId);

    Task<string> PreviewNextAsync(Guid identifierId);
}

/// <summary>
/// Generates sequential custom identifiers such as INV-2026-000123.
/// </summary>
public class DocumentIdentifierManager: DomainService, IDocumentIdentifierManager
{
    private readonly IRepository<DmsCustomIdentifier, Guid> _repository;
    private readonly IAbpSession _abpSession;

    public DocumentIdentifierManager(IRepository<DmsCustomIdentifier, Guid> repository, IAbpSession abpSession)
    {
        _repository = repository;
        _abpSession = abpSession;
    }

    public async Task<DmsCustomIdentifier> CreateAsync(DmsCustomIdentifier identifier)
    {
        identifier.TenantId = _abpSession.TenantId ?? 0;
        identifier.IsActive = true;

        if (identifier.PaddingLength <= 0)
        {
            identifier.PaddingLength = 4;
        }

        return await _repository.InsertAsync(identifier);
    }

    public async Task<DmsCustomIdentifier> UpdateAsync(DmsCustomIdentifier identifier)
    {
        var existing = await _repository.FirstOrDefaultAsync(identifier.Id);
        if (existing == null)
        {
            throw new UserFriendlyException("The custom identifier was not found.");
        }

        existing.Name = identifier.Name;
        existing.Prefix = identifier.Prefix;
        existing.Suffix = identifier.Suffix;
        existing.IncludeYear = identifier.IncludeYear;
        existing.IncludeMonth = identifier.IncludeMonth;
        existing.PaddingLength = identifier.PaddingLength > 0 ? identifier.PaddingLength : existing.PaddingLength;
        existing.IsActive = identifier.IsActive;
        existing.ResetPolicy = identifier.ResetPolicy;
        existing.TemplateId = identifier.TemplateId;
        existing.TemplateName = identifier.TemplateName;

        return await _repository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(Guid identifierId)
    {
        await _repository.HardDeleteAsync(identifierId);
    }

    public async Task<List<DmsCustomIdentifier>> ListAsync()
    {
        return await (await _repository.GetAllAsync())
            .OrderBy(i => i.Name)
            .ToListAsync();
    }

    public async Task<string> GenerateNextAsync(Guid identifierId)
    {
        var identifier = await _repository.FirstOrDefaultAsync(identifierId);
        if (identifier == null || !identifier.IsActive)
        {
            throw new UserFriendlyException("The custom identifier is not available.");
        }

        ResetIfRequired(identifier);

        identifier.CurrentSequence += 1;
        identifier.LastGeneratedAt = DateTime.UtcNow;

        if (identifier.PaddingLength > 0 && identifier.CurrentSequence.ToString().Length > identifier.PaddingLength)
        {
            throw new UserFriendlyException(
                "DmsIdentifierSequenceExhausted",
                "The custom identifier sequence has been exhausted.");
        }

        await _repository.UpdateAsync(identifier);

        return Format(identifier);
    }

    public async Task<string> PreviewNextAsync(Guid identifierId)
    {
        var identifier = await _repository.FirstOrDefaultAsync(identifierId);
        if (identifier == null)
        {
            return string.Empty;
        }

        var preview = new DmsCustomIdentifier
        {
            Prefix = identifier.Prefix,
            Suffix = identifier.Suffix,
            IncludeYear = identifier.IncludeYear,
            IncludeMonth = identifier.IncludeMonth,
            PaddingLength = identifier.PaddingLength,
            CurrentSequence = identifier.CurrentSequence + 1,
            ResetPolicy = identifier.ResetPolicy,
            LastGeneratedAt = identifier.LastGeneratedAt
        };

        return Format(preview);
    }

    private void ResetIfRequired(DmsCustomIdentifier identifier)
    {
        var now = DateTime.UtcNow;

        switch ((RhyoliteOrbitConsts.SequenceResetPolicy)identifier.ResetPolicy)
        {
            case RhyoliteOrbitConsts.SequenceResetPolicy.Yearly when
                identifier.LastGeneratedAt.HasValue && identifier.LastGeneratedAt.Value.Year != now.Year:
                identifier.CurrentSequence = 0;
                break;
            case RhyoliteOrbitConsts.SequenceResetPolicy.Monthly when
                identifier.LastGeneratedAt.HasValue &&
                (identifier.LastGeneratedAt.Value.Year != now.Year || identifier.LastGeneratedAt.Value.Month != now.Month):
                identifier.CurrentSequence = 0;
                break;
        }
    }

    private static string Format(DmsCustomIdentifier identifier)
    {
        var builder = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(identifier.Prefix))
        {
            builder.Append(identifier.Prefix.Trim());
        }

        if (identifier.IncludeYear)
        {
            AppendSeparator(builder);
            builder.Append(DateTime.UtcNow.Year);
        }

        if (identifier.IncludeMonth)
        {
            AppendSeparator(builder);
            builder.Append(DateTime.UtcNow.ToString("MM"));
        }

        AppendSeparator(builder);

        var padding = identifier.PaddingLength > 0 ? identifier.PaddingLength : 4;
        builder.Append(identifier.CurrentSequence.ToString().PadLeft(padding, '0'));

        if (!string.IsNullOrWhiteSpace(identifier.Suffix))
        {
            builder.Append(identifier.Suffix.Trim());
        }

        return builder.ToString();
    }

    private static void AppendSeparator(StringBuilder builder)
    {
        if (builder.Length > 0)
        {
            builder.Append('-');
        }
    }
}
