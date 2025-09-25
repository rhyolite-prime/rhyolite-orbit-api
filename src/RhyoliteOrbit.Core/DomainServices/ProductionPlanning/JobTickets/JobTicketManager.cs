using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Payroll;
using RhyoliteOrbit.Models.ProductionPlanning;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.ProductionPlanning.JobTickets;

public class JobTicketManager: DomainService, IJobTicketManager
{
    private readonly IRepository<JobTicket, Guid> _repositoryJobTicket;
    private readonly IRepository<SystemNumber, Guid> _repositorySystemNumber;
    private readonly IRepository<EmployeeBioData, Guid> _repositoryEmployeeBioData;
        
    public JobTicketManager(IRepository<JobTicket, Guid> repositoryJobTicket, IRepository<SystemNumber, Guid> repositorySystemNumber, IRepository<EmployeeBioData, Guid> repositoryEmployeeBioData)
    {
        _repositoryJobTicket = repositoryJobTicket;
        _repositorySystemNumber = repositorySystemNumber;
        _repositoryEmployeeBioData = repositoryEmployeeBioData;
    }

    public async Task Create(JobTicket entity)
    {
        var datta = await _repositoryJobTicket.FirstOrDefaultAsync(x => x.ProductionOrderId == entity.ProductionOrderId && x.JobTicketIdentifier == entity.JobTicketIdentifier);

        if (datta == null)
        {
            
            //generate JobTicketIdentifier from system numbers
            var systemNumber = await _repositorySystemNumber.FirstOrDefaultAsync(x => x.Module == (int)RhyoliteOrbitConsts.ErpModules.Manufacturing && x.ItemName == "JobTicketID");
        
            // Increment the last number
            systemNumber.LastNo++;
            
            // Use FormatPattern if defined, otherwise default to [Prefix]-[Number]-[Suffix]
            var format = string.IsNullOrWhiteSpace(systemNumber.FormatPattern)
                ? "{prefix}-{number}-{suffix}"
                : systemNumber.FormatPattern;
            
            entity.JobTicketIdentifier = format
                .Replace("{prefix}", systemNumber.Prefix ?? string.Empty)
                .Replace("{number}", systemNumber.LastNo.ToString("D4")) // padded number e.g. 0001
                .Replace("{suffix}", systemNumber.Suffix ?? string.Empty);

            var employeeInfo = await _repositoryEmployeeBioData.GetAsync(entity.SupervisorId);
            entity.SupervisorName = !string.IsNullOrEmpty(employeeInfo.OtherName) ? $"{employeeInfo.FirstName}  {employeeInfo.LastName} {employeeInfo.OtherName}" : $"{employeeInfo.FirstName}  {employeeInfo.LastName}" ;
            entity.SupervisorEmail = employeeInfo.CompanyEmail;
            entity.SupervisorPhone = employeeInfo.PrimaryPhoneNumber;
            entity.SupervisorDepartment = employeeInfo.DepartmentName;
            await _repositoryJobTicket.InsertAsync(entity);
            return;
        }
        
        throw new UserFriendlyException(400, "Duplicate Job Tickets are not allowed.");
         
    }

    public async Task Update(JobTicket entity)
    {
        var employeeInfo = await _repositoryEmployeeBioData.GetAsync(entity.SupervisorId);
        entity.SupervisorName = !string.IsNullOrEmpty(employeeInfo.OtherName) ? $"{employeeInfo.FirstName}  {employeeInfo.LastName} {employeeInfo.OtherName}" : $"{employeeInfo.FirstName}  {employeeInfo.LastName}" ;
        entity.SupervisorEmail = employeeInfo.CompanyEmail;
        entity.SupervisorPhone = employeeInfo.PrimaryPhoneNumber;
        entity.SupervisorDepartment = employeeInfo.DepartmentName;
        
        await _repositoryJobTicket.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryJobTicket.GetAsync(id);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<JobTicket>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.ToLower().Contains(query.ToLower()));
        }
            
        var totalCount =
            await _repositoryJobTicket.CountAsync(predicate);
            
        var data = await (await _repositoryJobTicket.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.Description, a.WorkCenterName, a.WorkCenterId, a.ProductionOrderId, a.JobTicketIdentifier, a.EndTime, a.StartTime, a.EstimatedDurationInHours, a.SupervisorDepartment, a.CreationTime, a.SupervisorEmail, a.SupervisorId, a.SupervisorName,  }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

        var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

        var paginatedResult = new
        {
            pageNo,
            pageSize,
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
        await _repositoryJobTicket.DeleteAsync(id);
    }
}