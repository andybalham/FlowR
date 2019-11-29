using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{
    public class TestDictionaryRegexMapBindingsRequest : FlowActivityRequest<TestDictionaryRegexMapBindingsResponse>
    {
        public string RequestValue1 { get; set; }
        public string RequestValue2 { get; set; }
        public string NonMatchInput { get; set; }
    }

    public class TestDictionaryRegexMapBindingsResponse : FlowResponse
    {
        public string ResponseValue1 { get; set; }
        public string ResponseValue2 { get; set; }
        public string NonMatchOutput { get; set; }
    }

    public class TestDictionaryRegexMapBindingsFlow : FlowHandler<TestDictionaryRegexMapBindingsRequest, TestDictionaryRegexMapBindingsResponse>
    {
        public TestDictionaryRegexMapBindingsFlow(IMediator mediator, IFlowLogger<TestDictionaryRegexMapBindingsFlow> logger) : base(mediator, logger)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition flowDefinition)
        {
            flowDefinition
                .Do("MapValues", 
                    new FlowActivityDefinition<DictionaryInputToOutputMapRequest, DictionaryInputToOutputMapResponse>()
                        .BindInputs(rq => rq.FlowInputs, 
                            new FlowValueRegexSelector("^Request.*", n => n.Replace("Request", "Dictionary")))
                        .BindOutputs(rs => rs.FlowOutputs, 
                            new FlowValueRegexSelector("^Dictionary.*", n => n.Replace("Dictionary", "Response"))));
        }
    }
}
