using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlowR;
using MediatR;

namespace TwentyQuestions
{
    public class GuessRequest : FlowActivityRequest<GuessResponse>
    {
        public static FlowActivityDefinition<GuessRequest, GuessResponse> NewDefinition() =>
            new FlowActivityDefinition<GuessRequest, GuessResponse>();

        public override string GetText() => $"Guess at '{Guess}'";

        public string Guess { get; set; }
    }

    public class GuessResponse
    {
    }

    public class GuessHandler : IRequestHandler<GuessRequest, GuessResponse>
    {
        public Task<GuessResponse> Handle(GuessRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine("****************************************************");
            Console.WriteLine($"Is it a {request.Guess}?");
            Console.WriteLine("****************************************************");

            return Task.FromResult(new GuessResponse());
        }
    }
}
