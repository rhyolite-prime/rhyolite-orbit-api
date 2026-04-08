using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.DeductionTypes;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeBio;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeDeductions;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.EmployeeDeductions.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeDeductions;

public class EmployeeDeductionAppService: RhyoliteOrbitAppServiceBase
{
        private readonly IEmployeeDeductionManager _employeeDeductionManager;
        private readonly IEmployeeBioDataManager _employeeBioDataManager;
        private readonly IDeductionTypeManager _deductionTypeManager;
        private readonly IMapper _mapper;

        public EmployeeDeductionAppService(IEmployeeDeductionManager employeeDeductionManager, IMapper mapper, IEmployeeBioDataManager employeeBioDataManager, IDeductionTypeManager deductionTypeManager)
        {
            _employeeDeductionManager = employeeDeductionManager;
            _mapper = mapper;
            _employeeBioDataManager = employeeBioDataManager;
            _deductionTypeManager = deductionTypeManager;
        }

        [HttpGet]
        public async Task<object> GetAll(int pageNo, int pageSize, string query)
        {
            return await _employeeDeductionManager.ListAll(pageNo, pageSize, query);
        }

        [HttpGet]
        public async Task<object> GetDeductionsForEmployee(Guid employeeId)
        {
            return await _employeeDeductionManager.ListAll(employeeId);
        }
        
        [HttpPost]
        public async Task Create(CreateEmployeeDeductionInput input)
        {
            var obj = _mapper.Map<EmployeeDeduction>(input);
            await _employeeDeductionManager.Create(obj);
        }

        [HttpPost]
        public async Task Import(List<ImportEmployeeDeductionInput> inputList)
        {
            foreach (var input in inputList)
            {
                var deductionType = await _deductionTypeManager.GetAsync(input.DeductionTypeName);
                var employee = await _employeeBioDataManager.GetAsync(input.EmployeeIdentifier);

                if (deductionType != null && employee != null)
                {
                    var employeeDeduction = new CreateEmployeeDeductionInput
                    {
                        Amount= input.Amount,
                        DeductionTypeId = deductionType.Id, 
                        DeductionTypeName = input.DeductionTypeName,
                        EmployeeId = employee.Id, 
                        EmployeeIdentifier = employee.EmployeeIdentifier, 
                        EmployerAmount = input.EmployerAmount, 
                        EmployeeName = string.IsNullOrEmpty(employee.OtherName) ? $"{employee.LastName} {employee.FirstName}" : $"{employee.LastName} {employee.FirstName} {employee.OtherName}",

                    };

                    var obj = _mapper.Map<EmployeeDeduction>(employeeDeduction);
                    await _employeeDeductionManager.Create(obj);

                }

            }
        }
        
        [HttpPost]
        public async Task Update(UpdateEmployeeDeductionInput input)
        {
            var obj = _mapper.Map<EmployeeDeduction>(input);
            var tenantInfo = await GetCurrentTenantAsync();
            obj.TenantId = tenantInfo.Id;
            await _employeeDeductionManager.Update(obj);
        }

        [HttpDelete]
        public async Task Delete(Guid id)
        {
            await _employeeDeductionManager.Delete(id);
        }
}