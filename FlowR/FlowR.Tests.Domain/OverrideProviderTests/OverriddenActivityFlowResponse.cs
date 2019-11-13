namespace FlowR.Tests.Domain.OverrideProviderTests
{
    public class OverriddenActivityFlowResponse : FlowResponse
    {
        public string OverridableOutputValue { get; set; }
        public string NonOverridableOutputValue { get; set; }
    }
}