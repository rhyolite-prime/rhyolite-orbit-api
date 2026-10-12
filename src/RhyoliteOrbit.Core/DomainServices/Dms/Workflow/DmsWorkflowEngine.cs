using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RhyoliteOrbit.Models.Dms;
using RhyoliteOrbit.Notifications;

namespace RhyoliteOrbit.DomainServices.Dms.Workflow;

public interface IDmsWorkflowEngine
{
    // templates
    Task<DmsWorkflowTemplate> CreateTemplateAsync(DmsWorkflowTemplate template);

    Task<DmsWorkflowTemplate> UpdateTemplateAsync(DmsWorkflowTemplate template);

    Task DeleteTemplateAsync(Guid templateId);

    Task<List<DmsWorkflowTemplate>> ListTemplatesAsync(bool onlyActive = true);

    Task<DmsWorkflowTemplate> GetTemplateAsync(Guid templateId);

    Task ValidateTemplateAsync(DmsWorkflowTemplate template);

    // instances
    Task<DmsWorkflowInstance> StartAsync(Guid documentId, Guid templateId, long startedBy);

    Task<DmsWorkflowInstance> GetInstanceAsync(Guid instanceId);

    Task<List<DmsWorkflowInstance>> ListInstancesAsync(Guid? documentId, string status, int pageNo, int pageSize);

    Task CancelAsync(Guid instanceId, long cancelledBy, string reason);

    // tasks
    Task<List<DmsWorkflowTask>> GetTasksForUserAsync(long userId, bool includeCompleted = false);

    Task<List<DmsWorkflowTask>> GetTasksForDocumentAsync(Guid documentId);

    Task<DmsWorkflowTask> GetTaskAsync(Guid taskId);

    Task CompleteTaskAsync(Guid taskId, RhyoliteOrbitConsts.WorkflowTaskStatus outcome, string comment, long userId);

    Task DelegateTaskAsync(Guid taskId, long toUserId, long byUserId);

    /// <summary>Advances tasks that passed their timeout (escalation steps or reminders).</summary>
    Task<int> ProcessEscalationsAsync();
}

/// <summary>
/// Document scoped workflow engine: instantiates templates, routes tasks to
/// assignees, evaluates conditions, runs step actions and handles timeouts.
/// </summary>
public class DmsWorkflowEngine: DomainService, IDmsWorkflowEngine
{
    private readonly IRepository<DmsWorkflowTemplate, Guid> _templateRepository;
    private readonly IRepository<DmsWorkflowInstance, Guid> _instanceRepository;
    private readonly IRepository<DmsWorkflowTask, Guid> _taskRepository;
    private readonly IRepository<DmsDocument, Guid> _documentRepository;
    private readonly IRepository<DmsDocumentMetadataValues, Guid> _metadataRepository;
    private readonly IRepository<DmsComment, Guid> _commentRepository;
    private readonly IConditionEvaluator _conditionEvaluator;
    private readonly IAbpSession _abpSession;
    private readonly RhyoliteOrbit.Authorization.Users.UserManager _userManager;
    private readonly ICustomEmailSender _emailSender;

    public DmsWorkflowEngine(
        IRepository<DmsWorkflowTemplate, Guid> templateRepository,
        IRepository<DmsWorkflowInstance, Guid> instanceRepository,
        IRepository<DmsWorkflowTask, Guid> taskRepository,
        IRepository<DmsDocument, Guid> documentRepository,
        IRepository<DmsDocumentMetadataValues, Guid> metadataRepository,
        IRepository<DmsComment, Guid> commentRepository,
        IConditionEvaluator conditionEvaluator,
        IAbpSession abpSession,
        RhyoliteOrbit.Authorization.Users.UserManager userManager,
        ICustomEmailSender emailSender)
    {
        _templateRepository = templateRepository;
        _instanceRepository = instanceRepository;
        _taskRepository = taskRepository;
        _documentRepository = documentRepository;
        _metadataRepository = metadataRepository;
        _commentRepository = commentRepository;
        _conditionEvaluator = conditionEvaluator;
        _abpSession = abpSession;
        _userManager = userManager;
        _emailSender = emailSender;
    }

    // ---------------------------------------------------------------- templates

    public async Task<DmsWorkflowTemplate> CreateTemplateAsync(DmsWorkflowTemplate template)
    {
        await ValidateTemplateAsync(template);

        template.TenantId = _abpSession.TenantId ?? 0;
        template.IsActive = true;
        template.Version = 1;

        return await _templateRepository.InsertAsync(template);
    }

    public async Task<DmsWorkflowTemplate> UpdateTemplateAsync(DmsWorkflowTemplate template)
    {
        var existing = await GetTemplateAsync(template.Id);
        if (existing == null)
        {
            throw new UserFriendlyException("The workflow template was not found.");
        }

        await ValidateTemplateAsync(template);

        existing.Name = template.Name;
        existing.Description = template.Description;
        existing.WorkflowTemplateSteps = template.WorkflowTemplateSteps;
        existing.IsActive = template.IsActive;
        existing.Version += 1;

        return await _templateRepository.UpdateAsync(existing);
    }

    public async Task DeleteTemplateAsync(Guid templateId)
    {
        await _templateRepository.HardDeleteAsync(templateId);
    }

    public async Task<List<DmsWorkflowTemplate>> ListTemplatesAsync(bool onlyActive = true)
    {
        var query = await _templateRepository.GetAllAsync();

        if (onlyActive)
        {
            query = query.Where(t => t.IsActive);
        }

        return await query.OrderBy(t => t.Name).ToListAsync();
    }

    public async Task<DmsWorkflowTemplate> GetTemplateAsync(Guid templateId)
    {
        return await _templateRepository.FirstOrDefaultAsync(templateId);
    }

    public async Task ValidateTemplateAsync(DmsWorkflowTemplate template)
    {
        var steps = template.WorkflowTemplateSteps ?? new List<WorkflowTemplateStep>();

        if (!steps.Any())
        {
            throw new UserFriendlyException(
                "DmsWorkflowInvalidSteps",
                "The workflow template must contain at least one step.");
        }

        if (steps.Select(s => s.StepIndex).Distinct().Count() != steps.Count)
        {
            throw new UserFriendlyException("Workflow step indexes must be unique.");
        }

        await Task.CompletedTask;
    }

    // ---------------------------------------------------------------- instances

    public async Task<DmsWorkflowInstance> StartAsync(Guid documentId, Guid templateId, long startedBy)
    {
        var template = await GetTemplateAsync(templateId);
        if (template == null || !template.IsActive)
        {
            throw new UserFriendlyException(
                "DmsWorkflowNotActive",
                "The selected workflow template is not active.");
        }

        var document = await _documentRepository.FirstOrDefaultAsync(documentId);
        if (document == null)
        {
            throw new UserFriendlyException("DmsDocumentNotFound", "The requested document was not found.");
        }

        var steps = (template.WorkflowTemplateSteps ?? new List<WorkflowTemplateStep>())
            .OrderBy(s => s.StepIndex)
            .ToList();

        var instance = new DmsWorkflowInstance
        {
            TenantId = _abpSession.TenantId ?? 0,
            TemplateId = template.Id,
            TemplateName = template.Name,
            DocumentId = documentId,
            CurrentStepIndex = steps.First().StepIndex,
            Status = ((int)RhyoliteOrbitConsts.WorkflowInstanceStatus.InProgress).ToString(),
            StartDate = DateTime.UtcNow,
            StartedById = startedBy,
            LastActionAt = DateTime.UtcNow,
            TemplateSnapshotJson = JsonConvert.SerializeObject(steps),
            VariablesJson = JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                ["documentId"] = documentId.ToString(),
                ["startedBy"] = startedBy.ToString()
            }),
            CreatorUserId = startedBy
        };

        instance = await _instanceRepository.InsertAsync(instance);

        await EnterStepAsync(instance, steps.First());

        return instance;
    }

    public async Task<DmsWorkflowInstance> GetInstanceAsync(Guid instanceId)
    {
        return await _instanceRepository.FirstOrDefaultAsync(instanceId);
    }

    public async Task<List<DmsWorkflowInstance>> ListInstancesAsync(Guid? documentId, string status, int pageNo, int pageSize)
    {
        if (pageNo < 1)
        {
            pageNo = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 20;
        }

        var query = await _instanceRepository.GetAllAsync();

        if (documentId.HasValue)
        {
            query = query.Where(i => i.DocumentId == documentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(i => i.Status == status);
        }

        return await query
            .OrderByDescending(i => i.StartDate)
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task CancelAsync(Guid instanceId, long cancelledBy, string reason)
    {
        var instance = await GetInstanceAsync(instanceId);
        if (instance == null)
        {
            throw new UserFriendlyException("The workflow instance was not found.");
        }

        instance.Status = ((int)RhyoliteOrbitConsts.WorkflowInstanceStatus.Cancelled).ToString();
        instance.EndDate = DateTime.UtcNow;
        instance.LastActionAt = DateTime.UtcNow;

        await _instanceRepository.UpdateAsync(instance);

        var openTasks = await (await _taskRepository.GetAllAsync())
            .Where(t => t.WorkflowInstanceId == instanceId && t.Status == RhyoliteOrbitConsts.WorkflowTaskStatus.Pending)
            .ToListAsync();

        foreach (var task in openTasks)
        {
            task.Status = RhyoliteOrbitConsts.WorkflowTaskStatus.Skipped;
            task.CompletedAt = DateTime.UtcNow;
            task.Comments = reason;
            await _taskRepository.UpdateAsync(task);
        }
    }

    // ---------------------------------------------------------------- tasks

    public async Task<List<DmsWorkflowTask>> GetTasksForUserAsync(long userId, bool includeCompleted = false)
    {
        var query = (await _taskRepository.GetAllAsync())
            .Where(t => t.AssignedTo == userId);

        if (!includeCompleted)
        {
            query = query.Where(t => t.Status == RhyoliteOrbitConsts.WorkflowTaskStatus.Pending);
        }

        return await query
            .OrderBy(t => t.DueDate ?? DateTime.MaxValue)
            .ThenByDescending(t => t.CreationTime)
            .ToListAsync();
    }

    public async Task<List<DmsWorkflowTask>> GetTasksForDocumentAsync(Guid documentId)
    {
        var instanceIds = await (await _instanceRepository.GetAllAsync())
            .Where(i => i.DocumentId == documentId)
            .Select(i => i.Id)
            .ToListAsync();

        return await (await _taskRepository.GetAllAsync())
            .Where(t => instanceIds.Contains(t.WorkflowInstanceId))
            .OrderBy(t => t.StepIndex)
            .ToListAsync();
    }

    public async Task<DmsWorkflowTask> GetTaskAsync(Guid taskId)
    {
        return await _taskRepository.FirstOrDefaultAsync(taskId);
    }

    public async Task CompleteTaskAsync(
        Guid taskId,
        RhyoliteOrbitConsts.WorkflowTaskStatus outcome,
        string comment,
        long userId)
    {
        var task = await GetTaskAsync(taskId);
        if (task == null)
        {
            throw new UserFriendlyException("The workflow task was not found.");
        }

        if (task.Status != RhyoliteOrbitConsts.WorkflowTaskStatus.Pending)
        {
            throw new UserFriendlyException("DmsWorkflowAlreadyCompleted", "This workflow has already been completed.");
        }

        if (task.AssignedTo != userId)
        {
            throw new UserFriendlyException("DmsWorkflowTaskNotAssigned", "This task is not assigned to you.");
        }

        var instance = await GetInstanceAsync(task.WorkflowInstanceId);
        if (instance == null || instance.Status != ((int)RhyoliteOrbitConsts.WorkflowInstanceStatus.InProgress).ToString())
        {
            throw new UserFriendlyException("DmsWorkflowAlreadyCompleted", "This workflow has already been completed.");
        }

        task.Status = outcome;
        task.Comments = comment;
        task.CompletedAt = DateTime.UtcNow;
        task.OutcomeJson = JsonConvert.SerializeObject(new { outcome = outcome.ToString(), comment });

        await _taskRepository.UpdateAsync(task);

        if (!string.IsNullOrWhiteSpace(comment))
        {
            await _commentRepository.InsertAsync(new DmsComment
            {
                TenantId = instance.TenantId,
                DocumentId = instance.DocumentId,
                VersionId = null,
                CommentText = comment,
                CommentType = outcome == RhyoliteOrbitConsts.WorkflowTaskStatus.Rejected ? "Resolution" : "Suggestion"
            });
        }

        await EvaluateStepCompletionAsync(instance, task, outcome);
    }

    public async Task DelegateTaskAsync(Guid taskId, long toUserId, long byUserId)
    {
        var task = await GetTaskAsync(taskId);
        if (task == null)
        {
            throw new UserFriendlyException("The workflow task was not found.");
        }

        task.DelegatedFrom = byUserId;
        task.AssignedTo = toUserId;

        await _taskRepository.UpdateAsync(task);
    }

    public async Task<int> ProcessEscalationsAsync()
    {
        var now = DateTime.UtcNow;

        var overdue = await (await _taskRepository.GetAllAsync())
            .Where(t => t.Status == RhyoliteOrbitConsts.WorkflowTaskStatus.Pending
                        && t.DueDate != null
                        && t.DueDate < now)
            .ToListAsync();

        foreach (var task in overdue)
        {
            var instance = await GetInstanceAsync(task.WorkflowInstanceId);
            if (instance == null)
            {
                continue;
            }

            var steps = DeserializeSteps(instance);
            var step = steps.FirstOrDefault(s => s.StepIndex == task.StepIndex);

            if (step?.TimeoutHours == null)
            {
                continue;
            }

            // notify the assignee and the workflow owner
            try
            {
                await _emailSender.SendEmailAsync(
                    string.Empty,
                    "Workflow task overdue",
                    $"Task '{step.Name}' on document {instance.DocumentId} is overdue.");
            }
            catch (Exception ex)
            {
                Logger.Warn($"Could not send escalation notification: {ex.Message}");
            }
        }

        return overdue.Count;
    }

    // ---------------------------------------------------------------- internals

    private async Task EnterStepAsync(DmsWorkflowInstance instance, WorkflowTemplateStep step)
    {
        instance.CurrentStepIndex = step.StepIndex;
        instance.LastActionAt = DateTime.UtcNow;

        await _instanceRepository.UpdateAsync(instance);

        if (step.Type == RhyoliteOrbitConsts.WorkflowStepType.Notification)
        {
            // informational step: no task, move on immediately
            await RunStepActionsAsync(instance, step);
            await AdvanceAsync(instance, step, null);
            return;
        }

        if (step.Type == RhyoliteOrbitConsts.WorkflowStepType.Condition)
        {
            var context = await BuildContextAsync(instance, null);
            var branch = (step.Conditions ?? new List<StepCondition>())
                .FirstOrDefault(c => _conditionEvaluator.Evaluate(c, context));

            var nextIndex = branch?.NextStepIndex ?? step.NextStepOnReject ?? -1;

            await RunStepActionsAsync(instance, step);

            if (nextIndex >= 0)
            {
                var steps = DeserializeSteps(instance);
                var next = steps.FirstOrDefault(s => s.StepIndex == nextIndex);

                if (next != null)
                {
                    await EnterStepAsync(instance, next);
                    return;
                }
            }

            await CompleteInstanceAsync(instance);
            return;
        }

        var assignees = await ResolveAssigneesAsync(instance, step);

        if (!assignees.Any())
        {
            Logger.Warn($"Workflow step {step.StepIndex} of instance {instance.Id} has no assignees; skipping.");
            await AdvanceAsync(instance, step, null);
            return;
        }

        foreach (var assignee in assignees.Distinct())
        {
            await _taskRepository.InsertAsync(new DmsWorkflowTask
            {
                TenantId = instance.TenantId,
                WorkflowInstanceId = instance.Id,
                AssignedTo = assignee,
                TaskType = MapTaskType(step.Type),
                Status = RhyoliteOrbitConsts.WorkflowTaskStatus.Pending,
                StepIndex = step.StepIndex,
                StepName = step.Name,
                DueDate = step.TimeoutHours.HasValue ? DateTime.UtcNow.AddHours(step.TimeoutHours.Value) : (DateTime?)null
            });
        }
    }

    private async Task EvaluateStepCompletionAsync(
        DmsWorkflowInstance instance,
        DmsWorkflowTask completedTask,
        RhyoliteOrbitConsts.WorkflowTaskStatus outcome)
    {
        var steps = DeserializeSteps(instance);
        var step = steps.FirstOrDefault(s => s.StepIndex == completedTask.StepIndex);

        if (step == null)
        {
            return;
        }

        if (outcome == RhyoliteOrbitConsts.WorkflowTaskStatus.Rejected ||
            outcome == RhyoliteOrbitConsts.WorkflowTaskStatus.RequestChanges)
        {
            if (step.NextStepOnReject.HasValue)
            {
                var rejectStep = steps.FirstOrDefault(s => s.StepIndex == step.NextStepOnReject.Value);

                if (rejectStep != null)
                {
                    await CancelPendingTasksAsync(instance.Id);
                    await EnterStepAsync(instance, rejectStep);
                    return;
                }
            }

            instance.Status = ((int)RhyoliteOrbitConsts.WorkflowInstanceStatus.Rejected).ToString();
            instance.EndDate = DateTime.UtcNow;
            await _instanceRepository.UpdateAsync(instance);

            await CancelPendingTasksAsync(instance.Id);
            return;
        }

        var pending = await (await _taskRepository.GetAllAsync())
            .Where(t => t.WorkflowInstanceId == instance.Id
                        && t.StepIndex == step.StepIndex
                        && t.Status == RhyoliteOrbitConsts.WorkflowTaskStatus.Pending)
            .AnyAsync();

        if (pending)
        {
            return; // wait for the remaining assignees
        }

        await RunStepActionsAsync(instance, step);
        await AdvanceAsync(instance, step, completedTask.AssignedTo);
    }

    private async Task AdvanceAsync(DmsWorkflowInstance instance, WorkflowTemplateStep step, long? actorId)
    {
        var steps = DeserializeSteps(instance);

        var next = steps
            .Where(s => s.StepIndex > step.StepIndex)
            .OrderBy(s => s.StepIndex)
            .FirstOrDefault();

        if (next == null)
        {
            await CompleteInstanceAsync(instance);
            return;
        }

        await EnterStepAsync(instance, next);
    }

    private async Task CompleteInstanceAsync(DmsWorkflowInstance instance)
    {
        instance.Status = ((int)RhyoliteOrbitConsts.WorkflowInstanceStatus.Completed).ToString();
        instance.EndDate = DateTime.UtcNow;
        instance.LastActionAt = DateTime.UtcNow;

        await _instanceRepository.UpdateAsync(instance);
    }

    private async Task CancelPendingTasksAsync(Guid instanceId)
    {
        var pending = await (await _taskRepository.GetAllAsync())
            .Where(t => t.WorkflowInstanceId == instanceId && t.Status == RhyoliteOrbitConsts.WorkflowTaskStatus.Pending)
            .ToListAsync();

        foreach (var task in pending)
        {
            task.Status = RhyoliteOrbitConsts.WorkflowTaskStatus.Skipped;
            task.CompletedAt = DateTime.UtcNow;
            await _taskRepository.UpdateAsync(task);
        }
    }

    private async Task<List<long>> ResolveAssigneesAsync(DmsWorkflowInstance instance, WorkflowTemplateStep step)
    {
        var assignees = new List<long>();

        foreach (var assignee in step.Assignees ?? new List<StepAssignee>())
        {
            switch (assignee.Type)
            {
                case RhyoliteOrbitConsts.AssigneeType.SpecificUser:
                    if (long.TryParse(assignee.Value, out var userId))
                    {
                        assignees.Add(userId);
                    }

                    break;

                case RhyoliteOrbitConsts.AssigneeType.Role:
                    try
                    {
                        var users = await _userManager.GetUsersInRoleAsync(assignee.Value);
                        assignees.AddRange(users.Select(u => u.Id));
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn($"Could not resolve role '{assignee.Value}': {ex.Message}");
                    }

                    break;

                case RhyoliteOrbitConsts.AssigneeType.DocumentCreator:
                    assignees.Add(instance.StartedById);
                    break;

                case RhyoliteOrbitConsts.AssigneeType.ManagerOfCreator:
                    assignees.Add(await ResolveTenantAdminAsync());
                    break;

                case RhyoliteOrbitConsts.AssigneeType.Dynamic:
                    var context = await BuildContextAsync(instance, null);

                    if (context.Metadata != null && context.Metadata.TryGetValue(new Guid(assignee.Value), out var dynamicUserIdText)
                        && long.TryParse(dynamicUserIdText, out var dynamicUserId))
                    {
                        assignees.Add(dynamicUserId);
                    }

                    break;

                case RhyoliteOrbitConsts.AssigneeType.Department:
                    assignees.Add(await ResolveTenantAdminAsync());
                    break;
            }
        }

        return assignees;
    }

    private async Task<long> ResolveTenantAdminAsync()
    {
        var admins = await _userManager.GetUsersInRoleAsync("Admin");

        return admins.FirstOrDefault()?.Id ?? 0;
    }

    private async Task RunStepActionsAsync(DmsWorkflowInstance instance, WorkflowTemplateStep step)
    {
        foreach (var action in step.Actions ?? new List<StepAction>())
        {
            try
            {
                await ExecuteActionAsync(instance, action);
            }
            catch (Exception ex)
            {
                Logger.Warn($"Workflow action {action.Type} failed: {ex.Message}");
            }
        }
    }

    private async Task ExecuteActionAsync(DmsWorkflowInstance instance, StepAction action)
    {
        var parameters = action.Parameters ?? new Dictionary<string, object>();

        switch (action.Type)
        {
            case RhyoliteOrbitConsts.StepActionType.SendEmail:
                var recipients = parameters.TryGetValue("recipients", out var recipientsValue)
                    ? recipientsValue?.ToString()
                    : string.Empty;

                var subject = parameters.TryGetValue("subject", out var subjectValue)
                    ? subjectValue?.ToString()
                    : "Document workflow update";

                var body = parameters.TryGetValue("body", out var bodyValue)
                    ? bodyValue?.ToString()
                    : $"Workflow step completed for document {instance.DocumentId}.";

                if (!string.IsNullOrWhiteSpace(recipients))
                {
                    await _emailSender.SendEmailAsync(recipients, subject, body);
                }

                break;

            case RhyoliteOrbitConsts.StepActionType.UpdateMetadata:
                if (parameters.TryGetValue("attributeId", out var attributeIdValue) &&
                    Guid.TryParse(attributeIdValue?.ToString(), out var attributeId) &&
                    parameters.TryGetValue("value", out var value))
                {
                    var existing = await (await _metadataRepository.GetAllAsync())
                        .FirstOrDefaultAsync(v => v.DocumentId == instance.DocumentId && v.AttributeId == attributeId);

                    if (existing == null)
                    {
                        await _metadataRepository.InsertAsync(new DmsDocumentMetadataValues
                        {
                            TenantId = instance.TenantId,
                            DocumentId = instance.DocumentId,
                            AttributeId = attributeId,
                            Value = value?.ToString()
                        });
                    }
                    else
                    {
                        existing.Value = value?.ToString();
                        await _metadataRepository.UpdateAsync(existing);
                    }
                }

                break;

            case RhyoliteOrbitConsts.StepActionType.Webhook:
                if (parameters.TryGetValue("url", out var urlValue) && urlValue != null)
                {
                    await InvokeWebhookAsync(urlValue.ToString(), instance);
                }

                break;

            case RhyoliteOrbitConsts.StepActionType.AddComment:
                if (parameters.TryGetValue("comment", out var commentValue))
                {
                    await _commentRepository.InsertAsync(new DmsComment
                    {
                        TenantId = instance.TenantId,
                        DocumentId = instance.DocumentId,
                        CommentText = commentValue?.ToString(),
                        CommentType = "General"
                    });
                }

                break;
        }

        await Task.CompletedTask;
    }

    private async Task InvokeWebhookAsync(string url, DmsWorkflowInstance instance)
    {
        try
        {
            using var client = new System.Net.Http.HttpClient();
            var payload = JsonConvert.SerializeObject(new
            {
                instanceId = instance.Id,
                documentId = instance.DocumentId,
                status = instance.Status,
                occurredAt = DateTime.UtcNow
            });

            var content = new System.Net.Http.StringContent(payload, System.Text.Encoding.UTF8, "application/json");
            await client.PostAsync(url, content);
        }
        catch (Exception ex)
        {
            Logger.Warn($"Webhook {url} failed: {ex.Message}");
        }
    }

    private async Task<WorkflowContext> BuildContextAsync(DmsWorkflowInstance instance, long? actorId)
    {
        var document = await _documentRepository.FirstOrDefaultAsync(instance.DocumentId);

        var values = await (await _metadataRepository.GetAllAsync())
            .Where(v => v.DocumentId == instance.DocumentId)
            .ToListAsync();

        var variables = string.IsNullOrEmpty(instance.VariablesJson)
            ? new Dictionary<string, string>()
            : JsonConvert.DeserializeObject<Dictionary<string, string>>(instance.VariablesJson) ?? new Dictionary<string, string>();

        return new WorkflowContext
        {
            Document = document,
            Metadata = values.GroupBy(v => v.AttributeId).ToDictionary(g => g.Key, g => g.Last().Value),
            Variables = variables,
            UserId = actorId ?? instance.StartedById
        };
    }

    private List<WorkflowTemplateStep> DeserializeSteps(DmsWorkflowInstance instance)
    {
        if (string.IsNullOrEmpty(instance.TemplateSnapshotJson))
        {
            return new List<WorkflowTemplateStep>();
        }

        try
        {
            return JsonConvert.DeserializeObject<List<WorkflowTemplateStep>>(instance.TemplateSnapshotJson)
                   ?? new List<WorkflowTemplateStep>();
        }
        catch
        {
            return new List<WorkflowTemplateStep>();
        }
    }

    private static RhyoliteOrbitConsts.WorkflowTaskType MapTaskType(RhyoliteOrbitConsts.WorkflowStepType stepType)
    {
        return stepType switch
        {
            RhyoliteOrbitConsts.WorkflowStepType.Approval => RhyoliteOrbitConsts.WorkflowTaskType.Approval,
            RhyoliteOrbitConsts.WorkflowStepType.Review => RhyoliteOrbitConsts.WorkflowTaskType.Review,
            RhyoliteOrbitConsts.WorkflowStepType.Signature => RhyoliteOrbitConsts.WorkflowTaskType.Signature,
            RhyoliteOrbitConsts.WorkflowStepType.Submission => RhyoliteOrbitConsts.WorkflowTaskType.Submission,
            RhyoliteOrbitConsts.WorkflowStepType.Escalation => RhyoliteOrbitConsts.WorkflowTaskType.Escalation,
            _ => RhyoliteOrbitConsts.WorkflowTaskType.Notification
        };
    }
}
