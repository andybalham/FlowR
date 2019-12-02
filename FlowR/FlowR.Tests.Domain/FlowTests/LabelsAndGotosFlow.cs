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

        protected override void ConfigureDefinition(FlowDefinition<LabelsAndGotosFlowRequest, LabelsAndGotosFlowResponse> flowDefinition)
        {
            flowDefinition
                .Goto("Label_one")
                .Label("Label_two")
                .End()
                .Label("Label_one")
                .Label("Label_three");
        }
    }

}
