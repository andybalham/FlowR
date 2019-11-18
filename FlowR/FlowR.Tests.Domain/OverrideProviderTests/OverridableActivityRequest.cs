namespace FlowR.Tests.Domain.OverrideProviderTests
{
    public class OverridableActivityRequest : FlowActivityRequest<OverridableActivityResponse>, ITestOverrideContext
    {
        [DesignTimeValue, OverridableValue]
        public string OverridableInputValue { get; set; }

        [DesignTimeValue]
        public string NonOverridableInputValue { get; set; }

        public string FlowValue { get; set; }
    }
}