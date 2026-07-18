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

public class OperationManager: DomainService, IOperationManager
{
    private readonly IRepository<Operation, Guid> _repositoryOperation;
    private readonly IRepository<SystemNumber, Guid> _repositorySystemNumber;
    private readonly IRepository<EmployeeBioData, Guid> _repositoryEmployeeBioData;
        
    public OperationManager(IRepository<Operation, Guid> repositoryJobTicket, IRepository<SystemNumber, Guid> repositorySystemNumber, IRepository<EmployeeBioData, Guid> repositoryEmployeeBioData)
    {
        _repositoryOperation = repositoryJobTicket;
        _repositorySystemNumber = repositorySystemNumber;
        _repositoryEmployeeBioData = repositoryEmployeeBioData;
    }

    public async Task Create(Operation entity)
    {
        var datta = await _repositoryOperation.FirstOrDefaultAsync(x => x.ProductionOrderId == entity.ProductionOrderId && x.JobTicketIdentifier == entity.JobTicketIdentifier);

        if (datta == null)
        {
            
            //generate JobTicketIdentifier from system numbers
            var systemNumber = await _repositorySystemNumber.FirstOrDefaultAsync(x => x.ModuleId == (int)RhyoliteOrbitConsts.ErpModule.Manufacturing && x.ItemName == "JobTicketID");
        
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
            await _repositoryOperation.InsertAsync(entity);
            return;
        }
        
        throw new UserFriendlyException(400, "Duplicate Job Tickets are not allowed.");
         
    }

    public async Task Update(Operation entity)
    {
        var employeeInfo = await _repositoryEmployeeBioData.GetAsync(entity.SupervisorId);
        entity.SupervisorName = !string.IsNullOrEmpty(employeeInfo.OtherName) ? $"{employeeInfo.FirstName}  {employeeInfo.LastName} {employeeInfo.OtherName}" : $"{employeeInfo.FirstName}  {employeeInfo.LastName}" ;
        entity.SupervisorEmail = employeeInfo.CompanyEmail;
        entity.SupervisorPhone = employeeInfo.PrimaryPhoneNumber;
        entity.SupervisorDepartment = employeeInfo.DepartmentName;
        
        await _repositoryOperation.UpdateAsync(entity);
    }

    public async Task<object> GetAsync(Guid id)
    {
        return await _repositoryOperation.GetAsync(id);
    }

    public async Task<object> ListAll(int pageNo, int pageSize, string query)
    {
        
        var predicate = PredicateBuilder.New<Operation>();

        predicate.And(x => !x.IsDeleted);
            
        if (!string.IsNullOrEmpty(query))
        {
            predicate.And(x => x.Name.ToLower().Contains(query.ToLower()));
        }
            
        var totalCount =
            await _repositoryOperation.CountAsync(predicate);
            
        var data = await (await _repositoryOperation.GetAllAsync()).Where(predicate).Select(a=> new { a.Id, a.Name, a.Description, a.WorkCenterName, a.WorkCenterId, a.ProductionOrderId, a.JobTicketIdentifier, a.EndTime, a.StartTime, a.EstimatedDurationInHours, a.SupervisorDepartment, a.CreationTime, a.SupervisorEmail, a.SupervisorId, a.SupervisorName,  }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

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
        await _repositoryOperation.DeleteAsync(id);
    }
}