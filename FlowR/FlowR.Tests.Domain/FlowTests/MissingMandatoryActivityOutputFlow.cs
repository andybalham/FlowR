using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{
    public class MissingMandatoryActivityOutputRequest : FlowActivityRequest<MissingMandatoryActivityOutputResponse>
    {
    }

    public class MissingMandatoryActivityOutputResponse : FlowResponse
    {
    }

    public class MissingMandatoryActivityOutputFlow : FlowHandler<MissingMandatoryActivityOutputRequest, MissingMandatoryActivityOutputResponse>
    {
        public MissingMandatoryActivityOutputFlow(IMediator mediator) : base(mediator)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            return new FlowDefinition()
                .Do("MandatoryOutputActivity",
                    new FlowActivityDefinition<MandatoryOutputActivityRequest, MandatoryOutputActivityResponse>());
        }
    }

    public class MandatoryOutputActivityRequest : FlowActivityRequest<MandatoryOutputActivityResponse>
    {
    }

    public class MandatoryOutputActivityResponse : FlowResponse
    {
        [NotNullValue]
        public string MandatoryOutput { get; set; }

        public string OptionalOutput { get; set; }
    }

    public class MandatoryOutputActivity : RequestHandler<MandatoryOutputActivityRequest, MandatoryOutputActivityResponse>
    {
        protected override MandatoryOutputActivityResponse Handle(MandatoryOutputActivityRequest request)
        {
            return new MandatoryOutputActivityResponse();
        }
    }
}
