using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class ProductionOrder: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(100)]
    public string Title { get; set; }
    [MaxLength(100)]
    public string Tag { get; set; }
    public bool IsApproved { get; set; }    
    public DateTime? ApprovalDate { get; set; }
    [MaxLength(20)]
    public string ProductionOrderIdentifier { get; set; }
    
    public Guid ProductId { get; set; }
    [MaxLength(200)]
    public string ProductName { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal ActualQuantity { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime ActualStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime ActualEndDate { get; set; }
    public decimal EstimatedBudget { get; set; }
    public int PriorityLevel { get; set; } // Priority enum
    public int Status { get; set; } // ProductionOrder status enum
    // Manager properties : from employees
    // Not everyone in the company will be a user on the portal/app
    public Guid ManagerId { get; set; }
    [MaxLength(130)]
    public string ManagerName { get; set; }
    [MaxLength(100)]
    public string ManagerEmail { get; set; }
    [MaxLength(150)]
    public string ManagerDepartment { get; set; }
    [MaxLength(20)]
    public string ManagerPhone { get; set; }
    
    [MaxLength(150)]
    public string Comments { get; set; }
    public int TenantId { get; set; }
}