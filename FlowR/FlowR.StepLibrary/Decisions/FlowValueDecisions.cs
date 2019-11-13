namespace FlowR.StepLibrary.Decisions
{
    // TODO: How about a decision that tests for null

    // TODO: Rename these FlowValue*

    public class IntFlowValueDecision : FlowValueDecision<int?> { }
    public class IntFlowValueDecisionHandler : FlowValueDecisionHandler<IntFlowValueDecision, int?> { }

    public class StringFlowValueDecision : NullableFlowValueDecision<string> { }
    public class StringFlowValueDecisionHandler : FlowValueDecisionHandler<StringFlowValueDecision, string> { }

    public class BoolFlowValueDecision : FlowValueDecision<bool?> { }
    public class BoolFlowValueDecisionHandler : FlowValueDecisionHandler<BoolFlowValueDecision, bool?> { }
}
