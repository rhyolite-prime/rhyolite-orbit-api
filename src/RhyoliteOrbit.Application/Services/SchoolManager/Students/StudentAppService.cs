using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.SchoolManager.Students;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.SchoolManager.Students.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.Students
{
    public class StudentAppService : RhyoliteOrbitAppServiceBase
    {
        private readonly IStudentManager _studentManager;
        private readonly IMapper _mapper;

        public StudentAppService(IStudentManager studentManager, IMapper mapper)
        {
            _studentManager = studentManager;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task Create(CreateStudentInput input)
        {
            var obj = _mapper.Map<Student>(input);
            await _studentManager.Create(obj);
        }

        [HttpGet]
        public async Task<object> GetAll(int pageNo, int pageSize, string query)
        {
            return await _studentManager.ListAll();
        }

        [HttpPost]
        public async Task Update(UpdateStudentInput input)
        {
            var obj = _mapper.Map<Student>(input);
            var tenantInfo = await GetCurrentTenantAsync();
            obj.TenantId = tenantInfo.Id;
            await _studentManager.Update(obj);
        }

        [HttpDelete]
        public async Task Delete(Guid id)
        {
            await _studentManager.Delete(id);
        }
    }
}