using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.ProductionPlanning.FinishedGoods;

namespace RhyoliteOrbit.Services.ProductionPlanning.FinishedGoods;

public class FinishedGoodAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IFinishedGoodManager  _finishedGoodManager;
    private readonly IMapper _mapper;

    public FinishedGoodAppService(IFinishedGoodManager finishedGoodManager, IMapper mapper)
    {
        _finishedGoodManager = finishedGoodManager;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<object> GetAll(int pageNo, int pageSize, string query)
    {
        return await _finishedGoodManager.ListAll(pageNo, pageSize, query);
    }
    
    [HttpDelete]
    public async Task Delete(Guid id)
    {
        await _finishedGoodManager.Delete(id);
    }
}