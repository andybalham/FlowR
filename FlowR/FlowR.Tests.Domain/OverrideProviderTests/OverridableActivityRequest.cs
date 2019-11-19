namespace FlowR.Tests.Domain.OverrideProviderTests
{
    public class OverridableActivityRequest : FlowActivityRequest<OverridableActivityResponse>, ITestOverrideContext
    {
        [OverridableValue]
        public string OverridableInputValue { get; set; }

        public string NonOverridableInputValue { get; set; }

        public string FlowValue { get; set; }
    }
}