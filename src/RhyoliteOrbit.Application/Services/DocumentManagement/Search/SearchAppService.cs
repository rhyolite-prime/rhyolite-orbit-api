using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Dms;
using RhyoliteOrbit.DomainServices.Dms.Acl;
using RhyoliteOrbit.DomainServices.Dms.Audit;
using RhyoliteOrbit.DomainServices.Dms.Documents;
using RhyoliteOrbit.DomainServices.Dms.Search;
using RhyoliteOrbit.Services.DocumentManagement.Dto;

namespace RhyoliteOrbit.Services.DocumentManagement.Search;

[AbpAuthorize]
public class SearchAppService: DmsAppServiceBase
{
    private readonly ISearchProvider _searchProvider;
    private readonly IMapper _mapper;

    public SearchAppService(
        ISearchProvider searchProvider,
        IAclManager aclManager,
        IDocumentManager documentManager,
        DomainServices.Dms.Folders.IFolderManager folderManager,
        IAuditTrail auditTrail,
        Abp.Domain.Repositories.IRepository<Authorization.Users.UserRole, long> userRoleRepository,
        Abp.Domain.Repositories.IRepository<Authorization.Roles.Role, int> roleRepository,
        IMapper mapper)
        : base(aclManager, documentManager, folderManager, auditTrail, userRoleRepository, roleRepository)
    {
        _searchProvider = searchProvider;
        _mapper = mapper;
    }

    /// <summary>Full text search over document content and metadata.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<DmsSearchResultDto> Search(DmsSearchInput input)
    {
        var request = new DmsSearchRequest
        {
            Query = input.Query,
            FolderId = input.FolderId,
            CollectionId = input.CollectionId,
            TagIds = input.TagIds,
            Extension = input.Extension,
            MimeType = input.MimeType,
            CustomId = input.CustomId,
            CreatedFrom = input.CreatedFrom,
            CreatedTo = input.CreatedTo,
            CreatedBy = input.CreatedBy,
            MetadataFilters = input.MetadataFilters,
            SortOrder = input.SortOrder,
            IncludeDeleted = input.IncludeDeleted,
            PageNo = input.MaxResultCount == 0 ? 1 : (input.SkipCount / Math.Max(input.MaxResultCount, 1)) + 1,
            PageSize = input.MaxResultCount == 0 ? 20 : input.MaxResultCount
        };

        var response = await _searchProvider.SearchAsync(request);

        return new DmsSearchResultDto
        {
            PageNo = response.PageNo,
            PageSize = response.PageSize,
            TotalCount = response.TotalCount,
            TotalPages = response.TotalPages,
            Data = _mapper.Map<List<DmsSearchHitDto>>(response.Data ?? new List<DmsSearchHit>()),
            Facets = response.Facets ?? new Dictionary<string, long>(),
            ElapsedMs = response.ElapsedMs
        };
    }

    /// <summary>Autocomplete suggestions for the search box.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<ListResultDto<DmsSuggestionDto>> Suggest(string prefix, int maxResults = 10)
    {
        var suggestions = await _searchProvider.SuggestAsync(prefix, maxResults);
        return new ListResultDto<DmsSuggestionDto>(_mapper.Map<List<DmsSuggestionDto>>(suggestions));
    }

    /// <summary>Re-indexes a document on demand.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageSettings)]
    [HttpPost]
    public async Task Index(Guid documentId)
    {
        await _searchProvider.IndexDocumentAsync(documentId);
    }

    /// <summary>Removes a document from the search index.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageSettings)]
    [HttpDelete]
    public async Task RemoveFromIndex(Guid documentId)
    {
        await _searchProvider.RemoveDocumentAsync(documentId);
    }
}
