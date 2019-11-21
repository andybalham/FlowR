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
        public string Guess { get; set; }
    }

    public class GuessHandler : IRequestHandler<GuessRequest, GuessResponse>
    {
        private readonly IConsoleService _console;

        public GuessHandler(IConsoleService console)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public Task<GuessResponse> Handle(GuessRequest request, CancellationToken cancellationToken)
        {
            _console.WriteLine("****************************************************");
            _console.WriteLine($"Is it a {request.Guess}?");
            _console.WriteLine("****************************************************");

            return Task.FromResult(new GuessResponse { Guess = request.Guess });
        }
    }
}
