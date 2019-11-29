using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{
    public class TestDictionaryListMapBindingsRequest : FlowActivityRequest<TestDictionaryListMapBindingsResponse>
    {
        public string RequestValue1 { get; set; }
        public string RequestValue2 { get; set; }
        public string RequestValue3 { get; set; }
    }

    public class TestDictionaryListMapBindingsResponse : FlowResponse
    {
        public string ResponseValue1 { get; set; }
        public string ResponseValue2 { get; set; }
        public string ResponseValue3 { get; set; }
    }

    public class TestDictionaryListMapBindingsFlow : FlowHandler<TestDictionaryListMapBindingsRequest, TestDictionaryListMapBindingsResponse>
    {
        public TestDictionaryListMapBindingsFlow(IMediator mediator, IFlowLogger<TestDictionaryListMapBindingsFlow> logger) 
            : base(mediator, logger)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition flowDefinition)
        {
            flowDefinition
                .Do("MapValues",
                    new FlowActivityDefinition<DictionaryInputToOutputMapRequest, DictionaryInputToOutputMapResponse>()
                        .BindInputs(rq => rq.FlowInputs, new Dictionary<string, string>
                            {
                                { "RequestValue1", "DictionaryValue1" },
                                { "RequestValue2", "DictionaryValue2" },
                            })
                        .BindOutputs(rs => rs.FlowOutputs, new Dictionary<string, string>
                            {
                                { "DictionaryValue1", "ResponseValue1" },
                            })
                    );
        }
    }
}
