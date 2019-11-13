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

        public override FlowDefinition GetFlowDefinition()
        {
            return new FlowDefinition()
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

    public class DictionaryBindingDecisionRequest : FlowDecisionRequest<int>
    {
        [BoundValue]
        public FlowValueDictionary<string> NamedStrings { get; set; }

        [BoundValue]
        public FlowValueDictionary<string> RenamedStrings { get; set; }

        [BoundValue]
        public FlowValueDictionary<int> StringLengths { get; set; }

        [BoundValue]
        public FlowValueDictionary<string> Strings { get; set; }
    }

    public class DictionaryBindingDecision : FlowDecisionHandler<DictionaryBindingDecisionRequest, int>
    {
        public override Task<int> Handle(DictionaryBindingDecisionRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}
