using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.SchoolManager.ResultTypes;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.SchoolManager.ResultTypes.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.ResultTypes
{
    public class ResultTypeAppService : RhyoliteOrbitAppServiceBase
    {
        private readonly IResultTypeManager _resultTypeManager;
        private readonly IMapper _mapper;

        public ResultTypeAppService(IResultTypeManager resultTypeManager, IMapper mapper)
        {
            _resultTypeManager = resultTypeManager;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<object> Create(CreateResultTypeInput input)
        {
            var obj = _mapper.Map<ResultType>(input);
            return await _resultTypeManager.Create(obj);
        }


        [HttpGet]
        public async Task<object> GetByTenant(int tenantId)
        {
           return await _resultTypeManager.ListAll(tenantId);
        }

        [HttpGet]
        public async Task<object> GetByLevel(Guid levelId)
        {
            return await _resultTypeManager.ListAll(levelId);
        }

        [HttpPost]
        public async Task Update(UpdateResultTypeInput input)
        {
            var obj = _mapper.Map<ResultType>(input);
            await _resultTypeManager.Update(obj);
        }

        [HttpGet]
        public async Task<object> GetByClass(Guid classId)
        {
            return await _resultTypeManager.ListByClass(classId);
        }

        [HttpDelete]
        public async Task Delete(Guid id)
        {
            await _resultTypeManager.Delete(id);
        }
    }
}