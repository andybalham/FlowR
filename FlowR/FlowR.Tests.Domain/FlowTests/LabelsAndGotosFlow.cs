using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{

    public class LabelsAndGotosFlowRequest : FlowActivityRequest<LabelsAndGotosFlowResponse>
    {
    }

    public class LabelsAndGotosFlowResponse : FlowResponse
    {
    }

    public class LabelsAndGotosFlow : FlowHandler<LabelsAndGotosFlowRequest, LabelsAndGotosFlowResponse>
    {
        public LabelsAndGotosFlow(IMediator mediator, IFlowLogger<LabelsAndGotosFlow> logger) : base(mediator, logger)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            return new FlowDefinition()
                .Goto("Label_one")
                .Label("Label_two")
                .End()
                .Label("Label_one")
                .Label("Label_three");
        }
    }

}
