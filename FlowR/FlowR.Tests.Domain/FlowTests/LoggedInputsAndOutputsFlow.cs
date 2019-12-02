using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowR.Tests.Domain.FlowTests
{
    public class LoggedInputsAndOutputsFlowRequest : FlowActivityRequest<LoggedInputsAndOutputsFlowResponse>
    {
        public int PublicValue { get; set; }

        [SensitiveValue]
        public int PrivateValue { get; set; }
    }

    public class LoggedInputsAndOutputsFlowResponse : FlowResponse
    {
        public int PublicValue { get; set; }

        [SensitiveValue]
        public int PrivateValue { get; set; }
    }

    public class LoggedInputsAndOutputsFlow : FlowHandler<LoggedInputsAndOutputsFlowRequest, LoggedInputsAndOutputsFlowResponse>
    {
        public LoggedInputsAndOutputsFlow(IMediator mediator, IFlowLogger<LoggedInputsAndOutputsFlow> logger) 
            : base(mediator, logger)
        {
        }
    }
}
