using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.SchoolManager.SchClasses;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.SchoolManager.SchClasses.Dto;

namespace RhyoliteOrbit.Application.Services.SchoolManager.SchClasses
{
    
    public class SchClassAppService : RhyoliteOrbitAppServiceBase
    {
        private readonly ISchClassManager _schClassManager;
        private readonly IMapper _mapper;

        public SchClassAppService(ISchClassManager schClassManager, IMapper mapper)
        {
            _schClassManager = schClassManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<object> GetAll(int pageNo, int pageSize, string query)
        {
            return await _schClassManager.ListAll(pageNo, pageSize, query);
        }

        [HttpPost]
        public async Task<object> Create(CreateSchClassInput input)
        {
            var obj = _mapper.Map<SchClass>(input);
            return await _schClassManager.Create(obj);
        }

        [HttpPost]
        public async Task Update(UpdateSchClassInput input)
        {
            var obj = _mapper.Map<SchClass>(input);
            var tenantInfo = await GetCurrentTenantAsync();
            obj.TenantId = tenantInfo.Id;
            await _schClassManager.Update(obj);
        }

        [HttpDelete]
        public async Task Delete(Guid id)
        {
            await _schClassManager.Delete(id);
        }
    }
}