using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Shared.MessageTemplates;
using RhyoliteOrbit.Models.Shared;
using RhyoliteOrbit.Services.Shared.MessageTemplates.Dto;

namespace RhyoliteOrbit.Services.Shared.MessageTemplates;

public class MessageTemplateAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IMessageTemplateManager _messageTemplateManager;
    private readonly IMapper _mapper;

    public MessageTemplateAppService(IMessageTemplateManager messageTemplateManager, IMapper mapper)
    {
        _messageTemplateManager = messageTemplateManager;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _messageTemplateManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CreateMessageTemplateInput input)
    {
        var obj = _mapper.Map<MessageTemplate>(input);
        await _messageTemplateManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateMessageTemplateInput input)
    {
        var obj = _mapper.Map<MessageTemplate>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _messageTemplateManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _messageTemplateManager.Delete(id);
    }
}