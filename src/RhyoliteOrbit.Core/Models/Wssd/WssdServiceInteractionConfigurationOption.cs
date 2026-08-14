using System;
using System.Collections.Generic;

namespace RhyoliteOrbit.Models.Wssd;

public class WssdServiceInteractionConfigurationOption
{
    public Guid Id { get; set; }
    public int SortOrder { get; set; }
    public Guid ParentNodeId { get; set; }
    public Guid NextParentNodeId { get; set; }

    public string ExecutionScope { get; set; } // ussd, wssd, wssd-ussd
    public bool IsActive { get; set; }

    public DisplayConfig Display { get; set; }
    public ActionConfig Action { get; set; }
    public InputConfig Input { get; set; }
    public ValidationConfig Validation { get; set; }
    public FlowControlConfig FlowControl { get; set; }
    public IntegrationConfig Integration { get; set; }
    public DataConfig Data { get; set; }
    public NotificationConfig Notification { get; set; }
    public CurrencyConfig Currency { get; set; }
    public DeferredExecutionConfig DeferredExecution { get; set; }
}

public class FlowContextVariable
{
    public Guid Id { get; set; }
    public string VariableName { get; set; }
    public string VariableValue { get; set; }
    public string ValueType { get; set; } // string, number, boolean, etc.
}
public class DataBucketQueryExpression
{
    public Guid Id { get; set; }
    public string DataBucketFieldName { get; set; }
    public string BaseRuleOrCriteria { get; set; }
    public string ParameterOrValue { get; set; }
    public string ConnectingCriteria { get; set; }
}

public class DeferredRoutineTerminationCriteria
{
    public Guid Id { get; set; }
    public string FieldName { get; set; }
    public string ValueType { get; set; }
    public string Value { get; set; }
}

public class DisplayConfig
{
    public string DisplayText { get; set; }
    public string DisplayType { get; set; }
    public string DisplayOutput { get; set; }
    public string VisibilityRule { get; set; }
    public string RedirectUrl { get; set; }
}

public class ActionConfig
{
    public string ActionType { get; set; } // api, menu, display, input
    public string PayloadMappingExpression { get; set; }
}

public class InputConfig
{
    public string InputType { get; set; } // text, number, date, time
    public string InputFieldName { get; set; }
    public List<string> InputFileList { get; set; }
    public string DefaultValue { get; set; }
}

public class ValidationConfig
{
    public string ValidationNote { get; set; }
    public int Retries { get; set; }
}

public class FlowControlConfig
{
    public string Expression { get; set; }
    public Guid FallbackActionId { get; set; }
}

public class IntegrationConfig
{
    public Guid FlexConnectId { get; set; }
    public Guid DataSourceFlexConnectId { get; set; }
    public string ApiOutputVariable { get; set; }
    public string TransformationExpression { get; set; }
}

public class DataConfig
{
    public Guid CategoryId { get; set; }
    public Guid DataBucketId { get; set; }
    public string OptionDataSource { get; set; }

    public List<DataBucketQueryExpression> Queries { get; set; }
    public List<FlowContextVariable> ContextVariables { get; set; }
}

public class NotificationConfig
{
    public Guid? NotificationAccountId { get; set; }
    public string Channel { get; set; }
}

public class CurrencyConfig
{
    public Guid CurrencyId { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public decimal BuyRate { get; set; }
    public decimal SellRate { get; set; }
}

public class DeferredExecutionConfig
{
    public int ExecutionDelay { get; set; } // seconds
    public int RetryDelay { get; set; } // seconds
    public List<DeferredRoutineTerminationCriteria> TerminationCriteria { get; set; }
}
