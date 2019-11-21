using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlowR;
using MediatR;

namespace TwentyQuestions
{
    public class BuildSummaryRequest : FlowActivityRequest<BuildSummaryResponse>
    {
        public static FlowActivityDefinition<BuildSummaryRequest, BuildSummaryResponse> NewDefinition() =>
            new FlowActivityDefinition<BuildSummaryRequest, BuildSummaryResponse>();

        public override string GetText() => "Build Summary";

        public FlowValueDictionary<string> Values { get; set; }
    }

    public class BuildSummaryResponse
    {
        public string Summary { get; set; }
    }

    public class BuildSummaryHandler : IRequestHandler<BuildSummaryRequest, BuildSummaryResponse>
    {
        public Task<BuildSummaryResponse> Handle(BuildSummaryRequest request, CancellationToken cancellationToken)
        {
            var summary = string.Join(", ", request.Values.Select(kvp => $"{kvp.Key}={kvp.Value ?? "<null>"}"));

            return Task.FromResult(new BuildSummaryResponse { Summary = summary });
        }
    }
}
