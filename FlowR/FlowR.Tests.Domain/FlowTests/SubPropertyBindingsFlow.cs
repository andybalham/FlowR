using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{
    public class SubPropertyBindingsFlowRequest : FlowActivityRequest<SubPropertyBindingsFlowResponse>
    {
        public string StringValue1 { get; set; }
        public string StringValue2 { get; set; }
    }

    public class SubPropertyBindingsFlowResponse : FlowResponse
    {
        public string StringValue1 { get; set; }
        public string StringValue2 { get; set; }
    }

    public class SubPropertyBindingsFlow : FlowHandler<SubPropertyBindingsFlowRequest, SubPropertyBindingsFlowResponse>
    {
        public SubPropertyBindingsFlow(IMediator mediator) : base(mediator)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            return new FlowDefinition()

                .Check("Decision_1", FlowValueDecision<int?>.NewDefinition()
                    .BindInput(rq => rq.SwitchValue, "StringValue1", (string s) => s.Length))
                .When(0).End()
                .Else().Continue()

                .Check("Decision_2",
                    FlowValueDecision<int?>.NewDefinition()
                        .BindInput(rq => rq.SwitchValue, "StringValue2", (string s) => s.Length))
                .When(0).End()
                .Else().Continue()

                .Do("Activity_1",
                    new FlowActivityDefinition<SubPropertyBindingsActivityRequest, SubPropertyBindingsActivityResponse>()
                        .BindInput(rq => rq.IntValue, "StringValue1", (string s) => s.Length)
                        .BindOutput(rs => rs.IntValue, "StringValue1", i => i.ToString()))

                .Do("Activity_2",
                    new FlowActivityDefinition<SubPropertyBindingsActivityRequest, SubPropertyBindingsActivityResponse>()
                        .BindInput(rq => rq.IntValue, "StringValue2", (string s) => s.Length)
                        .BindOutput(rs => rs.IntValue, "StringValue2", i => i.ToString()));
        }
    }

    public class SubPropertyBindingsActivityRequest : FlowActivityRequest<SubPropertyBindingsActivityResponse>
    {
        [BoundValue]
        public int IntValue { get; set; }
    }

    public class SubPropertyBindingsActivityResponse
    {
        public int IntValue { get; set; }
    }

    public class SubPropertyBindingsActivity : RequestHandler<SubPropertyBindingsActivityRequest, SubPropertyBindingsActivityResponse>
    {
        protected override SubPropertyBindingsActivityResponse Handle(SubPropertyBindingsActivityRequest request)
        {
            return new SubPropertyBindingsActivityResponse { IntValue = request.IntValue };
        }
    }
}
