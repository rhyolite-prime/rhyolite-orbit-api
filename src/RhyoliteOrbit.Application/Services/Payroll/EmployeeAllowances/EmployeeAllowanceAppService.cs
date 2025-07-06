using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.AllowanceTypes;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeAllowances;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeBio;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.EmployeeAllowances.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeAllowances;

public class EmployeeAllowanceAppService: RhyoliteOrbitAppServiceBase
{
        private readonly IEmployeeAllowanceManager _employeeAllowanceManager;
        private readonly IAllowanceTypeManager _allowanceTypeManager;
        private readonly IEmployeeBioDataManager _employeeBioDataManager;
        private readonly IMapper _mapper;

        public EmployeeAllowanceAppService(IEmployeeAllowanceManager employeeAllowanceManager, IMapper mapper, IEmployeeBioDataManager employeeBioDataManager, IAllowanceTypeManager allowanceTypeManager)
        {
            _employeeAllowanceManager = employeeAllowanceManager;
            _mapper = mapper;
            _employeeBioDataManager = employeeBioDataManager;
            _allowanceTypeManager = allowanceTypeManager;
        } 

        [HttpGet]
        public async Task<object> GetAll(int pageNo, int pageSize, string query)
        {
            return await _employeeAllowanceManager.ListAll(pageNo, pageSize, query);
        }

        [HttpGet]
        public async Task<object> GetAllowancesForEmployee(Guid employeeId)
        {
            return await _employeeAllowanceManager.ListAll(employeeId);
        }

        [HttpPost]
        public async Task<object> Create(CreateEmployeeAllowanceInput input)
        {
            var obj = _mapper.Map<EmployeeAllowance>(input);
            return await _employeeAllowanceManager.Create(obj);
        }

        [HttpPost]
        public async Task Import(List<ImportEmployeeAllowanceInput> inputList)
        {
            foreach (var input in inputList)
            {
                 
                var allowanceType = await _allowanceTypeManager.GetAsync(input.AllowanceTypeName);
                var employee = await _employeeBioDataManager.GetAsync(input.EmployeeIdentifier);

                if (allowanceType != null && employee!= null)
                {
                    var createEmployeeAllowanceInput = new CreateEmployeeAllowanceInput
                    {
                        AllowanceTypeId = allowanceType.Id,
                        AllowanceTypeName= allowanceType.Name,
                        AllowanceDays = input.AllowanceDays, 
                        Amount= input.Amount,
                        IsTaxable= input.Taxable,
                        EmployeeId = employee.Id,
                        EmployeeIdentifier = employee.EmployeeIdentifier, 
                        EmployeeName = string.IsNullOrEmpty(employee.OtherName) ? $"{employee.LastName} {employee.FirstName}" : $"{employee.LastName} {employee.FirstName} {employee.OtherName}", 
                        IsMonthly = true, 
                        ProvidentFund = false, 
                        Ssf = false,
                    };
                
                    var obj = _mapper.Map<EmployeeAllowance>(createEmployeeAllowanceInput);

                    await _employeeAllowanceManager.Create(obj);
                }


            }
        }

        [HttpPost]
        public async Task Update(UpdateEmployeeAllowanceInput input)
        {
            var obj = _mapper.Map<EmployeeAllowance>(input);
            await _employeeAllowanceManager.Update(obj);
        }

        [HttpDelete]
        public async Task Delete(Guid id)
        {
            await _employeeAllowanceManager.Delete(id);

        }
    
}