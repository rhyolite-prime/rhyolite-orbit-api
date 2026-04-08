using System.Collections.Generic;

namespace RhyoliteOrbit.Models.Dms;

public class StepAction
{
    public RhyoliteOrbitConsts.StepActionType Type { get; set; }
    public string Name { get; set; }
    public bool IsAutomatic { get; set; }  // Execute without user input
    public Dictionary<string, object> Parameters { get; set; }
}

public class SendEmailAction : StepAction
{
    public string EmailTemplateId { get; set; }
    public List<string> Recipients { get; set; }
}
    
public class UpdateMetadataAction : StepAction
{
    public string AttributeName { get; set; }
    public string ValueExpression { get; set; }
}
    
public class WebhookAction : StepAction
{
    public string Url { get; set; }
    public string HttpMethod { get; set; }
    public Dictionary<string, string> Headers { get; set; }
}