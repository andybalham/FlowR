using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{
    public class TestDictionarySubPropertyBindingsRequest : FlowActivityRequest<TestDictionarySubPropertyBindingsResponse>
    {
        public int NamedInput1 { get; set; }
        public int NamedInput2 { get; set; }
        public int RegexInput1 { get; set; }
        public int RegexInput2 { get; set; }
    }

    public class TestDictionarySubPropertyBindingsResponse : FlowResponse
    {
        public int NamedOutput1 { get; set; }
        public int NamedOutput2 { get; set; }
        public int RegexOutput1 { get; set; }
        public int RegexOutput2 { get; set; }
    }

    public class TestDictionarySubPropertyBindingsFlow : FlowHandler<TestDictionarySubPropertyBindingsRequest, TestDictionarySubPropertyBindingsResponse>
    {
        public TestDictionarySubPropertyBindingsFlow(IMediator mediator, IFlowLogger<TestDictionarySubPropertyBindingsFlow> logger) : base(mediator, logger)
        {
        }

        public override FlowDefinition GetFlowDefinition()
        {
            return new FlowDefinition()
                .Do("MapValues",
                    new FlowActivityDefinition<DictionarySubPropertyBindingsMappingRequest, DictionarySubPropertyBindingsMappingResponse>()
                        .BindInputs(rq => rq.NamedFlowInputs, new FlowValueListSelector("NamedInput1", "NamedInput2"), (int v) => v + 1)
                        .BindOutputs(rs => rs.NamedFlowOutputs, new FlowValueListSelector("NamedOutput1", "NamedOutput2"), v => v + 1)
                        .BindInputs(rq => rq.RegexFlowInputs, new FlowValueRegexSelector("^RegexInput.*"), (int v) => v + 2)
                        .BindOutputs(rs => rs.RegexFlowOutputs, new FlowValueRegexSelector("^RegexOutput.*"), v => v + 2)
                    );
        }
    }

    public class DictionarySubPropertyBindingsMappingRequest : FlowActivityRequest<DictionarySubPropertyBindingsMappingResponse>
    {
        public FlowValueDictionary<int> NamedFlowInputs { get; set; }

        public FlowValueDictionary<int> RegexFlowInputs { get; set; }
    }

    public class DictionarySubPropertyBindingsMappingResponse : FlowResponse
    {
        public FlowValueDictionary<int> NamedFlowOutputs { get; set; }

        public FlowValueDictionary<int> RegexFlowOutputs { get; set; }
    }

    public class DictionarySubPropertyBindingsMapping : RequestHandler<DictionarySubPropertyBindingsMappingRequest, DictionarySubPropertyBindingsMappingResponse>
    {
        protected override DictionarySubPropertyBindingsMappingResponse Handle(DictionarySubPropertyBindingsMappingRequest request)
        {
            var namedValues = new FlowValueDictionary<int>();

            foreach (var inputKey in request.NamedFlowInputs.Keys)
            {
                namedValues.Add(inputKey.Replace("Input", "Output"), request.NamedFlowInputs[inputKey]);
            }

            var regexValues = new FlowValueDictionary<int>();

            foreach (var inputKey in request.RegexFlowInputs.Keys)
            {
                regexValues.Add(inputKey.Replace("Input", "Output"), request.RegexFlowInputs[inputKey]);
            }

            var response = new DictionarySubPropertyBindingsMappingResponse
            {
                NamedFlowOutputs = namedValues,
                RegexFlowOutputs = regexValues
            };

            return response;
        }
    }
}
