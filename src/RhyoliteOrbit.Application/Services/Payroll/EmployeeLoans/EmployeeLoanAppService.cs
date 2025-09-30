using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeBio;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeLoans;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeLoans.Dto;
using RhyoliteOrbit.DomainServices.Payroll.LoanTypes;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Services.Payroll.EmployeeLoans.Dto;

namespace RhyoliteOrbit.Services.Payroll.EmployeeLoans;

public class EmployeeLoanAppService: RhyoliteOrbitAppServiceBase
{
        private readonly IEmployeeLoanManager _employeeLoanManager;
        private readonly IEmployeeBioDataManager _employeeBioDataManager;
        private readonly ILoanTypeManager _loanTypeManager;
        private readonly IMapper _mapper;

        public EmployeeLoanAppService(IEmployeeLoanManager employeeLoanManager, IMapper mapper, IEmployeeBioDataManager employeeBioDataManager, ILoanTypeManager loanTypeManager)
        {
            _employeeLoanManager = employeeLoanManager;
            _mapper = mapper;
            _employeeBioDataManager = employeeBioDataManager;
            _loanTypeManager = loanTypeManager;
        }
        
        [HttpGet]
        public async Task<object> GetAll(int pageNo, int pageSize, string query)
        {
            return await _employeeLoanManager.ListAll(pageNo, pageSize, query);
        }

        [HttpGet]
        public async Task<object> GetLoanRepaymentSummary(Guid loanId)
        {
            return await _employeeLoanManager.GetLoanRepaymentSummary(loanId);
        }
        
        [HttpPost]
        public async Task AmortizeLoan(LoanAmortizationDto dto)
        {
            await _employeeLoanManager.AmortizeLoan(dto);
        }

        [HttpGet]
        public async Task<object> ListApprovedLoans()
        {
            return await _employeeLoanManager.GetApprovedLoans();
        }

        [HttpGet]
        public async Task<object> ListPastLoans()
        {
            return await _employeeLoanManager.GetPastLoans();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputList"></param>
        [HttpPost]
        public async Task Import(List<ImportEmployeeLoanInput> inputList)
        {
            foreach (var input in inputList)
            {
                var loanType = await _loanTypeManager.GetAsync(input.LoanTypeName);

                var employee = await _employeeBioDataManager.GetAsync(input.EmployeeIdentifier);

                if (employee != null && loanType != null)
                {

                    var createEmployeeLoanInput = new CreateEmployeeLoanInput
                    {
                        EmployeeId = employee.Id,
                        EmployeeIdentifier = employee.EmployeeIdentifier,
                        EmployeeName = input.EmployeeName,
                        LoanTypeName = loanType.Name,
                        Amount = input.Amount,
                        AnnualInterestRate = input.AnnualInterestRate,
                        CurrentBalance = 0,
                        ChargeInterest = input.AnnualInterestRate > 0,
                        Duration = input.Duration, 
                        DeductionStarts = input.DeductionStarts, 
                        InterestType = input.InterestType,
                        IsApproved = true, 
                        LoanDate = input.LoanDate, 
                        LoanTypeId = loanType.Id, 
                        MonthlyDeduction = 0,
                        InterestCharges = 0, 
                        NextDeduction= 0, 
                        Status = "",
                    };

                    var obj = _mapper.Map<EmployeeLoan>(createEmployeeLoanInput);

                    await _employeeLoanManager.Create(obj);

                }

                


            }

        }


        [HttpGet]
        public async Task<object> GetLoansForEmployee(Guid employeeId)
        {
            return await _employeeLoanManager.ListAll(employeeId);
        }
        
        [HttpGet]
        public async Task<EmployeeLoan> GetAsync(Guid id)
        {
            return await _employeeLoanManager.GetAsync(id);
        }

        [HttpGet]
        public async Task<object> ListOutStandingLoans()
        {
            return await _employeeLoanManager.GetOutStandingLoans();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        [HttpPost]
        public async Task Create(CreateEmployeeLoanInput input)
        {
            var obj = _mapper.Map<EmployeeLoan>(input);
            await _employeeLoanManager.Create(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        [HttpPost]
        public async Task Update(UpdateEmployeeLoanInput input)
        {
            var obj = _mapper.Map<EmployeeLoan>(input);
            var tenantInfo = await GetCurrentTenantAsync();
            obj.TenantId = tenantInfo.Id;
            await _employeeLoanManager.Update(obj);
        }

        [HttpGet]
        public async Task<object> ListPendingLoans()
        {
            return await _employeeLoanManager.GetPendingLoans();
        }

        [HttpDelete]
        public async Task Delete(Guid id)
        {
            await _employeeLoanManager.Delete(id);
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="ids"></param>
       /// <param name="approvalType"></param>
       [HttpPost]
        public async Task Approve(List<Guid> ids, string approvalType)
        {
            await _employeeLoanManager.Approve(ids, approvalType);
        }
}