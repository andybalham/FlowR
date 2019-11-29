using MediatR;

namespace FlowR.Tests.Domain.OverrideProviderTests
{
    public class OverriddenActivityFlow : FlowHandler<OverriddenActivityFlowRequest, OverriddenActivityFlowResponse>
    {
        public OverriddenActivityFlow(IMediator mediator, IFlowOverrideProvider overrideProvider = null,
            IFlowLogger<OverriddenActivityFlow> flowLogger = null) 
            : base(mediator, overrideProvider, flowLogger)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition flowDefinition)
        {
            flowDefinition
                .Do("Activity", new FlowOverrideKey(OverriddenActivityFlowRequest.ActivityOverrideKey),
                    new FlowActivityDefinition<OverridableActivityRequest, OverridableActivityResponse>()
                        .SetValue(rq => rq.OverridableInputValue, OverriddenActivityFlowRequest.BaseValue)
                        .SetValue(rq => rq.NonOverridableInputValue, OverriddenActivityFlowRequest.BaseValue));
        }
    }
}
