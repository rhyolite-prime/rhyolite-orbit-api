using System.Collections.Generic;

namespace RhyoliteOrbit.Models.Dms;

public class WorkflowTemplateStep
{
    public int StepIndex { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public RhyoliteOrbitConsts.WorkflowStepType Type { get; set; }
    public List<StepAssignee> Assignees { get; set; }
    public RhyoliteOrbitConsts.CompletionRule CompletionRule { get; set; }
    public int? TimeoutHours { get; set; }
    public List<StepCondition> Conditions { get; set; }
    public List<StepAction> Actions { get; set; }
    public bool IsMandatory { get; set; }
    public int? NextStepOnReject { get; set; }
    public Dictionary<string, object> CustomProperties { get; set; }
}