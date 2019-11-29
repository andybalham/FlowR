using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{
    public class TestDictionaryRegexBindingsRequest : FlowActivityRequest<TestDictionaryRegexBindingsResponse>
    {
        public string FlowInput1 { get; set; }
        public string FlowInput2 { get; set; }
        public string NonMatchInput { get; set; }
    }

    public class TestDictionaryRegexBindingsResponse : FlowResponse
    {
        public string FlowOutput1 { get; set; }
        public string FlowOutput2 { get; set; }
        public string NonMatchOutput { get; set; }
    }

    public class TestDictionaryRegexBindingsFlow : FlowHandler<TestDictionaryRegexBindingsRequest, TestDictionaryRegexBindingsResponse>
    {
        public TestDictionaryRegexBindingsFlow(IMediator mediator, IFlowLogger<TestDictionaryRegexBindingsFlow> logger) : base(mediator, logger)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition flowDefinition)
        {
            flowDefinition
                .Do("MapValues", 
                    new FlowActivityDefinition<DictionaryInputToOutputMapRequest, DictionaryInputToOutputMapResponse>()
                        .BindInputs(rq => rq.FlowInputs, new FlowValueRegexSelector("^FlowInput.*"))
                        .BindOutputs(rs => rs.FlowOutputs, new FlowValueRegexSelector("^FlowOutput.*")));
        }
    }
}
