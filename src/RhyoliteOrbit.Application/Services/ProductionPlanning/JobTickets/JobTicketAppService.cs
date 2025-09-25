using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.ProductionPlanning.JobTickets;
using RhyoliteOrbit.Models.ProductionPlanning;
using RhyoliteOrbit.Services.ProductionPlanning.JobTickets.Dto;

namespace RhyoliteOrbit.Services.ProductionPlanning.JobTickets;

public class JobTicketAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IJobTicketManager _jobTicketManager;
    private readonly IMapper _mapper;

    public JobTicketAppService(IJobTicketManager jobTicketManager, IMapper mapper)
    {
        _jobTicketManager = jobTicketManager;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _jobTicketManager.ListAll(pageNo, pageSize, query);
    }

    [HttpPost]
    public async Task Create(CreateJobTicketInput input)
    {
        var obj = _mapper.Map<JobTicket>(input);
        await _jobTicketManager.Create(obj);
    }

    [HttpPost]
    public async Task Update(UpdateJobTicketInput input)
    {
        var obj = _mapper.Map<JobTicket>(input);
        var tenantInfo = await GetCurrentTenantAsync();
        obj.TenantId = tenantInfo.Id;
        await _jobTicketManager.Update(obj);
    }

    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _jobTicketManager.Delete(id);
    }
}