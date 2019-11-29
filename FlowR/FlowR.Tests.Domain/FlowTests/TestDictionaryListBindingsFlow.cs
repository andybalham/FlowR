using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{
    public class TestDictionaryListBindingsRequest : FlowActivityRequest<TestDictionaryListBindingsResponse>
    {
        public string FlowInput1 { get; set; }
        public string FlowInput2 { get; set; }
        public string FlowInput3 { get; set; }
    }

    public class TestDictionaryListBindingsResponse : FlowResponse
    {
        public string FlowOutput1 { get; set; }
        public string FlowOutput2 { get; set; }
        public string FlowOutput3 { get; set; }
    }

    public class TestDictionaryListBindingsFlow : FlowHandler<TestDictionaryListBindingsRequest, TestDictionaryListBindingsResponse>
    {
        public TestDictionaryListBindingsFlow(IMediator mediator, IFlowLogger<TestDictionaryListBindingsFlow> logger) : base(mediator, logger)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition flowDefinition)
        {
            flowDefinition
                .Do("MapValues",
                    new FlowActivityDefinition<DictionaryInputToOutputMapRequest, DictionaryInputToOutputMapResponse>()
                        .BindInputs(rq => rq.FlowInputs, "FlowInput1", "FlowInput2")
                        .BindOutputs(rs => rs.FlowOutputs, "FlowOutput1"));
        }
    }
}
