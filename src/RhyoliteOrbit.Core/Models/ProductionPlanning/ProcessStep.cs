using System;

namespace RhyoliteOrbit.Models.ProductionPlanning;

public class ProcessStep
{
    public Guid Id { get; set; }
    public int SequenceNumber { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid WorkCenterId { get; set; }
    public string WorkCenterName { get; set; }
    public decimal EstimatedDurationInHours { get; set; }
    public bool IsMandatory { get; set; }
    // Manager properties: from employees
    // Not everyone in the company will be a user on the portal/app
    public Guid ManagerId { get; set; }
    public string ManagerName { get; set; }
    public string ManagerEmail { get; set; }
    public string ManagerDepartment { get; set; }
    public string ManagerPhone { get; set; }
}