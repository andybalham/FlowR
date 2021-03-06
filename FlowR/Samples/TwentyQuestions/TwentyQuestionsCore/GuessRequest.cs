﻿using System;
using System.Threading;
using System.Threading.Tasks;
using FlowR;
using MediatR;

namespace TwentyQuestions.Core
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
            _console.WriteLine($"Is it a {request.Guess}?");

            return Task.FromResult(new GuessResponse { Guess = request.Guess });
        }
    }

    public static class GuessRequestMocks 
    {
        public static FlowContext MockGuessActivity(this FlowContext flowContext) =>
            flowContext.MockActivity<GuessRequest, GuessResponse>(req => new GuessResponse { Guess = req.Guess });
    }
}
