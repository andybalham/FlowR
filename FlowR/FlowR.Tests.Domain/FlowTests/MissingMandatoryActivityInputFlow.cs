using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{
    public class MissingMandatoryActivityInputRequest : FlowActivityRequest<MissingMandatoryActivityInputResponse>
    {
    }

    public class MissingMandatoryActivityInputResponse : FlowResponse
    {
    }

    public class MissingMandatoryActivityInputFlow : FlowHandler<MissingMandatoryActivityInputRequest, MissingMandatoryActivityInputResponse>
    {
        public MissingMandatoryActivityInputFlow(IMediator mediator) : base(mediator)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            return new FlowDefinition()
                .Do("MandatoryInputActivity",
                    new FlowActivityDefinition<MandatoryInputActivityRequest, MandatoryInputActivityResponse>());
        }
    }

    public class MandatoryInputActivityRequest : FlowActivityRequest<MandatoryInputActivityResponse>
    {
        [DesignTimeValue, NotNullValue]
        public string MandatorySetInput { get; set; }

        [DesignTimeValue]
        public string OptionalSetInput { get; set; }

        public string OptionalBoundInput { get; set; }

        [NotNullValue]
        public string MandatoryBoundInput { get; set; }

        [NotNullValue]
        public string DefaultBoundInput { get; set; } = "DefaultBoundInput";
    }

    public class MandatoryInputActivityResponse : FlowResponse
    {
    }

    public class MandatoryInputActivity : RequestHandler<MandatoryInputActivityRequest, MandatoryInputActivityResponse>
    {
        protected override MandatoryInputActivityResponse Handle(MandatoryInputActivityRequest request)
        {
            return new MandatoryInputActivityResponse();
        }
    }
}
