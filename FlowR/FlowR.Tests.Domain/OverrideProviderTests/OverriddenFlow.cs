using System.Collections.Generic;
using FlowR.StepLibrary.Activities;
using MediatR;

namespace FlowR.Tests.Domain.OverrideProviderTests
{
    public class OverriddenFlowRequest : FlowActivityRequest<OverriddenFlowResponse>, ITestOverrideContext
    {
        public const string BaseValue = "BaseValue";

        public string FlowValue { get; set; }

        public string InputValue => BaseValue;
    }

    public class OverriddenFlowResponse : FlowResponse
    {
        public string OutputValue { get; set; }
    }

    public class OverriddenFlow : FlowHandler<OverriddenFlowRequest, OverriddenFlowResponse>
    {
        public OverriddenFlow(IMediator mediator, IFlowOverrideProvider overrideProvider = null,
            IFlowLogger<OverriddenFlow> flowLogger = null) 
            : base(mediator, overrideProvider, flowLogger)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            return new FlowDefinition()
                .Do("Activity", 
                    new FlowActivityDefinition<SetStringFlowValueRequest, SetStringFlowValueResponse>()
                        .SetValue(rq => rq.OutputValue, OverriddenFlowRequest.BaseValue)
                        .BindOutput(rq => rq.Output, nameof(OverriddenFlowResponse.OutputValue)));
        }
    }
}
