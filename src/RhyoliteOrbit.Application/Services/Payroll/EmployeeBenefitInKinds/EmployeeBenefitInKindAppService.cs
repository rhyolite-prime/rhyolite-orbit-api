using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.BikTypes;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeBenefitInKinds;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeBio;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.EmployeeBenefitInKinds.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeBenefitInKinds;

public class EmployeeBenefitInKindAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IEmployeeBenefitInKindManager _employeeBenefitInKindManager;
        private readonly IEmployeeBioDataManager _employeeBioDataManager;
        private readonly IBikTypeManager _bikTypeManager;
        private readonly IMapper _mapper;

        public EmployeeBenefitInKindAppService(IEmployeeBenefitInKindManager employeeBenefitInKindManager, IMapper mapper, IEmployeeBioDataManager employeeBioDataManager, IBikTypeManager bikTypeManager)
        {
            _employeeBenefitInKindManager = employeeBenefitInKindManager;
            _mapper = mapper;
            _employeeBioDataManager = employeeBioDataManager;
            _bikTypeManager = bikTypeManager;
        }

        [HttpGet]
        public async Task<object> GetAll(int pageNo, int pageSize, string query)
        {
            return await _employeeBenefitInKindManager.ListAll(pageNo, pageSize, query);
        }

        [HttpGet]
        public async Task<object> ListAll(Guid employeeId)
        {
            return await _employeeBenefitInKindManager.ListAll(employeeId);
        }

        [HttpPost]
        public async Task Import(List<ImportEmployeeBenefitInKindInput> inputList)
        {

            foreach (var input in inputList)
            {
                var benefitsInKindType = await _bikTypeManager.GetAsync(input.BenefitInKindTypeName);

                var employee = await _employeeBioDataManager.GetAsync(input.EmployeeIdentifier);

                if (benefitsInKindType != null && employee != null)
                {
                    var createEmployeeBenefitInKindInput = new CreateEmployeeBenefitInKindInput
                    {
                        EmployeeId = employee.Id,
                        EmployeeName = input.EmployeeName,
                        BenefitInKindTypeId= benefitsInKindType.Id,
                        BenefitInKindTypeName= benefitsInKindType.Name,
                        Amount= input.Amount, 
                        EmployeeIdentifier = input.EmployeeIdentifier,
                    };

                    var obj = _mapper.Map<EmployeeBenefitInKind>(createEmployeeBenefitInKindInput);

                    await _employeeBenefitInKindManager.Create(obj);

                }

            }
        }
        
        
        [HttpPost]
        public async Task<object> Create(CreateEmployeeBenefitInKindInput input)
        {
            var obj = _mapper.Map<EmployeeBenefitInKind>(input);
            return await _employeeBenefitInKindManager.Create(obj);
        }

        [HttpPost]
        public async Task Update(UpdateEmployeeBenefitInKindInput input)
        {
            var obj = _mapper.Map<EmployeeBenefitInKind>(input);
            await _employeeBenefitInKindManager.Update(obj);
        }

        [HttpDelete]
        public async Task Delete(Guid id)
        {
            await _employeeBenefitInKindManager.Delete(id);
        }
}