using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace FlowR.Microsoft.Extensions.Logging
{
    public class CoreFlowLogger<T> : IFlowLogger<T>
    {
        #region Member declarations

        private readonly ILogger _logger;

        #endregion

        #region Constructors

        public CoreFlowLogger(ILogger<T> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region IFlowLogger implementation

        public void LogFlowRequest(FlowContext flowContext, IFlowStepRequest flowRequest)
        {
            _logger.LogDebug(flowContext, "Request({RequestSummary})",
                _logger.EvalIfDebug(() => GetPublicPropertySummary(flowRequest)));
        }

        public void LogFlowOverride(FlowContext flowContext, IFlowStepRequest flowRequest, string criteria)
        {
            _logger.LogDebug(flowContext, "Flow definition for {FlowRequestTypeName} overridden for criteria '{Criteria}'",
                _logger.EvalIfDebug(() => flowRequest.GetType().Name),
                criteria);
        }

        public void LogFlowResponse(FlowContext flowContext, FlowResponse flowResponse, long elapsedMilliseconds)
        {
            _logger.LogDebug(flowContext, "Response({ResponseSummary}) in {FlowMillis}ms",
                _logger.EvalIfDebug(() => GetPublicPropertySummary(flowResponse)),
                elapsedMilliseconds);
        }

        public void LogActivityRequest(FlowContext flowContext, IFlowStepRequest activityRequest)
        {
            _logger.LogDebug(flowContext, "{ActivityRequestTypeName}({ActivityRequestSummary})",
                _logger.EvalIfDebug(() => activityRequest.GetType().Name),
                _logger.EvalIfDebug(() => GetPublicPropertySummary(activityRequest)));
        }

        public void LogActivityResponse(FlowContext flowContext, object activityResponse, long elapsedMilliseconds)
        {
            _logger.LogDebug(flowContext, "{ActivityResponseTypeName}({ActivityResponseSummary}) in {ActivityMillis}ms",
                _logger.EvalIfDebug(() => activityResponse.GetType().Name),
                _logger.EvalIfDebug(() => GetPublicPropertySummary(activityResponse)),
                elapsedMilliseconds);
        }

        public void LogMockRequestHandler(FlowContext flowContext, FlowStep flowStep)
        {
            _logger.LogDebug(flowContext, "Request handler for {ActivityRequestTypeName} instance '{OverrideKey}' is being mocked",
                _logger.EvalIfDebug(() => flowStep.Definition.RequestType.Name), flowStep.OverrideKey?.Value ?? "<null>");
        }

        public void LogDecisionRequest(FlowContext flowContext, IFlowStepRequest decisionRequest)
        {
            _logger.LogDebug(flowContext, "{DecisionRequestTypeName}({DecisionRequestSummary})",
                _logger.EvalIfDebug(() => decisionRequest.GetType().Name),
                _logger.EvalIfDebug(() => GetPublicPropertySummary(decisionRequest)));
        }

        public void LogDecisionResponse(FlowContext flowContext, DecisionFlowStepBase.Branch branch)
        {
            _logger.LogDebug(flowContext, "{BranchTargets} => {BranchDestination}",
                _logger.EvalIfDebug(() => string.Join("|", branch.Criteria?.ToArray() ?? new object[] { "ELSE" })),
                (branch.NextStepName ?? (branch.IsEnd ? "END" : "CONTINUE")));
        }

        public IDisposable BeginFlowScope(FlowContext flowContext)
        {
            return _logger.BeginScope(flowContext);
        }

        public void LogFlowException(FlowContext flowContext, IFlowStepRequest flowRequest, Exception ex)
        {
            _logger.LogError(flowContext, ex,
                "{ExceptionTypeName} occurred handling {RequestTypeName}({RequestSummary})",
                ex.GetType().Name, flowRequest.GetType().Name, _logger?.EvalIfError(() => GetPublicPropertySummary(flowRequest)));

            _logger.LogError("{ExceptionType}: {ExceptionMessage}", ex.GetType().Name, ex.Message);
            _logger.LogError("{ExceptionStackTrace}", ex.StackTrace);
        }

        public void LogFlowInnerException(FlowContext flowContext, Exception ex)
        {
            _logger.LogError(flowContext, ex, "Inner exception", ex.GetType().Name);

            _logger.LogError("{ExceptionType}: {ExceptionMessage}", ex.GetType().Name, ex.Message);
            _logger.LogError("{ExceptionStackTrace}", ex.StackTrace);
        }

        public void LogRequestOverrides(FlowContext flowContext, IFlowStepRequest request, List<Tuple<string, object, string>> overrides)
        {
            _logger.LogDebug(flowContext, "{RequestTypeName} overridden as {OverrideSummary}",
                request.GetType().Name, _logger?.EvalIfDebug(() => GetRequestOverrideSummary(overrides)));
        }

        public void LogLabel(FlowContext flowContext)
        {
            _logger.LogDebug(flowContext, "LABEL");
        }

        public void LogGoto(FlowContext flowContext, string nextStepName)
        {
            _logger.LogDebug(flowContext, nextStepName);
        }

        public void LogActivityRequestDisabled(FlowContext flowContext, IFlowStepRequest activityRequest)
        {
            _logger.LogDebug(flowContext, "Request disabled {ActivityRequestTypeName}({ActivityRequestSummary})",
                _logger.EvalIfDebug(() => activityRequest.GetType().Name),
                _logger.EvalIfDebug(() => GetPublicPropertySummary(activityRequest)));
        }

        #endregion

        #region Private members

        private static string GetPublicPropertySummary(object flowObj)
        {
            var flowRequestProperties = flowObj.GetType().GetFlowObjectType();

            object GetLogValue(FlowObjectProperty flowObjectPropertySummary)
            {
                var propertyValue = flowObjectPropertySummary.PropertyInfo.GetValue(flowObj);

                if (flowObjectPropertySummary.IsPrivate)
                {
                    return propertyValue == null ? "<null>" : "***";
                }

                if (propertyValue is IFlowValueDictionary flowValueDictionary)
                {
                    var itemSummaries = flowValueDictionary.Keys.Select(k => $"{k}={TrimmedLogValue(flowValueDictionary[k])}");
                    return $"[{string.Join(", ", itemSummaries)}]";
                }

                return TrimmedLogValue(propertyValue);
            }

            object TrimmedLogValue(object propertyValue)
            {
                const int maxLogValueLength = 128;
                var logValue = (propertyValue ?? "<null>").ToString();
                var trimmedLogValue =
                    logValue.Substring(0, Math.Min(logValue.Length, maxLogValueLength))
                    + ((logValue.Length > maxLogValueLength) ? "..." : "");
                return trimmedLogValue;
            }

            var propertyNameValueStrings =
                flowRequestProperties.Properties.Select(p => $"{p.PropertyInfo.Name}={GetLogValue(p)}");

            var propertySummary = string.Join(", ", propertyNameValueStrings);

            return propertySummary;
        }

        private static string GetRequestOverrideSummary(IEnumerable<Tuple<string, object, string>> overrides)
        {
            var bindingSummary = 
                string.Join(
                    ", ", overrides.Select(o => 
                        $"{o.Item1} => {o.Item2}[{o.Item3}]").ToArray());
            return bindingSummary;
        }

        #endregion
    }
}
