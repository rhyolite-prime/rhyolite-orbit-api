namespace RhyoliteOrbit.Models.Dms;

public class StepCondition
{
    public string Name { get; set; }
    public string Expression { get; set; }  // e.g., "Document.Metadata['Amount'] > 10000"
    public int NextStepIndex { get; set; }
    public string Description { get; set; }
    
    // For UI builders
    public RhyoliteOrbitConsts.ConditionField Field { get; set; }
    public RhyoliteOrbitConsts.ConditionOperator Operator { get; set; }
    public string Value { get; set; }
}