using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlowR.Tests.Domain.FlowTests
{
    public class DoNothingRequest : FlowActivityRequest<DoNothingResponse>
    {
    }

    public class DoNothingResponse
    {
    }

    public class DoNothingHandler : RequestHandler<DoNothingRequest, DoNothingResponse>
    {
        public const string DebugText = "Doing nothing";

        private readonly ILogger<DoNothingHandler> _logger;

        public DoNothingHandler(ILogger<DoNothingHandler> logger)
        {
            _logger = logger;
        }

        protected override DoNothingResponse Handle(DoNothingRequest request)
        {
            _logger.LogDebug($"{request.FlowContext}: {DebugText}");

            return new DoNothingResponse();
        }
    }
}
