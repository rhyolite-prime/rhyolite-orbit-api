using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.SchoolManager.Students;
using RhyoliteOrbit.DomainServices.SchoolManager.Students.Dto;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.SchoolManager.Students.Dto;

namespace RhyoliteOrbit.Services.SchoolManager.Students
{
    public class StudentAppService : RhyoliteOrbitAppServiceBase
    {
        private readonly IStudentManager _studentManager;
        private readonly IValidateTenant _validateTenant;
        private readonly IMapper _mapper;

        public StudentAppService(IStudentManager studentManager, IMapper mapper, IValidateTenant validateTenant)
        {
            _studentManager = studentManager;
            _mapper = mapper;
            _validateTenant = validateTenant;
        }

        [HttpGet]
        public async Task<object> GetAll(int pageNo, int pageSize, string query, Guid classId)
        {
            return await _studentManager.GetAll(pageNo, pageSize, query, classId);
        }

        [HttpGet]
        public async Task<object> GetDetails(Guid id)
        {
            return await _studentManager.GetAsync(id);
        }

        [HttpPost]
        public async Task Create(CreateStudentInput input)
        {
            var obj = _mapper.Map<Student>(input);
            await _studentManager.Create(obj);
        }
        
        [HttpPost]
        public async Task Promote(StudentPromotion dto)
        {
            await _studentManager.Promote(dto);
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
        
        //external props
        /// <summary>
        /// GetStudentInfo
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="pin"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetStudentInfo(string studentId, string pin)
        {
            var tenantId = await _validateTenant.Validate();
            return await _studentManager.GetStudentInfo(studentId, pin, tenantId);
        }
        
        [HttpGet] 
        public async Task AssignPinsToAllStudents()
        {
            await _studentManager.AssignPinsToAllStudents();
        }
        
    }
}

