using MediatR;

namespace FlowR.Tests.DiscoveryTarget
{
    public class OverridableActivityRequest : FlowActivityRequest<OverridableActivityResponse>
    {
        [OverridableValue]
        public string OverridableInputValue { get; set; }

        public string NonOverridableInputValue { get; set; }

        public string FlowValue { get; set; }
    }

    public class OverridableActivityResponse
    {
        public string OverridableOutputValue { get; set; }
        public string NonOverridableOutputValue { get; set; }
    }

    public class OverridableActivity : RequestHandler<OverridableActivityRequest, OverridableActivityResponse>
    {
        protected override OverridableActivityResponse Handle(OverridableActivityRequest request)
        {
            return new OverridableActivityResponse
            {
                OverridableOutputValue = request.OverridableInputValue,
                NonOverridableOutputValue = request.NonOverridableInputValue,
            };
        }
    }
}