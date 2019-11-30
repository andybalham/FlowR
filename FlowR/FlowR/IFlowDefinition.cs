using System.Collections.Generic;

namespace FlowR
{
    public interface IFlowDefinition
    {
        string Criteria { get; }

        IReadOnlyList<FlowStep> Steps { get; }
        
        IEnumerable<string> Validate();
    }
}