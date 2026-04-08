namespace RhyoliteOrbit.Models.Dms;

public class StepAssignee
{
    public RhyoliteOrbitConsts.AssigneeType Type { get; set; }
    public string Value { get; set; }  // UserId, RoleName, DepartmentId
    public bool IsRequired { get; set; }
    public int? OrderIndex { get; set; }  // For sequential assignments
}