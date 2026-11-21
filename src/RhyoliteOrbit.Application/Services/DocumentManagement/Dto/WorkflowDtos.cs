using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.DocumentManagement.Dto;

// ---------------------------------------------------------------- workflow
public class WorkflowTemplateDto: EntityDto<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }

    public List<WorkflowTemplateStepDto> Steps { get; set; } = new();

    public bool IsActive { get; set; }

    public int Version { get; set; }

    public DateTime CreationTime { get; set; }
}

public class WorkflowTemplateStepDto
{
    public int StepIndex { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.WorkflowStepType"/>.</summary>
    public int Type { get; set; }

    public List<StepAssigneeDto> Assignees { get; set; } = new();

    /// <summary>See <see cref="RhyoliteOrbitConsts.CompletionRule"/>.</summary>
    public int CompletionRule { get; set; }

    public int? TimeoutHours { get; set; }

    public List<StepConditionDto> Conditions { get; set; } = new();

    public List<StepActionDto> Actions { get; set; } = new();

    public bool IsMandatory { get; set; }

    public int? NextStepOnReject { get; set; }
}

public class StepAssigneeDto
{
    /// <summary>See <see cref="RhyoliteOrbitConsts.AssigneeType"/>.</summary>
    public int Type { get; set; }

    /// <summary>UserId, role name, department id or a dynamic expression.</summary>
    public string Value { get; set; }

    public bool IsRequired { get; set; }

    public int? OrderIndex { get; set; }
}

public class StepConditionDto
{
    public string Name { get; set; }

    public string Expression { get; set; }

    public int NextStepIndex { get; set; }

    public string Description { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.ConditionField"/>.</summary>
    public int Field { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.ConditionOperator"/>.</summary>
    public int Operator { get; set; }

    public string Value { get; set; }
}

public class StepActionDto
{
    /// <summary>See <see cref="RhyoliteOrbitConsts.StepActionType"/>.</summary>
    public int Type { get; set; }

    public string Name { get; set; }

    public bool IsAutomatic { get; set; }

    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class CreateWorkflowTemplateDto
{
    public string Name { get; set; }

    public string Description { get; set; }

    public List<WorkflowTemplateStepDto> Steps { get; set; } = new();

    public bool IsActive { get; set; } = true;
}

public class UpdateWorkflowTemplateDto: EntityDto<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }

    public List<WorkflowTemplateStepDto> Steps { get; set; } = new();

    public bool IsActive { get; set; } = true;
}

public class WorkflowInstanceDto: EntityDto<Guid>
{
    public Guid TemplateId { get; set; }

    public string TemplateName { get; set; }

    public Guid DocumentId { get; set; }

    public string DocumentName { get; set; }

    public int CurrentStepIndex { get; set; }

    public string Status { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public long StartedById { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? LastActionAt { get; set; }
}

public class WorkflowTaskDto: EntityDto<Guid>
{
    public Guid WorkflowInstanceId { get; set; }

    public Guid DocumentId { get; set; }

    public string DocumentName { get; set; }

    public long AssignedTo { get; set; }

    public string AssignedToName { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.WorkflowTaskType"/>.</summary>
    public int TaskType { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.WorkflowTaskStatus"/>.</summary>
    public int Status { get; set; }

    public string Comments { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int StepIndex { get; set; }

    public string StepName { get; set; }

    public DateTime? DueDate { get; set; }

    public long? DelegatedFrom { get; set; }
}

public class StartWorkflowDto
{
    public Guid DocumentId { get; set; }

    public Guid TemplateId { get; set; }
}

public class CompleteWorkflowTaskDto
{
    public Guid TaskId { get; set; }

    /// <summary>See <see cref="RhyoliteOrbitConsts.WorkflowTaskStatus"/>.</summary>
    public int Outcome { get; set; }

    public string Comment { get; set; }
}

public class DelegateWorkflowTaskDto
{
    public Guid TaskId { get; set; }

    public long ToUserId { get; set; }
}

public class CancelWorkflowDto
{
    public Guid InstanceId { get; set; }

    public string Reason { get; set; }
}

public class WorkflowInstanceListInput: PagedAndSortedResultRequestDto
{
    public Guid? DocumentId { get; set; }

    public string Status { get; set; }
}
