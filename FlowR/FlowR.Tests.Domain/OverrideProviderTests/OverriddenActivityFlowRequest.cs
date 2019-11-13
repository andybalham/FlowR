namespace FlowR.Tests.Domain.OverrideProviderTests
{
    public class OverriddenActivityFlowRequest : FlowActivityRequest<OverriddenActivityFlowResponse>
    {
        public const string BaseValue = "BaseValue";
        public const string ActivityOverrideKey = "ActivityOverrideKey";

        public string FlowValue { get; set; }
    }
}