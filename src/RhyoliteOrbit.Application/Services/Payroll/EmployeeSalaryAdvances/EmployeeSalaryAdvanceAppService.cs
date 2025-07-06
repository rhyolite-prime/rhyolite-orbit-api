using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeSalaryAdvances;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.EmployeeSalaryAdvances.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeSalaryAdvances;

public class EmployeeSalaryAdvanceAppService : RhyoliteOrbitAppServiceBase
{
    
        private readonly IEmployeeSalaryAdvanceManager _employeeSalaryAdvanceManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeSalaryAdvanceManager"></param>
        /// <param name="mapper"></param>
        public EmployeeSalaryAdvanceAppService(IEmployeeSalaryAdvanceManager employeeSalaryAdvanceManager, IMapper mapper)
        {
            _employeeSalaryAdvanceManager = employeeSalaryAdvanceManager;
            _mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<object> GetAll(int pageNo, int pageSize, string query)
        {
            return await _employeeSalaryAdvanceManager.ListAll(pageNo, pageSize, query);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public async Task<object> GetSalaryAdvanceForEmployee(Guid employeeId)
        {
            return await _employeeSalaryAdvanceManager.ListAll(employeeId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<object> ListPendingSalaryAdvance(int pageNo, int pageSize, string query)
        {
            return await _employeeSalaryAdvanceManager.ListPendingSalaryAdvance(pageNo, pageSize, query);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<object> GetAsync(Guid id)
        {
            return await _employeeSalaryAdvanceManager.GetAsync(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="approvalType"></param>
        public async Task Approve(List<Guid> ids, string approvalType)
        {
            await _employeeSalaryAdvanceManager.Approve(ids, approvalType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        public async Task Create(CreateEmployeeSalaryAdvanceInput input)
        {
            var obj = _mapper.Map<EmployeeSalaryAdvance>(input);
            await _employeeSalaryAdvanceManager.Create(obj);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        public async Task Update(UpdateEmployeeSalaryAdvanceInput input)
        {
            var obj = _mapper.Map<EmployeeSalaryAdvance>(input);
            await _employeeSalaryAdvanceManager.Update(obj);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public async Task Delete(Guid id)
        {
            await _employeeSalaryAdvanceManager.Delete(id);

        }
}