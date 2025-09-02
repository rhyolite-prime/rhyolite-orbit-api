using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.SchoolManager.Levels;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.SchoolManager.Levels.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.Levels
{
    public class LevelAppService : RhyoliteOrbitAppServiceBase
    {
        private readonly ILevelManager _levelManager;
        private readonly IMapper _mapper;

        public LevelAppService(ILevelManager levelManager, IMapper mapper)
        {
            _levelManager = levelManager;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<object> Create(CreateLevelInput input)
        {
            var obj = _mapper.Map<Level>(input);
            return await _levelManager.Create(obj);
        }


        [HttpGet]
        public async Task<object> GetAll(int pageNo, int pageSize, string query)
        {
            return await _levelManager.ListAll(pageNo, pageSize, query);
        }

        [HttpPost]
        public async Task Update(UpdateLevelInput input)
        {
            var obj = _mapper.Map<Level>(input);
            var tenantInfo = await GetCurrentTenantAsync();
            obj.TenantId = tenantInfo.Id;
            await _levelManager.Update(obj);
        }

        [HttpDelete]
        public async Task Delete(Guid id)
        {
            await _levelManager.Delete(id);
        }
    }
}