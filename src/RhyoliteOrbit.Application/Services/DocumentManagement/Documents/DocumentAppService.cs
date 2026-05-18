using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.DocumentManagement.Documents;
using RhyoliteOrbit.Models.DocumentManagement;
using RhyoliteOrbit.Services.DocumentManagement.Documents.Dto;

namespace RhyoliteOrbit.Services.DocumentManagement.Documents;

public class DocumentAppService : RhyoliteOrbitAppServiceBase
{
    private readonly IDocumentManager _documentManager;
    private readonly IMapper _mapper;

    public DocumentAppService(IDocumentManager documentManager, IMapper mapper)
    {
        _documentManager = documentManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _documentManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CreateDocumentInput input)
    {
        var document = _mapper.Map<Document>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        document.TenantId = tenantInfo.Id;
        await _documentManager.Create(document);
    }

    [HttpPost]
    public async Task Update(UpdateDocumentInput input)
    {
        var document = _mapper.Map<Document>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        document.TenantId = tenantInfo.Id;
        await _documentManager.Update(document);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _documentManager.Delete(id);
    }
}
