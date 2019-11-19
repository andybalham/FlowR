using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlowR;
using MediatR;

namespace HelloFlowR
{
    public class SayGreetingRequest : FlowActivityRequest<SayGreetingResponse>
    {
        public string Greeting { get; set; }

        public string Name { get; set; }
    }

    public class SayGreetingResponse
    {
        public string OutputtedText { get; set; }
    }

    public class SayGreetingHandler : IRequestHandler<SayGreetingRequest, SayGreetingResponse>
    {
        public Task<SayGreetingResponse> Handle(SayGreetingRequest request, CancellationToken cancellationToken)
        {
            var text = $"{request.Greeting} {request.Name}";

            Console.WriteLine(text);

            return Task.FromResult(new SayGreetingResponse { OutputtedText = text });
        }
    }
}
