using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.SchoolManager.StudentAttendances;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.SchoolManager.StudentAttendances.Dto;

namespace RhyoliteOrbit.Application.Services.SchoolManager.StudentAttendances   
{

    public class StudentAttendanceAppService : RhyoliteOrbitAppServiceBase
    {
        private readonly IStudentAttendanceManager _studentAttendanceManager;
        private readonly IMapper _mapper;

        public StudentAttendanceAppService(IStudentAttendanceManager studentAttendanceManager, IMapper mapper)
        {
            _studentAttendanceManager = studentAttendanceManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<object> GetAll(int pageNo, int pageSize, string query)
        {
            return await _studentAttendanceManager.GetAll(pageNo, pageSize, query);
        }

        [HttpPost]
        public async Task<object> Create(CreateStudentAttendanceInput input)
        {
            var obj = _mapper.Map<StudentAttendance>(input);
            return await _studentAttendanceManager.Create(obj);
        }

        [HttpPost]
        public async Task Update(UpdateStudentAttendanceInput input)
        {
            var obj = _mapper.Map<StudentAttendance>(input);
            var tenantInfo = await GetCurrentTenantAsync();
            obj.TenantId = tenantInfo.Id;
            await _studentAttendanceManager.Update(obj);
        }

        [HttpDelete]
        public async Task Delete(Guid id)
        {
            await _studentAttendanceManager.Delete(id);
        }
    }
}