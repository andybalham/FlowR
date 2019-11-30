using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace FlowR.Tests.Domain.FlowTests
{
    public class TestDecisionDictionaryBindingRequest : FlowActivityRequest<TestDecisionDictionaryBindingResponse>
    {
        public string String1 { get; set; }
        public string String2 { get; set; }
        public string String3 { get; set; }
    }

    public class TestDecisionDictionaryBindingResponse : FlowResponse
    {
    }

    public class TestDecisionDictionaryBindingFlow : FlowHandler<TestDecisionDictionaryBindingRequest, TestDecisionDictionaryBindingResponse>
    {
        public TestDecisionDictionaryBindingFlow(IMediator mediator, IFlowLoggerBase logger = null) : base(mediator, logger)
        {
        }

        protected override void ConfigureDefinition(FlowDefinition<TestDecisionDictionaryBindingRequest, TestDecisionDictionaryBindingResponse> flowDefinition)
        {
            flowDefinition
                .Check("Decision", new FlowDecisionDefinition<DictionaryBindingDecisionRequest, int>()
                    .BindInputs(rq => rq.Strings, new FlowValueTypeSelector(typeof(string)))
                    .BindInputs(rq => rq.NamedStrings, "String2")
                    .BindInputs(rq => rq.StringLengths, new FlowValueRegexSelector(".*String.*", n => n + ".Length"), (string v) => v.Length)
                    .BindInputs(rq => rq.RenamedStrings, new Dictionary<string, string>
                    {
                        { "String1", "RenamedString1" },
                        { "String2", "RenamedString2" },
                    })
                )
                .Else().Continue();
        }
    }

    public class DictionaryBindingDecisionRequest : FlowDecision<int>
    {
        public FlowValueDictionary<string> NamedStrings { get; set; }

        public FlowValueDictionary<string> RenamedStrings { get; set; }

        public FlowValueDictionary<int> StringLengths { get; set; }

        public FlowValueDictionary<string> Strings { get; set; }

        public override int GetMatchingBranchIndex()
        {
            if (this.Strings.Count != 3) return -1;
            if (this.Strings["String3"] != "StringValue3") return -1;

            if (this.NamedStrings.Count != 1) return -1;
            if (this.NamedStrings["String2"] != "StringValue2") return -1;

            if (this.RenamedStrings.Count != 2) return -1;
            if (this.RenamedStrings["RenamedString1"] != "StringValue1") return -1;

            if (this.StringLengths.Count != 3) return -1;
            if (this.StringLengths["String1.Length"] != 12) return -1;

            return 0;
        }
    }
}
