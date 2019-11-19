using System;
using System.Collections.Generic;
using System.Text;

namespace FlowR
{
    public interface IFlowLoggerBase
    {
        void LogFlowRequest(FlowContext flowContext, IFlowStepRequest flowRequest);

        void LogFlowResponse(FlowContext flowContext, object flowResponse, long elapsedMilliseconds);

        void LogActivityRequest(FlowContext flowContext, IFlowStepRequest activityRequest);

        void LogActivityResponse(FlowContext flowContext, object activityResponse, long elapsedMilliseconds);

        void LogDecisionRequest(FlowContext flowContext, IFlowStepRequest decisionRequest);

        void LogDecisionResponse(FlowContext flowContext, DecisionFlowStepBase.Branch branch);

        IDisposable BeginFlowScope(FlowContext flowContext);

        void LogFlowException(FlowContext flowContext, IFlowStepRequest flowRequest, Exception ex);

        void LogFlowInnerException(FlowContext flowContext, Exception innerEx);

        void LogRequestOverrides(FlowContext flowContext, IFlowStepRequest request, List<Tuple<string, object, string>> overrides);

        void LogLabel(FlowContext flowContext);
        
        void LogGoto(FlowContext flowContext, string nextStepName);

        void LogMockRequestHandler(FlowContext flowContext, FlowStep flowStep);

        void LogActivityRequestDisabled(FlowContext flowContext, IFlowStepRequest activityRequest);

        void LogFlowOverride(FlowContext flowContext, IFlowStepRequest flowRequest, string criteria);
    }

    public interface IFlowLogger<T> : IFlowLoggerBase
    {
    }
}
