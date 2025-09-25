using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.Payroll.EmployeeBio.Dto;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Payroll.EmployeeBio;

public class EmployeeBioDataManager: Abp.Domain.Services.DomainService, IEmployeeBioDataManager
{
    private readonly IRepository<EmployeeBioData, Guid> _repositoryEmployeeBioData;
        private readonly IRepository<SystemNumber, Guid> _repositorySystemNumber; 

        public EmployeeBioDataManager(IRepository<EmployeeBioData, Guid> repositoryEmployeeBioData, IRepository<SystemNumber, Guid> repositorySystemNumber)
        {
            _repositoryEmployeeBioData = repositoryEmployeeBioData;
            _repositorySystemNumber = repositorySystemNumber;
        }

        public async Task Create(EmployeeBioData entity)
        {
            var datta = await _repositoryEmployeeBioData.FirstOrDefaultAsync(x => x.EmployeeIdentifier == entity.EmployeeIdentifier);

            if (datta == null)
            {

                entity.FirstName = entity.FirstName.Trim();
                entity.LastName = entity.LastName.Trim();

                await _repositoryEmployeeBioData.InsertAsync(entity);

                //update system numbers
                var systemNumberInfo = await _repositorySystemNumber.FirstOrDefaultAsync(x=> x.ItemName == "EmployeeID");

                if(systemNumberInfo != null)
                {
                    systemNumberInfo.LastNo++;

                    await _repositorySystemNumber.UpdateAsync(systemNumberInfo);
                }


                return;
            }

            throw new UserFriendlyException(400, "Duplicate Employee ID's are not allowed.", "");

        }

        public async Task Update(EmployeeBioData entity)
        {
            entity.FirstName = entity.FirstName.Trim();
            entity.LastName = entity.LastName.Trim();

            await _repositoryEmployeeBioData.UpdateAsync(entity);
        }

        public async Task<object> GetAsync(Guid id)
        {
            return await _repositoryEmployeeBioData.FirstOrDefaultAsync(id);
        }

        public async Task<EmployeeBioData> GetAsync(string employeeIdentifier)
        {
            return await _repositoryEmployeeBioData.FirstOrDefaultAsync(x=> !string.IsNullOrEmpty(employeeIdentifier) && x.EmployeeIdentifier == employeeIdentifier.Trim());
        }

        
        public async Task<object> ListAll(int pageNo, int pageSize, string query)
        {
            var predicate = PredicateBuilder.New<EmployeeBioData>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.FirstName.Contains(query) || x.LastName.Contains(query) || x.EmployeeIdentifier.Contains(query) || x.OtherName.Contains(query) || x.CompanyEmail.Contains(query) || x.PrimaryPhoneNumber.Contains(query));
            }
            
            var totalCount =
                
                await _repositoryEmployeeBioData.CountAsync(predicate);
            
            var data = await (await _repositoryEmployeeBioData.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.FirstName, a.LastName, a.OtherName, a.EmployeeIdentifier, a.EmployeePhoto, a.PrimaryPhoneNumber, a.CategoryName, a.DepartmentName, a.Gender,  }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

            var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

            var paginatedResult = new
            {
                pageNo,
                totalCount,
                totalPages,
                data,
                lowerBound = pageSize * (pageNo - 1) + 1,
                upperBound = totalPages == pageNo ? totalCount : pageNo * pageSize,
            };

            return paginatedResult;

        }
        
        
        public async Task Delete(Guid id)
        {
            await _repositoryEmployeeBioData.DeleteAsync(id);

        }
        
        public async Task UpdateStatus(Guid employeeId, RhyoliteOrbitConsts.EmployeeStatus status)
        {
            var employeeInfo = await _repositoryEmployeeBioData.FirstOrDefaultAsync(employeeId);

            if (employeeInfo != null)
            {
                employeeInfo.Status = (int)status;

                await _repositoryEmployeeBioData.UpdateAsync(employeeInfo);
            }

        }

        //export data
        public async Task<List<AllowanceExportDto>> ExportAllowances()
        {
            var datta =  await _repositoryEmployeeBioData.GetAllListAsync();

            var query = from u1 in datta 
                        select new AllowanceExportDto
                        {
                            EmployeeIdentifier = u1.EmployeeIdentifier,
                            Amount = 0,
                            IsTaxable = "Yes",
                            AllowanceDays = 0,
                            AllowanceType = string.Empty,
                            EmployeeName = string.IsNullOrEmpty(u1.OtherName) ? $"{u1.LastName} {u1.FirstName}" : $"{u1.LastName} {u1.FirstName} {u1.OtherName}",
                        };

            return query.OrderBy(a => a.EmployeeName).ToList();
        }

        public async Task<List<DeductionsExportDto>> ExportDeductions()
        {
            var datta = await _repositoryEmployeeBioData.GetAllListAsync();

            var query = from u1 in datta
                        select new DeductionsExportDto
                        {
                            EmployeeIdentifier = u1.EmployeeIdentifier,
                            Amount = 0,
                            EmployerAmount = 0,
                            DeductionType = string.Empty,
                            EmployeeName = string.IsNullOrEmpty(u1.OtherName) ? $"{u1.LastName} {u1.FirstName}" : $"{u1.LastName} {u1.FirstName} {u1.OtherName}",
                        };

            return query.OrderBy(a => a.EmployeeName).ToList();
        }

        public async Task<List<SalaryInfoDto>> ExportSalaryInfo()
        {
            var datta = await _repositoryEmployeeBioData.GetAllListAsync();

            var query = from u1 in datta
                        select new SalaryInfoDto
                        {
                            EmployeeIdentifier = u1.EmployeeIdentifier,
                            SalaryType = "Salaried/Hourly",
                            PayType = "Cheque/Cash/Mobile Wallet",
                            AccountNumber = string.Empty,
                            BankName = string.Empty,
                            Currency = string.Empty,
                            MonthlySalary = 0,
                            EmployeeName = string.IsNullOrEmpty(u1.OtherName) ? $"{u1.LastName} {u1.FirstName}" : $"{u1.LastName} {u1.FirstName} {u1.OtherName}",
                        };

            return query.OrderBy(a => a.EmployeeName).ToList();
        }

        public async Task<List<SsnitExportDto>> ExportSsnit()
        {
            var datta = await _repositoryEmployeeBioData.GetAllListAsync();

            var query = from u1 in datta
                        select new SsnitExportDto
                        {
                            EmployeeIdentifier = u1.EmployeeIdentifier,
                            SocialSecurityNo = string.Empty,
                            SocialSecurityFundEmployeeContribution = 0,
                            SocialSecurityFundEmployerContribution = 0,
                            ProvidentFundEmployeeContribution = 0,
                            ProvidentFundEmployerContribution = 0,
                            SecondProvidentFundEmployeeContribution = 0,
                            SecondProvidentFundEmployerContribution = 0,
                            SuperAnnuationEmployeeContribution = 0,
                            SuperAnnuationEmployerContribution = 0,
                            EmployeeName = string.IsNullOrEmpty(u1.OtherName) ? $"{u1.LastName} {u1.FirstName}" : $"{u1.LastName} {u1.FirstName} {u1.OtherName}",
                        };

            return query.OrderBy(a => a.EmployeeName).ToList();
        }
        
        public async Task<List<EmployeeLoansExportDto>> ExportEmployeeLoans()
        {
            var datta = await _repositoryEmployeeBioData.GetAllListAsync();

            var query = from u1 in datta
                        select new EmployeeLoansExportDto
                        {
                            EmployeeIdentifier = u1.EmployeeIdentifier,
                            Amount = 0,
                            LoanDate = DateTime.UtcNow.ToString("dd-MMM-yyyy"),
                            InterestRate = 0, 
                            InterestType = "Simple Interest/Compound Interest",
                            LoanDuration = 12, 
                            LoanType = "Rent Loan",
                            NextDeduction = 3

                        };

            return query.OrderBy(a => a.EmployeeIdentifier).ToList();

        }

        public async Task<List<BenefitsExportDto>> ExportBenefitsInKind()
        {
            var datta = await _repositoryEmployeeBioData.GetAllListAsync();

            var query = from u1 in datta
                        select new BenefitsExportDto
                        {
                            EmployeeIdentifier = u1.EmployeeIdentifier,
                            Amount = 0, 
                            BenefitsInKindTypeName = "",
                            EmployeeName = string.IsNullOrEmpty(u1.OtherName) ? $"{u1.LastName} {u1.FirstName}" : $"{u1.LastName} {u1.FirstName} {u1.OtherName}",
                        };

            return query.OrderBy(a => a.EmployeeName).ToList();
        }
}