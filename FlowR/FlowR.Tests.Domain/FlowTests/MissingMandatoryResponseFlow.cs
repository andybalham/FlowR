using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowR.Tests.Domain.FlowTests
{
    public class MissingMandatoryResponseRequest : FlowActivityRequest<MissingMandatoryResponseResponse>
    {
    }

    public class MissingMandatoryResponseResponse : FlowResponse
    {
        [NotNullValue]
        public int? MandatoryValue { get; set; }

        public int? OptionalValue { get; set; }

        [NotNullValue]
        public int? DefaultValue { get; set; } = 616;
    }

    public class MissingMandatoryResponseFlow : FlowHandler<MissingMandatoryResponseRequest, MissingMandatoryResponseResponse>
    {
        public MissingMandatoryResponseFlow(IMediator mediator, IFlowLogger<MissingMandatoryResponseFlow> logger) : base(mediator, logger)
        {
        }
    }
}
