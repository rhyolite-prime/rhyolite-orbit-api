using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Application.Services.SchoolManager.SubjectRemarks.Dto;
using RhyoliteOrbit.DomainServices.SchoolManager.SubjectRemarks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.Application.Services.SchoolManager.SubjectRemarks
{
    public class SubjectRemarkAppService : RhyoliteOrbitAppServiceBase
    {
        private readonly ISubjectRemarkManager _subjectRemarkManager;
        private readonly IMapper _mapper;

        public SubjectRemarkAppService(ISubjectRemarkManager subjectRemarkManager, IMapper mapper)
        {
            _subjectRemarkManager = subjectRemarkManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<object> GetAll(int pageNo, int pageSize, string query)
        {
            return await _subjectRemarkManager.ListAll(pageNo, pageSize, query);
        }

        [HttpPost]
        public async Task Create(CreateSubjectRemarkInput input)
        {
            var obj = _mapper.Map<SubjectRemark>(input);
            await _subjectRemarkManager.Create(obj);
        }

        [HttpPost]
        public async Task Update(UpdateSubjectRemarkInput input)
        {
            var obj = _mapper.Map<SubjectRemark>(input);
            var tenantInfo = await GetCurrentTenantAsync();
            obj.TenantId = tenantInfo.Id;
            await _subjectRemarkManager.Update(obj);
        }

        [HttpDelete]
        public async Task Delete(Guid id)
        {
            await _subjectRemarkManager.Delete(id);
        }
    }
}
