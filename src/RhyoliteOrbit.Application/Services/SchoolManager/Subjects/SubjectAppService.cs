using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.SchoolManager.Subjects;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.SchoolManager.Subjects.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.Subjects
{
    public class SubjectAppService : RhyoliteOrbitAppServiceBase
    {
        private readonly ISubjectManager _subjectManager;
        private readonly IMapper _mapper;

        public SubjectAppService(ISubjectManager subjectManager, IMapper mapper)
        {
            _subjectManager = subjectManager;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task Create(CreateSubjectInput input)
        {
            var obj = _mapper.Map<Subject>(input);
             await _subjectManager.Create(obj);
        }

        [HttpGet]
        public async Task<object> GetAll(int pageNo, int pageSize, string query)
        {
            return await _subjectManager.ListAll(pageNo, pageSize, query);
        }

        [HttpPost]
        public async Task Update(UpdateSubjectInput input)
        {
            var obj = _mapper.Map<Subject>(input);
            var tenantInfo = await GetCurrentTenantAsync();
            obj.TenantId = tenantInfo.Id;
            await _subjectManager.Update(obj);
        }

        [HttpDelete]
        public async Task Delete(Guid id)
        {
            await _subjectManager.Delete(id);
        }
    }
}