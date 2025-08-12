using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.SchoolManager.StudentBills;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Services.SchoolManager.StudentBills.Dto;

namespace RhyoliteOrbit.Application.Services.SchoolManager.StudentBills
{

    public class StudentBillAppService : RhyoliteOrbitAppServiceBase
    {
        private readonly IStudentBillManager _studentBillManager;
        private readonly IMapper _mapper;

        public StudentBillAppService(IStudentBillManager studentBillManager, IMapper mapper)
        {
            _studentBillManager = studentBillManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<object> GetAll(int pageNo, int pageSize, string query)
        {
            return await _studentBillManager.ListAll(pageNo, pageSize, query);
        }

        [HttpPost]
        public async Task<object> Create(CreateStudentBillInput input)
        {
            var obj = _mapper.Map<StudentBill>(input);
            return await _studentBillManager.Create(obj);
        }

        [HttpPost]
        public async Task Update(UpdateStudentBillInput input)
        {
            var obj = _mapper.Map<StudentBill>(input);
            var tenantInfo = await GetCurrentTenantAsync();
            obj.TenantId = tenantInfo.Id;
            await _studentBillManager.Update(obj);
        }

        [HttpDelete]
        public async Task Delete(Guid id)
        {
            await _studentBillManager.Delete(id);
        }
    }
}