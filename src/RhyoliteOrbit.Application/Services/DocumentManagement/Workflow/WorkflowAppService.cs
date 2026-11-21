using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RhyoliteOrbit.Authorization;
using RhyoliteOrbit.DomainServices.Dms.Acl;
using RhyoliteOrbit.DomainServices.Dms.Audit;
using RhyoliteOrbit.DomainServices.Dms.Documents;
using RhyoliteOrbit.DomainServices.Dms.Workflow;
using RhyoliteOrbit.Models.Dms;
using RhyoliteOrbit.Services.DocumentManagement.Dto;

namespace RhyoliteOrbit.Services.DocumentManagement.Workflow;

/// <summary>Document approval workflows: templates, instances and tasks.</summary>
[AbpAuthorize]
public class WorkflowAppService: DmsAppServiceBase
{
    private readonly IDmsWorkflowEngine _engine;
    private readonly IMapper _mapper;

    public WorkflowAppService(
        IDmsWorkflowEngine engine,
        IAclManager aclManager,
        IDocumentManager documentManager,
        DomainServices.Dms.Folders.IFolderManager folderManager,
        IAuditTrail auditTrail,
        IRepository<Authorization.Users.UserRole, long> userRoleRepository,
        IRepository<Authorization.Roles.Role, int> roleRepository,
        IMapper mapper)
        : base(aclManager, documentManager, folderManager, auditTrail, userRoleRepository, roleRepository)
    {
        _engine = engine;
        _mapper = mapper;
    }

    // ---------------------------------------------------------------- templates

    /// <summary>Lists the workflow templates.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageWorkflows)]
    [HttpGet]
    public async Task<ListResultDto<WorkflowTemplateDto>> GetTemplates(bool onlyActive = true)
    {
        var templates = await _engine.ListTemplatesAsync(onlyActive);
        return new ListResultDto<WorkflowTemplateDto>(_mapper.Map<List<WorkflowTemplateDto>>(templates));
    }

    /// <summary>Gets a workflow template.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageWorkflows)]
    [HttpGet]
    public async Task<WorkflowTemplateDto> GetTemplate(Guid id)
    {
        var template = await _engine.GetTemplateAsync(id);
        return _mapper.Map<WorkflowTemplateDto>(template);
    }

    /// <summary>Creates a workflow template.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageWorkflows)]
    [HttpPost]
    public async Task<WorkflowTemplateDto> CreateTemplate(CreateWorkflowTemplateDto input)
    {
        var template = new DmsWorkflowTemplate
        {
            TenantId = CurrentTenantId,
            Name = input.Name,
            Description = input.Description,
            IsActive = input.IsActive,
            Version = 1,
            WorkflowTemplateSteps = _mapper.Map<List<WorkflowTemplateStep>>(input.Steps ?? new List<WorkflowTemplateStepDto>()),
            CreatorUserId = CurrentUserId
        };

        var created = await _engine.CreateTemplateAsync(template);
        return _mapper.Map<WorkflowTemplateDto>(created);
    }

    /// <summary>Updates a workflow template.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageWorkflows)]
    [HttpPut]
    public async Task<WorkflowTemplateDto> UpdateTemplate(UpdateWorkflowTemplateDto input)
    {
        var template = new DmsWorkflowTemplate
        {
            Id = input.Id,
            TenantId = CurrentTenantId,
            Name = input.Name,
            Description = input.Description,
            IsActive = input.IsActive,
            WorkflowTemplateSteps = _mapper.Map<List<WorkflowTemplateStep>>(input.Steps ?? new List<WorkflowTemplateStepDto>()),
            LastModifierUserId = CurrentUserId
        };

        var updated = await _engine.UpdateTemplateAsync(template);
        return _mapper.Map<WorkflowTemplateDto>(updated);
    }

    /// <summary>Deletes a workflow template.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageWorkflows)]
    [HttpDelete]
    public async Task DeleteTemplate(Guid id)
    {
        await _engine.DeleteTemplateAsync(id);
    }

    /// <summary>Validates a workflow template definition.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageWorkflows)]
    [HttpPost]
    public async Task ValidateTemplate(UpdateWorkflowTemplateDto input)
    {
        var template = new DmsWorkflowTemplate
        {
            Name = input.Name,
            WorkflowTemplateSteps = _mapper.Map<List<WorkflowTemplateStep>>(input.Steps ?? new List<WorkflowTemplateStepDto>())
        };

        await _engine.ValidateTemplateAsync(template);
    }

    // --------------------------------------------------------------- instances

    /// <summary>Starts a workflow for a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_StartWorkflow)]
    [HttpPost]
    public async Task<WorkflowInstanceDto> Start(StartWorkflowDto input)
    {
        await EnsureDocumentPermissionAsync(input.DocumentId, RhyoliteOrbitConsts.DmsPermission.ManageWorkflow);

        var instance = await _engine.StartAsync(input.DocumentId, input.TemplateId, CurrentUserId);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.WorkflowStarted, input.DocumentId, null,
            System.Text.Json.JsonSerializer.Serialize(new { instance.TemplateName }), CurrentUserId);

        return _mapper.Map<WorkflowInstanceDto>(instance);
    }

    /// <summary>Gets a workflow instance.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<WorkflowInstanceDto> GetInstance(Guid id)
    {
        var instance = await _engine.GetInstanceAsync(id);
        return _mapper.Map<WorkflowInstanceDto>(instance);
    }

    /// <summary>Lists the workflow instances.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<PagedResultDto<WorkflowInstanceDto>> GetInstances(WorkflowInstanceListInput input)
    {
        var pageNo = input.MaxResultCount == 0 ? 1 : (input.SkipCount / Math.Max(input.MaxResultCount, 1)) + 1;
        var pageSize = input.MaxResultCount == 0 ? 20 : input.MaxResultCount;

        var instances = await _engine.ListInstancesAsync(input.DocumentId, input.Status, pageNo, pageSize);

        return new PagedResultDto<WorkflowInstanceDto>(
            instances.Count,
            _mapper.Map<List<WorkflowInstanceDto>>(instances));
    }

    /// <summary>Cancels a running workflow.</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageWorkflows)]
    [HttpPost]
    public async Task Cancel(CancelWorkflowDto input)
    {
        var instance = await _engine.GetInstanceAsync(input.InstanceId);
        await EnsureDocumentPermissionAsync(instance.DocumentId, RhyoliteOrbitConsts.DmsPermission.ManageWorkflow);

        await _engine.CancelAsync(input.InstanceId, CurrentUserId, input.Reason);

        await AuditTrail.RecordAsync(
            RhyoliteOrbitConsts.AuditAction.WorkflowCancelled, instance.DocumentId, null, input.Reason, CurrentUserId);
    }

    // ------------------------------------------------------------------- tasks

    /// <summary>Lists the workflow tasks assigned to the current user.</summary>
    [AbpAuthorize(PermissionNames.DMS_ParticipateInWorkflow)]
    [HttpGet]
    public async Task<ListResultDto<WorkflowTaskDto>> GetMyTasks(bool includeCompleted = false)
    {
        var tasks = await _engine.GetTasksForUserAsync(CurrentUserId, includeCompleted);
        return new ListResultDto<WorkflowTaskDto>(_mapper.Map<List<WorkflowTaskDto>>(tasks));
    }

    /// <summary>Lists the workflow tasks of a document.</summary>
    [AbpAuthorize(PermissionNames.DMS_ViewDocuments)]
    [HttpGet]
    public async Task<ListResultDto<WorkflowTaskDto>> GetDocumentTasks(Guid documentId)
    {
        await EnsureDocumentPermissionAsync(documentId, RhyoliteOrbitConsts.DmsPermission.View);
        var tasks = await _engine.GetTasksForDocumentAsync(documentId);
        return new ListResultDto<WorkflowTaskDto>(_mapper.Map<List<WorkflowTaskDto>>(tasks));
    }

    /// <summary>Gets a single workflow task.</summary>
    [AbpAuthorize(PermissionNames.DMS_ParticipateInWorkflow)]
    [HttpGet]
    public async Task<WorkflowTaskDto> GetTask(Guid id)
    {
        var task = await _engine.GetTaskAsync(id);
        return _mapper.Map<WorkflowTaskDto>(task);
    }

    /// <summary>Completes a workflow task (approve, reject, request changes).</summary>
    [AbpAuthorize(PermissionNames.DMS_ParticipateInWorkflow)]
    [HttpPost]
    public async Task CompleteTask(CompleteWorkflowTaskDto input)
    {
        var task = await _engine.GetTaskAsync(input.TaskId);

        if (task.AssignedTo != CurrentUserId)
        {
            throw new UserFriendlyException("DmsTaskNotAssigned", "This task is assigned to another user.");
        }

        await _engine.CompleteTaskAsync(
            input.TaskId,
            (RhyoliteOrbitConsts.WorkflowTaskStatus)input.Outcome,
            input.Comment,
            CurrentUserId);

        var action = (RhyoliteOrbitConsts.WorkflowTaskStatus)input.Outcome == RhyoliteOrbitConsts.WorkflowTaskStatus.Rejected
            ? RhyoliteOrbitConsts.AuditAction.WorkflowRejected
            : RhyoliteOrbitConsts.AuditAction.WorkflowTaskCompleted;

        await AuditTrail.RecordAsync(
            action, null, null, input.Comment, CurrentUserId);
    }

    /// <summary>Delegates a workflow task to another user.</summary>
    [AbpAuthorize(PermissionNames.DMS_ParticipateInWorkflow)]
    [HttpPost]
    public async Task DelegateTask(DelegateWorkflowTaskDto input)
    {
        var task = await _engine.GetTaskAsync(input.TaskId);

        if (task.AssignedTo != CurrentUserId)
        {
            throw new UserFriendlyException("DmsTaskNotAssigned", "This task is assigned to another user.");
        }

        await _engine.DelegateTaskAsync(input.TaskId, input.ToUserId, CurrentUserId);
    }

    /// <summary>Runs the escalation/timeout sweep (also invoked by the scheduler).</summary>
    [AbpAuthorize(PermissionNames.DMS_ManageWorkflows)]
    [HttpPost]
    public async Task<int> ProcessEscalations()
    {
        return await _engine.ProcessEscalationsAsync();
    }
}
