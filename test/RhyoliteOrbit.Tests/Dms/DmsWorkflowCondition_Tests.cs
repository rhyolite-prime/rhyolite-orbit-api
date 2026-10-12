using System;
using System.Collections.Generic;
using RhyoliteOrbit.DomainServices.Dms.Workflow;
using RhyoliteOrbit.Models.Dms;
using Shouldly;
using Xunit;

namespace RhyoliteOrbit.Tests.Dms;

/// <summary>
/// Unit tests for the structured/expression workflow step condition evaluator.
/// Pure logic, no database required.
/// </summary>
public class DmsWorkflowCondition_Tests
{
    private readonly IConditionEvaluator _evaluator = new ConditionEvaluator();

    private static WorkflowContext NewContext(DmsDocument document = null)
    {
        return new WorkflowContext
        {
            Document = document ?? new DmsDocument
            {
                Name = "Invoice 2024",
                Extension = ".pdf",
                FileSize = 2048,
                Status = 1,
                CustomId = "INV-0001",
                FolderId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Language = "en"
            },
            Metadata = new Dictionary<Guid, string>
            {
                { Guid.Parse("22222222-2222-2222-2222-222222222222"), "15000" },
                { Guid.Parse("33333333-3333-3333-3333-333333333333"), "GHS" }
            },
            Variables = new Dictionary<string, string> { { "Region", "Greater Accra" } },
            UserId = 42,
            PreviousOutcome = "Approved"
        };
    }

    [Fact]
    public void Should_Return_True_When_Condition_Is_Null()
    {
        _evaluator.Evaluate(null, NewContext()).ShouldBeTrue();
    }

    [Fact]
    public void Should_Evaluate_Document_Property_Equals()
    {
        var condition = new StepCondition
        {
            Field = RhyoliteOrbitConsts.ConditionField.DocumentProperty,
            Name = "Extension",
            Operator = RhyoliteOrbitConsts.ConditionOperator.Equals,
            Value = ".pdf"
        };

        _evaluator.Evaluate(condition, NewContext()).ShouldBeTrue();
    }

    [Fact]
    public void Should_Evaluate_Document_Property_Numeric_Comparison()
    {
        var condition = new StepCondition
        {
            Field = RhyoliteOrbitConsts.ConditionField.DocumentProperty,
            Name = "FileSize",
            Operator = RhyoliteOrbitConsts.ConditionOperator.GreaterThan,
            Value = "1024"
        };

        _evaluator.Evaluate(condition, NewContext()).ShouldBeTrue();

        condition.Operator = RhyoliteOrbitConsts.ConditionOperator.LessThan;
        _evaluator.Evaluate(condition, NewContext()).ShouldBeFalse();
    }

    [Fact]
    public void Should_Not_Evaluate_Numeric_Comparison_When_Value_Is_Not_A_Number()
    {
        var condition = new StepCondition
        {
            Field = RhyoliteOrbitConsts.ConditionField.DocumentProperty,
            Name = "Name",
            Operator = RhyoliteOrbitConsts.ConditionOperator.GreaterThan,
            Value = "10"
        };

        _evaluator.Evaluate(condition, NewContext()).ShouldBeFalse();
    }

    [Fact]
    public void Should_Evaluate_Metadata_By_Attribute_Id()
    {
        var condition = new StepCondition
        {
            Field = RhyoliteOrbitConsts.ConditionField.DocumentMetadata,
            Name = "22222222-2222-2222-2222-222222222222",
            Operator = RhyoliteOrbitConsts.ConditionOperator.GreaterThan,
            Value = "10000"
        };

        _evaluator.Evaluate(condition, NewContext()).ShouldBeTrue();
    }

    [Fact]
    public void Should_Evaluate_Metadata_Contains_And_In()
    {
        var contains = new StepCondition
        {
            Field = RhyoliteOrbitConsts.ConditionField.DocumentMetadata,
            Name = "33333333-3333-3333-3333-333333333333",
            Operator = RhyoliteOrbitConsts.ConditionOperator.Contains,
            Value = "GH"
        };

        _evaluator.Evaluate(contains, NewContext()).ShouldBeTrue();

        var @in = new StepCondition
        {
            Field = RhyoliteOrbitConsts.ConditionField.DocumentMetadata,
            Name = "33333333-3333-3333-3333-333333333333",
            Operator = RhyoliteOrbitConsts.ConditionOperator.In,
            Value = "USD, GHS , EUR"
        };

        _evaluator.Evaluate(@in, NewContext()).ShouldBeTrue();
    }

    [Fact]
    public void Should_Evaluate_System_Variables()
    {
        var today = new StepCondition
        {
            Field = RhyoliteOrbitConsts.ConditionField.SystemVariable,
            Name = "Today",
            Operator = RhyoliteOrbitConsts.ConditionOperator.StartsWith,
            Value = DateTime.UtcNow.ToString("yyyy")
        };

        _evaluator.Evaluate(today, NewContext()).ShouldBeTrue();

        var variable = new StepCondition
        {
            Field = RhyoliteOrbitConsts.ConditionField.SystemVariable,
            Name = "Region",
            Operator = RhyoliteOrbitConsts.ConditionOperator.Equals,
            Value = "Greater Accra"
        };

        _evaluator.Evaluate(variable, NewContext()).ShouldBeTrue();
    }

    [Fact]
    public void Should_Evaluate_User_And_Previous_Outcome()
    {
        var user = new StepCondition
        {
            Field = RhyoliteOrbitConsts.ConditionField.UserProperty,
            Name = "Id",
            Operator = RhyoliteOrbitConsts.ConditionOperator.Equals,
            Value = "42"
        };

        _evaluator.Evaluate(user, NewContext()).ShouldBeTrue();

        var outcome = new StepCondition
        {
            Field = RhyoliteOrbitConsts.ConditionField.PreviousStepOutcome,
            Operator = RhyoliteOrbitConsts.ConditionOperator.NotEquals,
            Value = "Rejected"
        };

        _evaluator.Evaluate(outcome, NewContext()).ShouldBeTrue();
    }

    [Fact]
    public void Should_Be_Case_Insensitive_For_Text_Comparison()
    {
        var condition = new StepCondition
        {
            Field = RhyoliteOrbitConsts.ConditionField.DocumentProperty,
            Name = "Name",
            Operator = RhyoliteOrbitConsts.ConditionOperator.Contains,
            Value = "INVOICE"
        };

        _evaluator.Evaluate(condition, NewContext()).ShouldBeTrue();
    }
}
