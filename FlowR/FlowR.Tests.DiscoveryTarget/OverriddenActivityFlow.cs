using MediatR;

namespace FlowR.Tests.DiscoveryTarget
{
    public class OverriddenActivityFlowRequest : FlowActivityRequest<OverriddenActivityFlowResponse>
    {
        public const string BaseValue = "BaseValue";
        public const string ActivityOverrideKey = "ActivityOverrideKey";

        public string FlowValue { get; set; }
    }

    public class OverriddenActivityFlowResponse : FlowResponse
    {
        public string OverridableOutputValue { get; set; }
        public string NonOverridableOutputValue { get; set; }
    }

    public class OverriddenActivityFlow : FlowHandler<OverriddenActivityFlowRequest, OverriddenActivityFlowResponse>
    {
        public OverriddenActivityFlow(IMediator mediator, IFlowOverrideProvider overrideProvider = null,
            IFlowLogger<OverriddenActivityFlow> flowLogger = null) 
            : base(mediator, overrideProvider, flowLogger)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            return new FlowDefinition()
                .Do("Activity", new FlowOverrideKey(OverriddenActivityFlowRequest.ActivityOverrideKey),
                    new FlowActivityDefinition<OverridableActivityRequest, OverridableActivityResponse>()
                        .SetValue(rq => rq.OverridableInputValue, OverriddenActivityFlowRequest.BaseValue)
                        .SetValue(rq => rq.NonOverridableInputValue, OverriddenActivityFlowRequest.BaseValue));
        }
    }
}
