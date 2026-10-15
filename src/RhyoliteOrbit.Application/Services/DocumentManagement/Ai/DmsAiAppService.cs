using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Dms.Acl;
using RhyoliteOrbit.DomainServices.Dms.Ai;
using RhyoliteOrbit.DomainServices.Dms.Audit;
using RhyoliteOrbit.DomainServices.Dms.Documents;
using RhyoliteOrbit.DomainServices.Dms.Extraction;
using RhyoliteOrbit.DomainServices.Dms.Security;
using RhyoliteOrbit.DomainServices.Dms.Versions;
using RhyoliteOrbit.Models.Dms;
using RhyoliteOrbit.Services.DocumentManagement.Dto;

namespace RhyoliteOrbit.Services.DocumentManagement.Ai;

/// <summary>
/// AI helpers: classification of uploaded documents, tag suggestions and
/// retrieval augmented question answering over the indexed content.
/// </summary>
[AbpAuthorize]
public class DmsAiAppService: DmsAppServiceBase
{
    private readonly IAiDocumentService _ai;
    private readonly IRagService _rag;
    private readonly IEmbeddingService _embeddings;
    private readonly IVersionManager _versions;
    private readonly ITextExtractorFactory _extractors;
    private readonly IPiiDetector _piiDetector;
    private readonly IDmsSettingsManager _settings;
    private readonly IMapper _mapper;

    public DmsAiAppService(
        IAiDocumentService ai,
        IRagService rag,
        IEmbeddingService embeddings,
        IVersionManager versions,
        ITextExtractorFactory extractors,
        IPiiDetector piiDetector,
        DomainServices.Dms.Settings.IDmsSettingsManager settings,
        IAclManager aclManager,
        IDocumentManager documentManager,
        DomainServices.Dms.Folders.IFolderManager folderManager,
        IAuditTrail auditTrail,
        IRepository<Authorization.Users.UserRole, long> userRoleRepository,
        IRepository<Authorization.Roles.Role, int> roleRepository,
        IMapper mapper)
        : base(aclManager, documentManager, folderManager, auditTrail, userRoleRepository, roleRepository)
    {
        _ai = ai;
        _rag = rag;
        _embeddings = embeddings;
        _versions = versions;
        _extractors = extractors;
        _piiDetector = piiDetector;
        _settings = settings;
        _mapper = mapper;
    }

    /// <summary>Classifies a document and suggests metadata for it.</summary>
    [AbpAuthorize(PermissionNames.DMS_UseAiFeatures)]
    [HttpGet]
    public async Task<DocumentClassificationDto> Classify(Guid documentId)
    {
        await EnsureDocumentPermissionAsync(documentId, RhyoliteOrbitConsts.DmsPermission.View);

        var settings = await _settings.GetAsync();
        if (settings != null && !settings.AiEnabled)
        {
            throw new UserFriendlyException("DmsAiDisabled", "AI features are disabled for this tenant.");
        }

        var document = await DocumentManager.GetAsync(documentId);
        var version = document.CurrentVersionId.HasValue
            ? await _versions.GetAsync(document.CurrentVersionId.Value)
            : null;

        var text = version?.ExtractedText;

        if (string.IsNullOrWhiteSpace(text))
        {
            await using var content = await DocumentManager.OpenReadAsync(documentId, version?.Id);
            text = await _extractors.ExtractAsync(content, version?.Extension ?? document.Extension);
        }

        var classification = await _ai.ClassifyAsync(document.FileName, document.Extension, text);

        return new DocumentClassificationDto
        {
            SuggestedDocumentType = classification.DocumentType,
            Summary = classification.Summary,
            DetectedLanguage = classification.Language,
            SuggestedTags = classification.SuggestedTags ?? new List<string>()
        };
    }

    /// <summary>Suggests tags for a piece of text.</summary>
    [AbpAuthorize(PermissionNames.DMS_UseAiFeatures)]
    [HttpPost]
    public async Task<ListResultDto<string>> SuggestTags(string text, int maxTags = 10)
    {
        var tags = await _ai.SuggestTagsAsync(text, maxTags);
        return new ListResultDto<string>(tags);
    }

    /// <summary>Asks a question answered from the indexed document content.</summary>
    [AbpAuthorize(PermissionNames.DMS_UseAiFeatures)]
    [HttpPost]
    public async Task<object> Ask(AiQuestionDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Question))
        {
            throw new UserFriendlyException("DmsEmptyQuestion", "A question is required.");
        }

        return await _rag.AskAsync(input.Question, input.MaxChunks <= 0 ? 5 : input.MaxChunks);
    }

    /// <summary>Scans a document for personally identifiable information.</summary>
    [AbpAuthorize(PermissionNames.DMS_UseAiFeatures)]
    [HttpGet]
    public async Task<object> DetectPii(Guid documentId)
    {
        await EnsureDocumentPermissionAsync(documentId, RhyoliteOrbitConsts.DmsPermission.View);

        var document = await DocumentManager.GetAsync(documentId);
        var version = document.CurrentVersionId.HasValue
            ? await _versions.GetAsync(document.CurrentVersionId.Value)
            : null;

        var text = version?.ExtractedText;

        if (string.IsNullOrWhiteSpace(text))
        {
            await using var content = await DocumentManager.OpenReadAsync(documentId, version?.Id);
            text = await _extractors.ExtractAsync(content, version?.Extension ?? document.Extension);
        }

        var matches = await _piiDetector.DetectAsync(text);

        return new
        {
            documentId,
            matchCount = matches.Count,
            matches = matches.Select(m => new { m.Type, m.Value, m.Start, m.Length })
        };
    }

    /// <summary>Generates the embedding vector of a document version.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageSettings)]
    [HttpPost]
    public async Task<object> Embed(Guid documentId, Guid? versionId = null)
    {
        await EnsureDocumentPermissionAsync(documentId, RhyoliteOrbitConsts.DmsPermission.Edit);

        var version = versionId.HasValue
            ? await _versions.GetAsync(versionId.Value)
            : await _versions.GetCurrentAsync(documentId);

        if (version == null)
        {
            throw new UserFriendlyException("DmsNoVersion", "The document has no version to embed.");
        }

        var chunks = await _embeddings.EmbedAsync(version.ExtractedText ?? string.Empty);

        return new
        {
            model = _embeddings.ModelName,
            chunkCount = chunks.Count
        };
    }
}
