using System;
using System.Collections.Generic;
using System.Text;
using FlowR;
using MediatR;

namespace HelloFlowR
{
    public class SayHelloRequest : FlowActivityRequest<SayHelloResponse>
    {
        public string Name { get; set; }
    }

    public class SayHelloResponse
    {
        public string OutputtedText { get; set; }
    }

    public class SayHelloHandler : FlowHandler<SayHelloRequest, SayHelloResponse>
    {
        public SayHelloHandler(IMediator mediator) : base(mediator)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition<SayHelloRequest, SayHelloResponse> flowDefinition)
        {
            flowDefinition
                .Do("SayHello", new FlowActivityDefinition<SayGreetingRequest, SayGreetingResponse>()
                    .SetValue(req => req.Greeting, "Hello"));
        }
    }
}
