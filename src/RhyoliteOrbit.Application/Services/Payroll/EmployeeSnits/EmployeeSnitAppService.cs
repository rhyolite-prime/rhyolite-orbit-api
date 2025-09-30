using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeBio;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeSnits;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.EmployeeSnits.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeSnits;

public class EmployeeSnitAppService: RhyoliteOrbitAppServiceBase
{
        private readonly IEmployeeSnitManager _employeeSnitManager;
        private readonly IEmployeeBioDataManager _employeeBioDataManager;
        private readonly IMapper _mapper;

        public EmployeeSnitAppService(IEmployeeSnitManager employeeSnitManager, IMapper mapper, IEmployeeBioDataManager employeeBioDataManager)
        {
            _employeeSnitManager = employeeSnitManager;
            _mapper = mapper;
            _employeeBioDataManager = employeeBioDataManager;
        }

        [HttpGet]
        public async Task<object> GetAll(int pageNo, int pageSize, string query)
        {
            return await _employeeSnitManager.ListAll(pageNo, pageSize, query);
        }

        [HttpPost]
        public async Task Create(CreateEmployeeSnitInput input)
        {
            var obj = _mapper.Map<EmployeeSnit>(input);
            await _employeeSnitManager.Create(obj);
        }


        [HttpPost]
        public async Task Import(List<ImportEmployeeSnitInput> inputList)
        {
            foreach (var input in inputList)
            {
                var employee = await _employeeBioDataManager.GetAsync(input.EmployeeIdentifier);

                if(employee != null)
                {

                    var employeeSsnit = new CreateEmployeeSnitInput
                    {
                        EmployeeIdentifier = input.EmployeeIdentifier,
                        EmployeeName = string.IsNullOrEmpty(employee.OtherName) ? $"{employee.LastName} {employee.FirstName}" : $"{employee.LastName} {employee.FirstName} {employee.OtherName}",
                        EmployeeId = employee.Id, 
                        ProvidentFundEmployeeContribution = input.ProvidentFundEmployeeContribution, 
                        ProvidentFundEmployerContribution = input.ProvidentFundEmployerContribution, 
                        SecondProvidentFundEmployeeContribution = input.SecondProvidentFundEmployeeContribution, 
                        SecondProvidentFundEmployerContribution = input.SecondProvidentFundEmployerContribution,
                        SocialSecurityFundEmployeeContribution = input.SocialSecurityFundEmployeeContribution, 
                        SocialSecurityFundEmployerContribution = input.SocialSecurityFundEmployerContribution, 
                        SocialSecurityNo = input.SocialSecurityNo, 
                        SuperAnnuationEmployeeContribution = input.SuperAnnuationEmployeeContribution, 
                        SuperAnnuationEmployerContribution = input.SuperAnnuationEmployerContribution

                    };

                    var obj = _mapper.Map<EmployeeSnit>(employeeSsnit);

                    await _employeeSnitManager.Create(obj);
                }
            }

        }

        [HttpPost]
        public async Task Update(UpdateEmployeeSnitInput input)
        {
            var obj = _mapper.Map<EmployeeSnit>(input);
            var tenantInfo = await GetCurrentTenantAsync();
            obj.TenantId = tenantInfo.Id;
            await _employeeSnitManager.Update(obj);
        }

        [HttpGet]
        public async Task<object> GetEmployeeSnitDetails(Guid employeeId)
        {
          return await _employeeSnitManager.GetAsync(employeeId);
        }

        [HttpDelete]
        public async Task Delete(Guid id)
        {
            await _employeeSnitManager.Delete(id);
        }
}