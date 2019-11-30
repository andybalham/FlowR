using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{
    public class TestElseThrowRequest : FlowActivityRequest<TestElseThrowResponse>
    {
        public string Input { get; set; }
    }

    public class TestElseThrowResponse : FlowResponse
    {
    }

    public class TestElseThrowHandler : FlowHandler<TestElseThrowRequest, TestElseThrowResponse>
    {
        public TestElseThrowHandler(IMediator mediator) : base(mediator)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition<TestElseThrowRequest, TestElseThrowResponse> flowDefinition)
        {
            flowDefinition
                .Check("UnhandledDecisionName", NullableFlowValueDecision<string>.NewDefinition()
                    .BindInput(rq => rq.SwitchValue, nameof(TestElseThrowRequest.Input)))
                .When((string)null).End()
                .Else().Unhandled();
        }
    }
}
