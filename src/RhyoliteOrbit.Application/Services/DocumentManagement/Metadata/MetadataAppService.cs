using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Dms.Acl;
using RhyoliteOrbit.DomainServices.Dms.Audit;
using RhyoliteOrbit.DomainServices.Dms.Documents;
using RhyoliteOrbit.DomainServices.Dms.Identifiers;
using RhyoliteOrbit.DomainServices.Dms.Metadata;
using RhyoliteOrbit.DomainServices.Dms.Tags;
using RhyoliteOrbit.Models.Dms;
using RhyoliteOrbit.Services.DocumentManagement.Dto;

namespace RhyoliteOrbit.Services.DocumentManagement.Metadata;

/// <summary>Tags, document templates, custom attributes and identifiers.</summary>
[AbpAuthorize]
public class MetadataAppService: DmsAppServiceBase
{
    private readonly IDmsTagManager _tags;
    private readonly IDocumentMetadataManager _metadata;
    private readonly IDocumentIdentifierManager _identifiers;
    private readonly IRepository<DmsCustomAttributes, Guid> _attributeRepository;
    private readonly IMapper _mapper;

    public MetadataAppService(
        IDmsTagManager tags,
        IDocumentMetadataManager metadata,
        IDocumentIdentifierManager identifiers,
        IRepository<DmsCustomAttributes, Guid> attributeRepository,
        IAclManager aclManager,
        IDocumentManager documentManager,
        DomainServices.Dms.Folders.IFolderManager folderManager,
        IAuditTrail auditTrail,
        IRepository<Authorization.Users.UserRole, long> userRoleRepository,
        IRepository<Authorization.Roles.Role, int> roleRepository,
        IMapper mapper)
        : base(aclManager, documentManager, folderManager, auditTrail, userRoleRepository, roleRepository)
    {
        _tags = tags;
        _metadata = metadata;
        _identifiers = identifiers;
        _attributeRepository = attributeRepository;
        _mapper = mapper;
    }

    // --------------------------------------------------------------------- tags

    /// <summary>Lists the tags of the tenant.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<ListResultDto<TagDto>> GetTags(string query = null)
    {
        var tags = await _tags.ListAsync(query);
        return new ListResultDto<TagDto>(_mapper.Map<List<TagDto>>(tags));
    }

    /// <summary>Creates a tag.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageTags)]
    [HttpPost]
    public async Task<TagDto> CreateTag(CreateTagDto input)
    {
        var tag = new DmsTag
        {
            TenantId = CurrentTenantId,
            Name = input.Name,
            ColorCode = input.ColorCode,
            Description = input.Description,
            CreatorUserId = CurrentUserId
        };

        var created = await _tags.CreateAsync(tag);
        return _mapper.Map<TagDto>(created);
    }

    /// <summary>Updates a tag.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageTags)]
    [HttpPut]
    public async Task<TagDto> UpdateTag(UpdateTagDto input)
    {
        var tag = new DmsTag
        {
            Id = input.Id,
            TenantId = CurrentTenantId,
            Name = input.Name,
            ColorCode = input.ColorCode,
            Description = input.Description,
            LastModifierUserId = CurrentUserId
        };

        var updated = await _tags.UpdateAsync(tag);
        return _mapper.Map<TagDto>(updated);
    }

    /// <summary>Deletes a tag.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageTags)]
    [HttpDelete]
    public async Task DeleteTag(Guid id)
    {
        await _tags.DeleteAsync(id);
    }

    /// <summary>Gets the tag cloud (most used tags first).</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<object> GetTagCloud(int maxTags = 50)
    {
        return await _tags.GetTagCloudAsync(maxTags);
    }

    /// <summary>Assigns tags to a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_UpdateDocument)]
    [HttpPost]
    public async Task AssignTags(DocumentTagsInput input)
    {
        await EnsureDocumentPermissionAsync(input.Id, RhyoliteOrbitConsts.DmsPermission.Edit);
        await _tags.AssignAsync(input.Id, input.TagIds ?? new List<Guid>());
    }

    /// <summary>Lists the tags of a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<ListResultDto<TagDto>> GetDocumentTags(Guid documentId)
    {
        await EnsureDocumentPermissionAsync(documentId, RhyoliteOrbitConsts.DmsPermission.View);
        var tags = await _tags.GetDocumentTagsAsync(documentId);
        return new ListResultDto<TagDto>(_mapper.Map<List<TagDto>>(tags));
    }

    // ------------------------------------------------------------------ templates

    /// <summary>Lists the document templates.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageTemplates)]
    [HttpGet]
    public async Task<ListResultDto<DocumentTemplateDto>> GetTemplates(bool onlyActive = true)
    {
        var templates = await _metadata.ListTemplatesAsync(onlyActive);
        return new ListResultDto<DocumentTemplateDto>(_mapper.Map<List<DocumentTemplateDto>>(templates));
    }

    /// <summary>Gets a document template.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageTemplates)]
    [HttpGet]
    public async Task<DocumentTemplateDto> GetTemplate(Guid id)
    {
        var template = await _metadata.GetTemplateAsync(id);
        return _mapper.Map<DocumentTemplateDto>(template);
    }

    /// <summary>Creates a document template.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageTemplates)]
    [HttpPost]
    public async Task<DocumentTemplateDto> CreateTemplate(CreateDocumentTemplateDto input)
    {
        var template = new DmsDocumentTemplate
        {
            TenantId = CurrentTenantId,
            Name = input.Name,
            Description = input.Description,
            IsActive = input.IsActive,
            AllowedFileTypes = input.AllowedFileTypes ?? new List<string>(),
            WorkflowTemplateId = input.WorkflowTemplateId,
            RetentionPolicyId = input.RetentionPolicyId,
            VersionScheme = input.VersionScheme,
            RequiresApproval = input.RequiresApproval,
            CreatorUserId = CurrentUserId
        };

        var created = await _metadata.CreateTemplateAsync(template);
        return _mapper.Map<DocumentTemplateDto>(created);
    }

    /// <summary>Updates a document template.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageTemplates)]
    [HttpPut]
    public async Task<DocumentTemplateDto> UpdateTemplate(UpdateDocumentTemplateDto input)
    {
        var template = new DmsDocumentTemplate
        {
            Id = input.Id,
            TenantId = CurrentTenantId,
            Name = input.Name,
            Description = input.Description,
            IsActive = input.IsActive,
            AllowedFileTypes = input.AllowedFileTypes ?? new List<string>(),
            WorkflowTemplateId = input.WorkflowTemplateId,
            RetentionPolicyId = input.RetentionPolicyId,
            VersionScheme = input.VersionScheme,
            RequiresApproval = input.RequiresApproval,
            LastModifierUserId = CurrentUserId
        };

        var updated = await _metadata.UpdateTemplateAsync(template);
        return _mapper.Map<DocumentTemplateDto>(updated);
    }

    /// <summary>Deletes a document template.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageTemplates)]
    [HttpDelete]
    public async Task DeleteTemplate(Guid id)
    {
        await _metadata.DeleteTemplateAsync(id);
    }

    // ---------------------------------------------------------------- attributes

    /// <summary>Lists the attributes of a template.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageTemplates)]
    [HttpGet]
    public async Task<ListResultDto<CustomAttributeDto>> GetAttributes(Guid templateId)
    {
        var attributes = await _metadata.ListAttributesAsync(templateId);
        return new ListResultDto<CustomAttributeDto>(_mapper.Map<List<CustomAttributeDto>>(attributes));
    }

    /// <summary>Creates a custom attribute.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageTemplates)]
    [HttpPost]
    public async Task<CustomAttributeDto> CreateAttribute(CreateCustomAttributeDto input)
    {
        var attribute = new DmsCustomAttributes
        {
            TenantId = CurrentTenantId,
            TemplateId = input.TemplateId,
            AttributeName = input.AttributeName,
            DataType = input.DataType,
            PossibleValues = input.PossibleValues ?? new List<string>(),
            DefaultValue = input.DefaultValue,
            OrderIndex = input.OrderIndex,
            IsRequired = input.IsRequired,
            IsSearchable = input.IsSearchable,
            IsSensitive = input.IsSensitive,
            ValidationRegex = input.ValidationRegex,
            CreatorUserId = CurrentUserId
        };

        var created = await _metadata.CreateAttributeAsync(attribute);
        return _mapper.Map<CustomAttributeDto>(created);
    }

    /// <summary>Updates a custom attribute.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageTemplates)]
    [HttpPut]
    public async Task<CustomAttributeDto> UpdateAttribute(UpdateCustomAttributeDto input)
    {
        var attribute = new DmsCustomAttributes
        {
            Id = input.Id,
            AttributeName = input.AttributeName,
            DataType = input.DataType,
            PossibleValues = input.PossibleValues ?? new List<string>(),
            DefaultValue = input.DefaultValue,
            OrderIndex = input.OrderIndex,
            IsRequired = input.IsRequired,
            IsSearchable = input.IsSearchable,
            IsSensitive = input.IsSensitive,
            ValidationRegex = input.ValidationRegex,
            LastModifierUserId = CurrentUserId
        };

        var updated = await _metadata.UpdateAttributeAsync(attribute);
        return _mapper.Map<CustomAttributeDto>(updated);
    }

    /// <summary>Deletes a custom attribute.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageTemplates)]
    [HttpDelete]
    public async Task DeleteAttribute(Guid id)
    {
        await _metadata.DeleteAttributeAsync(id);
    }

    /// <summary>Gets the metadata values of a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<ListResultDto<MetadataValueDto>> GetDocumentMetadata(Guid documentId)
    {
        await EnsureDocumentPermissionAsync(documentId, RhyoliteOrbitConsts.DmsPermission.View);

        var values = await _metadata.GetValuesAsync(documentId);
        if (values.Count == 0)
        {
            return new ListResultDto<MetadataValueDto>(new List<MetadataValueDto>());
        }

        var attributeIds = values.Select(v => v.AttributeId).Distinct().ToList();

        var names = await (await _attributeRepository.GetAllAsync())
            .Where(a => attributeIds.Contains(a.Id))
            .Select(a => new { a.Id, a.AttributeName })
            .ToListAsync();

        var lookup = names.ToDictionary(x => x.Id, x => x.AttributeName);

        return new ListResultDto<MetadataValueDto>(values.Select(v => new MetadataValueDto
        {
            AttributeId = v.AttributeId,
            AttributeName = lookup.TryGetValue(v.AttributeId, out var name) ? name : null,
            Value = v.Value
        }).ToList());
    }

    /// <summary>Saves the metadata values of a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_UpdateDocument)]
    [HttpPost]
    public async Task SaveDocumentMetadata(DocumentMetadataInput input)
    {
        await EnsureDocumentPermissionAsync(input.Id, RhyoliteOrbitConsts.DmsPermission.Edit);
        await DocumentManager.SaveMetadataAsync(input.Id, input.Values ?? new Dictionary<Guid, string>(), CurrentUserId);
    }

    // --------------------------------------------------------------- identifiers

    /// <summary>Lists the custom identifiers.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageIdentifiers)]
    [HttpGet]
    public async Task<ListResultDto<CustomIdentifierDto>> GetIdentifiers()
    {
        var identifiers = await _identifiers.ListAsync();
        return new ListResultDto<CustomIdentifierDto>(_mapper.Map<List<CustomIdentifierDto>>(identifiers));
    }

    /// <summary>Creates a custom identifier.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageIdentifiers)]
    [HttpPost]
    public async Task<CustomIdentifierDto> CreateIdentifier(CreateCustomIdentifierDto input)
    {
        var identifier = new DmsCustomIdentifier
        {
            TenantId = CurrentTenantId,
            Name = input.Name,
            Prefix = input.Prefix,
            Suffix = input.Suffix,
            IncludeYear = input.IncludeYear,
            IncludeMonth = input.IncludeMonth,
            PaddingLength = input.PaddingLength,
            TemplateId = input.TemplateId,
            IsActive = input.IsActive,
            ResetPolicy = input.ResetPolicy,
            CreatorUserId = CurrentUserId
        };

        var created = await _identifiers.CreateAsync(identifier);
        return _mapper.Map<CustomIdentifierDto>(created);
    }

    /// <summary>Updates a custom identifier.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageIdentifiers)]
    [HttpPut]
    public async Task<CustomIdentifierDto> UpdateIdentifier(UpdateCustomIdentifierDto input)
    {
        var identifier = new DmsCustomIdentifier
        {
            Id = input.Id,
            TenantId = CurrentTenantId,
            Name = input.Name,
            Prefix = input.Prefix,
            Suffix = input.Suffix,
            IncludeYear = input.IncludeYear,
            IncludeMonth = input.IncludeMonth,
            PaddingLength = input.PaddingLength,
            TemplateId = input.TemplateId,
            IsActive = input.IsActive,
            ResetPolicy = input.ResetPolicy,
            LastModifierUserId = CurrentUserId
        };

        var updated = await _identifiers.UpdateAsync(identifier);
        return _mapper.Map<CustomIdentifierDto>(updated);
    }

    /// <summary>Deletes a custom identifier.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageIdentifiers)]
    [HttpDelete]
    public async Task DeleteIdentifier(Guid id)
    {
        await _identifiers.DeleteAsync(id);
    }

    /// <summary>Generates the next identifier value (consumes the sequence).</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageIdentifiers)]
    [HttpGet]
    public async Task<string> NextIdentifier(Guid id)
    {
        return await _identifiers.GenerateNextAsync(id);
    }

    /// <summary>Previews the next identifier value without consuming the sequence.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageIdentifiers)]
    [HttpGet]
    public async Task<string> PreviewIdentifier(Guid id)
    {
        return await _identifiers.PreviewNextAsync(id);
    }
}
