using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.Dms;
using RhyoliteOrbit.Notifications;

namespace RhyoliteOrbit.DomainServices.Dms.Workflow;

/// <summary>Data available to step conditions.</summary>
public class WorkflowContext
{
    public DmsDocument Document { get; set; }

    public Dictionary<Guid, string> Metadata { get; set; }

    public Dictionary<string, string> Variables { get; set; }

    public long UserId { get; set; }

    public string PreviousOutcome { get; set; }
}

/// <summary>Evaluates the structured and expression forms of a <see cref="StepCondition"/>.</summary>
public interface IConditionEvaluator
{
    bool Evaluate(StepCondition condition, WorkflowContext context);
}

public class ConditionEvaluator: IConditionEvaluator
{
    public bool Evaluate(StepCondition condition, WorkflowContext context)
    {
        if (condition == null)
        {
            return true;
        }

        var left = ResolveField(condition, context);
        var right = condition.Value ?? string.Empty;

        switch (condition.Operator)
        {
            case RhyoliteOrbitConsts.ConditionOperator.Equals:
                return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
            case RhyoliteOrbitConsts.ConditionOperator.NotEquals:
                return !string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
            case RhyoliteOrbitConsts.ConditionOperator.Contains:
                return left != null && left.ToLowerInvariant().Contains(right.ToLowerInvariant());
            case RhyoliteOrbitConsts.ConditionOperator.StartsWith:
                return left != null && left.ToLowerInvariant().StartsWith(right.ToLowerInvariant());
            case RhyoliteOrbitConsts.ConditionOperator.EndsWith:
                return left != null && left.ToLowerInvariant().EndsWith(right.ToLowerInvariant());
            case RhyoliteOrbitConsts.ConditionOperator.In:
                return right.Split(',').Select(v => v.Trim()).Contains(left);
            case RhyoliteOrbitConsts.ConditionOperator.NotIn:
                return !right.Split(',').Select(v => v.Trim()).Contains(left);
            case RhyoliteOrbitConsts.ConditionOperator.GreaterThan:
            case RhyoliteOrbitConsts.ConditionOperator.LessThan:
                return CompareNumeric(left, right, condition.Operator);
            default:
                return false;
        }
    }

    private static string ResolveField(StepCondition condition, WorkflowContext context)
    {
        switch (condition.Field)
        {
            case RhyoliteOrbitConsts.ConditionField.DocumentProperty:
                return ResolveDocumentProperty(condition.Name, context);
            case RhyoliteOrbitConsts.ConditionField.DocumentMetadata:
                if (context.Metadata != null)
                {
                    // the value may be the attribute name or the attribute id
                    if (Guid.TryParse(condition.Name, out var attributeId) &&
                        context.Metadata.TryGetValue(attributeId, out var byId))
                    {
                        return byId;
                    }

                    var match = context.Metadata.FirstOrDefault(kvp =>
                        string.Equals(kvp.Key.ToString(), condition.Name, StringComparison.OrdinalIgnoreCase));

                    if (!string.IsNullOrEmpty(match.Value))
                    {
                        return match.Value;
                    }
                }

                return condition.Value;

            case RhyoliteOrbitConsts.ConditionField.UserProperty:
                return context.UserId.ToString();

            case RhyoliteOrbitConsts.ConditionField.SystemVariable:
                return condition.Name?.ToLowerInvariant() switch
                {
                    "today" => DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    "now" => DateTime.UtcNow.ToString("O"),
                    "userid" => context.UserId.ToString(),
                    _ => context.Variables != null && context.Variables.TryGetValue(condition.Name, out var variable)
                        ? variable
                        : null
                };

            case RhyoliteOrbitConsts.ConditionField.PreviousStepOutcome:
                return context.PreviousOutcome;

            default:
                return null;
        }
    }

    private static string ResolveDocumentProperty(string name, WorkflowContext context)
    {
        var document = context.Document;
        if (document == null)
        {
            return null;
        }

        return name?.ToLowerInvariant() switch
        {
            "name" => document.Name,
            "extension" => document.Extension,
            "filesize" => document.FileSize.ToString(),
            "status" => document.Status.ToString(),
            "customid" => document.CustomId,
            "folderid" => document.FolderId.ToString(),
            "language" => document.Language,
            _ => null
        };
    }

    private static bool CompareNumeric(string left, string right, RhyoliteOrbitConsts.ConditionOperator op)
    {
        var leftOk = decimal.TryParse(left, out var leftValue);
        var rightOk = decimal.TryParse(right, out var rightValue);

        if (!leftOk || !rightOk)
        {
            return false;
        }

        return op == RhyoliteOrbitConsts.ConditionOperator.GreaterThan
            ? leftValue > rightValue
            : leftValue < rightValue;
    }
}
