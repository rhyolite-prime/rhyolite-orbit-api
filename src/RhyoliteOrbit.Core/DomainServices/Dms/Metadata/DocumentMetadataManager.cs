using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Dms;

namespace RhyoliteOrbit.DomainServices.Dms.Metadata;

public interface IDocumentMetadataManager
{
    // templates
    Task<DmsDocumentTemplate> CreateTemplateAsync(DmsDocumentTemplate template);

    Task<DmsDocumentTemplate> UpdateTemplateAsync(DmsDocumentTemplate template);

    Task DeleteTemplateAsync(Guid templateId);

    Task<List<DmsDocumentTemplate>> ListTemplatesAsync(bool onlyActive = false);

    Task<DmsDocumentTemplate> GetTemplateAsync(Guid templateId);

    // attributes
    Task<DmsCustomAttributes> CreateAttributeAsync(DmsCustomAttributes attribute);

    Task<DmsCustomAttributes> UpdateAttributeAsync(DmsCustomAttributes attribute);

    Task DeleteAttributeAsync(Guid attributeId);

    Task<List<DmsCustomAttributes>> ListAttributesAsync(Guid templateId);

    // values
    Task<List<DmsDocumentMetadataValues>> GetValuesAsync(Guid documentId);

    Task<Dictionary<Guid, string>> GetValueMapAsync(Guid documentId);

    Task ValidateRequiredAsync(Guid templateId, Dictionary<Guid, string> values);
}

public class DocumentMetadataManager: DomainService, IDocumentMetadataManager
{
    private readonly IRepository<DmsDocumentTemplate, Guid> _templateRepository;
    private readonly IRepository<DmsCustomAttributes, Guid> _attributeRepository;
    private readonly IRepository<DmsDocumentMetadataValues, Guid> _valueRepository;
    private readonly IAbpSession _abpSession;

    public DocumentMetadataManager(
        IRepository<DmsDocumentTemplate, Guid> templateRepository,
        IRepository<DmsCustomAttributes, Guid> attributeRepository,
        IRepository<DmsDocumentMetadataValues, Guid> valueRepository,
        IAbpSession abpSession)
    {
        _templateRepository = templateRepository;
        _attributeRepository = attributeRepository;
        _valueRepository = valueRepository;
        _abpSession = abpSession;
    }

    public async Task<DmsDocumentTemplate> CreateTemplateAsync(DmsDocumentTemplate template)
    {
        var tenantId = _abpSession.TenantId ?? 0;

        var exists = await (await _templateRepository.GetAllAsync())
            .AnyAsync(t => t.Name.ToLower() == template.Name.Trim().ToLower());

        if (exists)
        {
            throw new UserFriendlyException("A document template with the same name already exists.");
        }

        template.TenantId = tenantId;
        template.IsActive = true;
        template.VersionScheme = (int)RhyoliteOrbitConsts.DocumentVersionScheme.Major;

        return await _templateRepository.InsertAsync(template);
    }

    public async Task<DmsDocumentTemplate> UpdateTemplateAsync(DmsDocumentTemplate template)
    {
        var existing = await GetTemplateAsync(template.Id);
        if (existing == null)
        {
            throw new UserFriendlyException("The document template was not found.");
        }

        existing.Name = template.Name;
        existing.Description = template.Description;
        existing.IsActive = template.IsActive;
        existing.AllowedFileTypes = template.AllowedFileTypes ?? existing.AllowedFileTypes;
        existing.WorkflowTemplateId = template.WorkflowTemplateId;
        existing.RetentionPolicyId = template.RetentionPolicyId;
        existing.VersionScheme = template.VersionScheme;
        existing.RequiresApproval = template.RequiresApproval;

        return await _templateRepository.UpdateAsync(existing);
    }

    public async Task DeleteTemplateAsync(Guid templateId)
    {
        var attributes = await ListAttributesAsync(templateId);

        foreach (var attribute in attributes)
        {
            await _attributeRepository.HardDeleteAsync(attribute);
        }

        await _templateRepository.HardDeleteAsync(templateId);
    }

    public async Task<List<DmsDocumentTemplate>> ListTemplatesAsync(bool onlyActive = false)
    {
        var query = await _templateRepository.GetAllAsync();

        if (onlyActive)
        {
            query = query.Where(t => t.IsActive);
        }

        return await query.OrderBy(t => t.Name).ToListAsync();
    }

    public async Task<DmsDocumentTemplate> GetTemplateAsync(Guid templateId)
    {
        return await _templateRepository.FirstOrDefaultAsync(templateId);
    }

    public async Task<DmsCustomAttributes> CreateAttributeAsync(DmsCustomAttributes attribute)
    {
        var template = await GetTemplateAsync(attribute.TemplateId);
        if (template == null)
        {
            throw new UserFriendlyException("The document template was not found.");
        }

        attribute.TemplateName = template.Name;
        attribute.TenantId = _abpSession.TenantId ?? 0;

        return await _attributeRepository.InsertAsync(attribute);
    }

    public async Task<DmsCustomAttributes> UpdateAttributeAsync(DmsCustomAttributes attribute)
    {
        var existing = await _attributeRepository.FirstOrDefaultAsync(attribute.Id);
        if (existing == null)
        {
            throw new UserFriendlyException("The attribute was not found.");
        }

        existing.AttributeName = attribute.AttributeName;
        existing.DataType = attribute.DataType;
        existing.PossibleValues = attribute.PossibleValues;
        existing.DefaultValue = attribute.DefaultValue;
        existing.OrderIndex = attribute.OrderIndex;
        existing.IsRequired = attribute.IsRequired;
        existing.IsSearchable = attribute.IsSearchable;
        existing.IsSensitive = attribute.IsSensitive;
        existing.ValidationRegex = attribute.ValidationRegex;

        return await _attributeRepository.UpdateAsync(existing);
    }

    public async Task DeleteAttributeAsync(Guid attributeId)
    {
        var values = await (await _valueRepository.GetAllAsync())
            .Where(v => v.AttributeId == attributeId)
            .ToListAsync();

        foreach (var value in values)
        {
            await _valueRepository.HardDeleteAsync(value);
        }

        await _attributeRepository.HardDeleteAsync(attributeId);
    }

    public async Task<List<DmsCustomAttributes>> ListAttributesAsync(Guid templateId)
    {
        return await (await _attributeRepository.GetAllAsync())
            .Where(a => a.TemplateId == templateId)
            .OrderBy(a => a.OrderIndex)
            .ToListAsync();
    }

    public async Task<List<DmsDocumentMetadataValues>> GetValuesAsync(Guid documentId)
    {
        return await (await _valueRepository.GetAllAsync())
            .Where(v => v.DocumentId == documentId)
            .ToListAsync();
    }

    public async Task<Dictionary<Guid, string>> GetValueMapAsync(Guid documentId)
    {
        var values = await GetValuesAsync(documentId);

        return values.GroupBy(v => v.AttributeId).ToDictionary(g => g.Key, g => g.Last().Value);
    }

    public async Task ValidateRequiredAsync(Guid templateId, Dictionary<Guid, string> values)
    {
        var attributes = await ListAttributesAsync(templateId);
        var provided = values ?? new Dictionary<Guid, string>();

        foreach (var attribute in attributes.Where(a => a.IsRequired))
        {
            var hasValue = provided.TryGetValue(attribute.Id, out var value) && !string.IsNullOrWhiteSpace(value);

            if (!hasValue && string.IsNullOrWhiteSpace(attribute.DefaultValue))
            {
                throw new UserFriendlyException($"The attribute '{attribute.AttributeName}' is required.");
            }
        }
    }
}
