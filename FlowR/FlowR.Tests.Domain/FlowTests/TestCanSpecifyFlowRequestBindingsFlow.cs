using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{
    public class TestCanSpecifyFlowRequestBindingsRequest : FlowActivityRequest<TestCanSpecifyFlowRequestBindingsResponse>
    {
        public string Default { get; set; }

        public string NameMap { get; set; }

        public string PropertyMap { get; set; }

        public FlowValueDictionary<string> DictionaryDefault { get; set; }

        public FlowValueDictionary<string> DictionaryNameMap { get; set; }

        public FlowValueDictionary<string> DictionaryPropertyMap { get; set; }
    }

    public class TestCanSpecifyFlowRequestBindingsResponse
    {
        public string Default { get; set; }

        public string NameMapped { get; set; }

        public int PropertyMapped { get; set; }

        public string DictionaryDefault1 { get; set; }

        public string DictionaryDefault2 { get; set; }

        public string DictionaryNameMapped1 { get; set; }

        public string DictionaryNameMapped2 { get; set; }

        public int DictionaryPropertyMapped1 { get; set; }

        public int DictionaryPropertyMapped2 { get; set; }
    }

    public class TestCanSpecifyFlowRequestBindingsHandler
        : FlowHandler<TestCanSpecifyFlowRequestBindingsRequest, TestCanSpecifyFlowRequestBindingsResponse>
    {
        public TestCanSpecifyFlowRequestBindingsHandler(IMediator mediator) : base(mediator)
        {
        }

        protected override void ConfigureDefinition(
            FlowDefinition<TestCanSpecifyFlowRequestBindingsRequest, TestCanSpecifyFlowRequestBindingsResponse> flowDefinition)
        {
            flowDefinition
                .Initialize(init => init
                    .BindValue(req => req.NameMap, "NameMapped")
                    .BindValue(req => req.PropertyMap, "PropertyMapped", s => s.Length)
                    .BindOutputs(req => req.DictionaryDefault, "DictionaryDefault1", "DictionaryDefault2")
                    .BindOutputs(req => req.DictionaryNameMap, new Dictionary<string, string>
                    {
                        { "DictionaryNameMap1", "DictionaryNameMapped1" },
                        { "DictionaryNameMap2", "DictionaryNameMapped2" }
                    })
                    .BindOutputs(req => req.DictionaryPropertyMap, new FlowValueListSelector(new Dictionary<string, string>()
                    {
                        { "DictionaryPropertyMap1", "DictionaryPropertyMapped1" },
                        { "DictionaryPropertyMap2", "DictionaryPropertyMapped2" }
                    }), s => s.Length)
                );
        }
    }
}
