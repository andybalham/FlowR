using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowR.Tests.Domain.FlowTests
{
    public class MissingMandatoryRequestRequest : FlowActivityRequest<MissingMandatoryRequestResponse>
    {
        [NotNullValue]
        public int? MandatoryValue { get; set; }

        public int? OptionalValue { get; set; }

        [NotNullValue]
        public int? DefaultValue { get; set; } = 616;
    }

    public class MissingMandatoryRequestResponse : FlowResponse
    {
    }

    public class MissingMandatoryRequestFlow : FlowHandler<MissingMandatoryRequestRequest, MissingMandatoryRequestResponse>
    {
        public MissingMandatoryRequestFlow(IMediator mediator, IFlowLogger<MissingMandatoryRequestFlow> logger) : base(mediator, logger)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition flowDefinition)
        {
        }
    }
}
