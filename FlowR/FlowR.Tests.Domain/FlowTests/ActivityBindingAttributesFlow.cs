using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlowR.Tests.Domain.FlowTests
{

    public class ActivityBindingAttributesFlowRequest : FlowActivityRequest<ActivityBindingAttributesFlowResponse>
    {
        public string FlowInput1 { get; set; }
        public string FlowInput2 { get; set; }
    }

    public class ActivityBindingAttributesFlowResponse : FlowResponse
    {
        public string FlowOutput1 { get; set; }
        public string FlowOutput2 { get; set; }
    }

    public class ActivityBindingAttributesFlow : FlowHandler<ActivityBindingAttributesFlowRequest, ActivityBindingAttributesFlowResponse>
    {
        public ActivityBindingAttributesFlow(IMediator mediator, IFlowLogger<ActivityBindingAttributesFlow> logger) : base(mediator, logger)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition<ActivityBindingAttributesFlowRequest, ActivityBindingAttributesFlowResponse> flowDefinition)
        {
            flowDefinition
                .Do("Bind_inputs_and_outputs", new FlowActivityDefinition<BindingAttributesActivityRequest, BindingAttributesActivityResponse>()

                    .SetValue(r => r.ActivityInput1Name, nameof(ActivityBindingAttributesFlowRequest.FlowInput1))
                    .SetValue(r => r.NamedActivityInputName, nameof(ActivityBindingAttributesFlowRequest.FlowInput2))

                    .SetValue(r => r.ActivityOutput1Name, nameof(ActivityBindingAttributesFlowResponse.FlowOutput1))
                    .SetValue(r => r.NamedActivityOutputName, nameof(ActivityBindingAttributesFlowResponse.FlowOutput2))
                );
        }
    }

    public class BindingAttributesActivityRequest : FlowActivityRequest<BindingAttributesActivityResponse>
    {
        [InputBindingName]
        public string ActivityInput1Name { get; set; }

        [InputBindingName("ActivityInput2")]
        public string NamedActivityInputName { get; set; }

        [OutputBindingName]
        public string ActivityOutput1Name { get; set; }

        [OutputBindingName(nameof(BindingAttributesActivityResponse.ActivityOutput2))]
        public string NamedActivityOutputName { get; set; }

        public string ActivityInput1 { get; set; }
        public string ActivityInput2 { get; set; }
    }

    public class BindingAttributesActivityResponse
    {
        public string ActivityOutput1 { get; set; }
        public string ActivityOutput2 { get; set; }
    }

    public class BindingAttributesActivity : RequestHandler<BindingAttributesActivityRequest, BindingAttributesActivityResponse>
    {
        protected override BindingAttributesActivityResponse Handle(BindingAttributesActivityRequest request)
        {
            return new BindingAttributesActivityResponse
            {
                ActivityOutput1 = request.ActivityInput1,
                ActivityOutput2 = request.ActivityInput2
            };
        }
    }
}
