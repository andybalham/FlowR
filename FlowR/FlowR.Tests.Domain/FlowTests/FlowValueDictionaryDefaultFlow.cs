using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{
    public class FlowValueDictionaryDefaultRequest : FlowActivityRequest<FlowValueDictionaryDefaultResponse>
    {
        public string StringInput1 { get; set; }
        public string StringInput2 { get; set; }
        public int IntInput1 { get; set; }
        public int IntInput2 { get; set; }
    }

    public class FlowValueDictionaryDefaultResponse : FlowResponse
    {
        public string StringOutput1 { get; set; }
        public string StringOutput2 { get; set; }
        public int IntOutput1 { get; set; }
        public int IntOutput2 { get; set; }
    }

    public class FlowValueDictionaryDefaultFlow : FlowHandler<FlowValueDictionaryDefaultRequest, FlowValueDictionaryDefaultResponse>
    {
        public FlowValueDictionaryDefaultFlow(IMediator mediator, IFlowLogger<FlowValueDictionaryDefaultFlow> logger) 
            : base(mediator, logger)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition flowDefinition)
        {
            flowDefinition
                .Do("MapValues",
                    new FlowActivityDefinition<DictionaryInputToOutputMapRequest, DictionaryInputToOutputMapResponse>());
        }
    }

    public class DictionaryInputToOutputMapRequest : FlowActivityRequest<DictionaryInputToOutputMapResponse>
    {
        public FlowValueDictionary<string> FlowInputs { get; set; }
    }

    public class DictionaryInputToOutputMapResponse : FlowResponse
    {
        public FlowValueDictionary<string> FlowOutputs { get; set; }
    }

    public class DictionaryInputToOutputMap : RequestHandler<DictionaryInputToOutputMapRequest, DictionaryInputToOutputMapResponse>
    {
        protected override DictionaryInputToOutputMapResponse Handle(DictionaryInputToOutputMapRequest request)
        {
            var responseDictionary = new FlowValueDictionary<string>();

            foreach (var inputKey in request.FlowInputs.Keys)
            {
                responseDictionary.Add(inputKey.Replace("Input", "Output"), request.FlowInputs[inputKey]);
            }

            return new DictionaryInputToOutputMapResponse { FlowOutputs = responseDictionary };
        }
    }
}
