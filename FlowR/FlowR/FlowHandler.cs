using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FlowR
{
    public abstract class FlowHandler<TFlowRequest, TFlowResponse> : IFlowHandler, IRequestHandler<TFlowRequest, TFlowResponse>
        where TFlowRequest : FlowActivityRequest<TFlowResponse>
        where TFlowResponse : FlowResponse
    {
        #region Member declarations

        private readonly IMediator _mediator;
        private readonly IFlowOverrideProvider _overrideProvider;
        private readonly MethodInfo _mediatorSend;
        private readonly MethodInfo _mediatorSendUnit;

        private readonly IFlowLoggerBase _logger;

        #endregion

        #region Constructors

        protected FlowHandler(IMediator mediator)
            : this(mediator, overrideProvider: null, logger: null)
        {
        }

        protected FlowHandler(IMediator mediator, IFlowLoggerBase logger = null)
        : this(mediator, overrideProvider: null, logger)
        {
        }

        protected FlowHandler(IMediator mediator, IFlowOverrideProvider overrideProvider = null, IFlowLoggerBase logger = null)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _overrideProvider = overrideProvider;
            _logger = logger;

            _mediatorSend = typeof(Mediator).GetMethod("Send");
            _mediatorSendUnit = _mediatorSend?.MakeGenericMethod(typeof(Unit));
        }

        #endregion

        #region IFlow

        public Type RequestType => typeof(TFlowRequest);

        public Type ResponseType => typeof(TFlowResponse);

        public abstract FlowDefinition GetFlowDefinition();

        #endregion

        #region IRequestHandler

        public async Task<TFlowResponse> Handle(TFlowRequest flowRequest, CancellationToken cancellationToken)
        {
            // TODO: Respect the cancellation token

            var flowContext = new FlowContext(typeof(TFlowRequest), flowRequest.FlowContext);

            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();

                using (flowContext.IsRootFlow ? _logger?.BeginFlowScope(flowContext) : null)
                {
                    _logger?.LogFlowRequest(flowContext, flowRequest);

                    var flowDefinition = ResolveFlowDefinition(flowRequest, flowContext);

                    var flowValues = GetInitialFlowValues(flowRequest);

                    var flowTrace = new FlowTrace();

                    var flowStepIndex = 0;

                    while (flowStepIndex < flowDefinition.Steps.Count)
                    {
                        var flowStep = flowDefinition.Steps[flowStepIndex];
                        var stepFlowContext = flowContext.GetStepContext(flowStep.Name);

                        flowStepIndex =
                            await PerformFlowStep(
                                stepFlowContext, flowDefinition, flowStep, flowStepIndex, flowValues, flowTrace, cancellationToken);
                    }

                    var flowResponse = BuildFlowResponse(flowContext, flowTrace, flowValues);

                    stopWatch.Stop();

                    _logger?.LogFlowResponse(flowContext, flowResponse, stopWatch.ElapsedMilliseconds);

                    return flowResponse;
                }
            }
            catch (Exception ex) when (LogFlowError(flowContext, flowRequest, ex))
            {
                throw;
            }
        }

        #endregion

        #region Private methods

        private FlowDefinition ResolveFlowDefinition(TFlowRequest flowRequest, FlowContext flowContext)
        {
            var flowDefinitionOverrides =
                _overrideProvider?.GetFlowDefinitionOverrides(typeof(TFlowRequest))?.ToList();

            var applicableFlowDefinitionOverride =
                _overrideProvider?.GetApplicableFlowDefinitionOverride(flowDefinitionOverrides, flowRequest);

            FlowDefinition flowDefinition;

            if (applicableFlowDefinitionOverride == null)
            {
                flowDefinition = GetFlowDefinition();
            }
            else
            {
                flowDefinition = applicableFlowDefinitionOverride;

                _logger?.LogFlowOverride(flowContext, flowRequest, applicableFlowDefinitionOverride.Criteria);
            }

            return flowDefinition;
        }

        private async Task<int> PerformFlowStep(FlowContext stepFlowContext, FlowDefinition flowDefinition, FlowStep flowStep,
            int flowStepIndex, FlowValues flowValues, FlowTrace flowTrace, CancellationToken cancellationToken)
        {
            int nextFlowStepIndex;

            switch (flowStep)
            {
                case ActivityFlowStep activityFlowStep:
                    await DoActivity(
                        activityFlowStep, stepFlowContext, flowValues,
                        flowTrace, cancellationToken);
                    nextFlowStepIndex = flowStepIndex + 1;
                    break;

                case DecisionFlowStepBase decisionFlowStep:
                    nextFlowStepIndex =
                        CheckDecision(
                            flowStepIndex, decisionFlowStep, flowDefinition, stepFlowContext, flowValues,
                            flowTrace);
                    break;

                case GotoFlowStep gotoFlowStep:
                    _logger.LogGoto(stepFlowContext, gotoFlowStep.NextStepName);
                    nextFlowStepIndex = flowDefinition.GetStepIndex(gotoFlowStep.NextStepName);
                    break;

                case LabelFlowStep _:
                    RecordLabelStep(flowTrace, stepFlowContext);
                    nextFlowStepIndex = flowStepIndex + 1;
                    break;

                case EndFlowStep _:
                    nextFlowStepIndex = int.MaxValue;
                    break;

                default:
                    throw new FlowException(
                        $"Unexpected flow activityFlowStep type {flowStep.GetType().FullName}");
            }

            return nextFlowStepIndex;
        }

        private void RecordLabelStep(FlowTrace flowTrace, FlowContext stepFlowContext)
        {
            _logger.LogLabel(stepFlowContext);

            flowTrace.AddStep(new FlowTraceStep { StepType = FlowTraceStepType.Label, Name = stepFlowContext.FlowStepName });
        }

        private async Task DoActivity(ActivityFlowStep activityFlowStep, FlowContext stepFlowContext,
            FlowValues flowValues, FlowTrace flowTrace, CancellationToken cancellationToken)
        {
            var activityRequest = CreateRequest(stepFlowContext, activityFlowStep, flowValues);

            if (IsRequestDisabled(activityRequest))
            {
                _logger?.LogActivityRequestDisabled(stepFlowContext, activityRequest);
                return;
            }

            _logger?.LogActivityRequest(stepFlowContext, activityRequest);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            object activityResponse = 
                await GetActivityResponse(
                    stepFlowContext, activityFlowStep, activityRequest, cancellationToken);

            stopWatch.Stop();

            ValidateFlowObjectValues(activityResponse, activityResponse.GetType().GetFlowObjectType(), activityFlowStep.Name);

            flowTrace.AddStep(new FlowTraceStep { StepType = FlowTraceStepType.Activity, Name = activityFlowStep.Name });

            _logger?.LogActivityResponse(stepFlowContext, activityResponse, stopWatch.ElapsedMilliseconds);

            SetFlowValuesFromResponse(activityFlowStep.Definition, stepFlowContext, flowValues, activityRequest, activityResponse);

            AppendResponseFlowTrace(flowTrace, activityResponse);
        }

        private static bool IsRequestDisabled(IFlowStepRequest flowStepRequest)
        {
            bool isRequestSkipped;
            switch (flowStepRequest)
            {
                case IFlowStepEnableable enableableStep when !enableableStep.IsEnabled:
                case IFlowStepDisableable disableableStep when disableableStep.IsDisabled:
                    isRequestSkipped = true;
                    break;

                default:
                    isRequestSkipped = false;
                    break;
            }

            return isRequestSkipped;
        }

        private async Task<dynamic> GetActivityResponse(FlowContext stepFlowContext, FlowStep flowStep, 
            IFlowStepRequest activityRequest, CancellationToken cancellationToken)
        {
            var mockHandler = 
                stepFlowContext.GetMockActivityHandler(
                    flowStep.Definition.RequestType, flowStep.OverrideKey?.Value);

            dynamic activityResponse;
            if (mockHandler == null)
            {
                var mediatorSendGeneric = _mediatorSend.MakeGenericMethod(flowStep.Definition.ResponseType);

                activityResponse =
                    await (dynamic)mediatorSendGeneric.Invoke(_mediator, new object[] { activityRequest, cancellationToken });
            }
            else
            {
                _logger?.LogMockRequestHandler(stepFlowContext, flowStep);

                activityResponse = mockHandler(activityRequest);
            }

            return activityResponse;
        }

        private int CheckDecision(int flowStepIndex, DecisionFlowStepBase decisionFlowStep, FlowDefinition flowDefinition, 
            FlowContext stepFlowContext, FlowValues flowValues, FlowTrace flowTrace)
        {
            var decisionRequest = (FlowDecisionBase)CreateRequest(stepFlowContext, decisionFlowStep, flowValues);

            // TODO: Pass the targets directly into GetMatchingBranchIndex
            decisionFlowStep.Branches.ForEach(b => decisionRequest.AddBranch(b.Targets));

            _logger?.LogDecisionRequest(stepFlowContext, decisionRequest);

            var branchIndex = decisionRequest.GetMatchingBranchIndex();

            if (branchIndex < 0 || branchIndex >= decisionFlowStep.Branches.Count)
            {
                throw new FlowException($"The branch index returned was out of bounds of the branch array: {branchIndex}");
            }

            var branch = decisionFlowStep.Branches[branchIndex];

            flowTrace.AddStep(new FlowTraceStep
            {
                StepType = FlowTraceStepType.Decision, Name = decisionFlowStep.Name, BranchTargets = branch.Targets
            });

            _logger?.LogDecisionResponse(stepFlowContext, branch);

            if (branch.IsEnd)
            {
                return int.MaxValue;
            }

            if (branch.IsException)
            {
                throw new FlowUnhandledElseException($"Unhandled ELSE for decision '{decisionFlowStep.Name}'");
            }

            var isContinue = (branch.NextStepName == null);
            if (isContinue)
            {
                return flowStepIndex + 1;
            }

            var nextFlowStepIndex = flowDefinition.GetStepIndex(branch.NextStepName);

            return nextFlowStepIndex;
        }

        private static void AppendResponseFlowTrace(FlowTrace flowTrace, dynamic activityResponse)
        {
            var subFlowTrace = activityResponse is FlowResponse flowStepResponse ? flowStepResponse.Trace : null;

            flowTrace.AddSubFlowTrace(subFlowTrace);
        }

        private void SetFlowValuesFromResponse(FlowStepDefinition flowStepDefinition, FlowContext flowContext, FlowValues flowValues,
            object request, object response)
        {
            var responseFlowObjectType = response.GetType().GetFlowObjectType();

            var bindingSummaries = new List<Tuple<string, string>>();

            foreach (var flowObjectProperty in responseFlowObjectType.Properties)
            {
                var responsePropertyValue = flowObjectProperty.PropertyInfo.GetValue(response);

                var outputBinding = flowStepDefinition.GetOutputBinding(flowObjectProperty);

                var outputValues = outputBinding.GetOutputValues(responsePropertyValue, request);

                foreach (var outputValue in outputValues)
                    flowValues.SetValue(outputValue.Key, outputValue.Value);
            }
        }

        private IFlowStepRequest CreateRequest(FlowContext flowContext, FlowStep flowStep, FlowValues flowValues)
        {
            var request = (IFlowStepRequest)Activator.CreateInstance(flowStep.Definition.RequestType);

            request.FlowContext = flowContext;

            var requestType = request.GetType().GetFlowObjectType();

            PopulateRequestSetValues(flowStep, request);

            PopulateRequestBoundValues(request, requestType, flowStep, flowValues);

            PopulateRequestOverriddenValues(flowContext, request, requestType, flowStep);

            ValidateFlowObjectValues(request, requestType, flowStep.Name);

            return request;
        }

        private static void PopulateRequestSetValues(FlowStep flowStep, IFlowStepRequest request)
        {
            foreach (var (requestPropertyInfo, requestPropertyValue) in flowStep.Definition.Setters)
            {
                requestPropertyInfo.SetValue(request, requestPropertyValue);
            }
        }

        private void PopulateRequestOverriddenValues(FlowContext flowContext,
            IFlowStepRequest request, FlowObjectType requestType, FlowStep flowStep)
        {
            var overrideKey = flowStep.OverrideKey?.Value;

            if ((_overrideProvider == null) || string.IsNullOrEmpty(overrideKey))
                return;

            var requestOverrides = _overrideProvider?.GetRequestOverrides(overrideKey)?.ToList();
            var applicableRequestOverrides =
                _overrideProvider?.GetApplicableRequestOverrides(requestOverrides, request);

            if (!(applicableRequestOverrides?.Count > 0))
                return;

            var overridableProperties = requestType.Properties.Where(p => p.IsOverridableValue);

            var overrides = new List<Tuple<string, object, string>>();

            foreach (var overridableProperty in overridableProperties)
            {
                var overridablePropertyName = overridableProperty.PropertyInfo.Name;

                if (!applicableRequestOverrides.TryGetValue(overridablePropertyName, out var overriddenValue))
                    continue;

                // TODO: Should we allow for type converters here?
                overridableProperty.PropertyInfo.SetValue(request, overriddenValue.Value);
                overrides.Add(new Tuple<string, object, string>(overridablePropertyName, overriddenValue.Value,
                    overriddenValue.Criteria));
            }

            _logger?.LogRequestOverrides(flowContext, request, overrides);
        }

        private static void PopulateRequestBoundValues(IFlowStepRequest request, 
            FlowObjectType requestType, FlowStep flowStep, FlowValues flowValues)
        {
            var boundProperties = requestType.Properties.Where(p => !p.IsDesignTimeValue);
            
            foreach (var boundProperty in boundProperties)
            {
                var inputBinding = flowStep.Definition.GetInputBinding(boundProperty);

                if (inputBinding.TryGetRequestValue(flowValues, request, out var requestValue))
                {
                    boundProperty.PropertyInfo.SetValue(request, requestValue);
                }
            }
        }

        private static void ValidateFlowObjectValues(object flowObject, FlowObjectType flowObjectType, string stepName)
        {
            var missingMandatoryPropertyNames = GetMissingMandatoryPropertyNames(flowObject, flowObjectType);

            if (missingMandatoryPropertyNames.Count > 0)
            {
                throw new FlowException(
                    "The following mandatory properties were not populated on the " +
                    $"{(flowObject is IFlowStepRequest ? "request" : "response")} for step {stepName}: " +
                    $"{string.Join(", ", missingMandatoryPropertyNames.ToArray())}");
            }
        }

        private static List<string> GetMissingMandatoryPropertyNames(object flowObject, FlowObjectType flowObjectType)
        {
            var missingMandatoryPropertyNames = new List<string>();

            foreach (var flowObjectProperty in flowObjectType.Properties)
            {
                CheckMandatoryFlowObjectProperty(flowObject, flowObjectProperty, missingMandatoryPropertyNames);
            }

            return missingMandatoryPropertyNames;
        }

        private static FlowValues GetInitialFlowValues(TFlowRequest flowRequest)
        {
            var flowValues = new FlowValues();

            var flowRequestType = typeof(TFlowRequest);
            var flowRequestProperties = flowRequestType.GetFlowObjectType();

            var missingMandatoryPropertyNames = new List<string>();

            foreach (var flowRequestProperty in flowRequestProperties.Properties)
            {
                CheckMandatoryFlowObjectProperty(flowRequest, flowRequestProperty, missingMandatoryPropertyNames);
                flowValues.SetValue(flowRequestProperty.PropertyInfo.Name, flowRequestProperty.PropertyInfo.GetValue(flowRequest));
            }

            if (missingMandatoryPropertyNames.Count > 0)
            {
                throw new FlowException(
                    $"The following mandatory properties were not populated on {flowRequestType.FullName}: " +
                    $"{string.Join(", ", missingMandatoryPropertyNames.ToArray())}");
            }

            return flowValues;
        }

        private static TFlowResponse BuildFlowResponse(FlowContext flowContext, FlowTrace flowTrace, FlowValues flowValues)
        {
            var flowResponseType = typeof(TFlowResponse);
            var flowResponse = (TFlowResponse)Activator.CreateInstance(flowResponseType);
            var flowObjectType = flowResponseType.GetFlowObjectType();

            var missingMandatoryPropertyNames = new List<string>();

            foreach (var flowObjectProperty in flowObjectType.Properties)
            {
                SetFlowResponseProperty(flowResponse, flowObjectProperty, flowValues);
                CheckMandatoryFlowObjectProperty(flowResponse, flowObjectProperty, missingMandatoryPropertyNames);
            }

            if (missingMandatoryPropertyNames.Count > 0)
            {
                throw new FlowException(
                    $"The following mandatory properties were not populated on {flowResponseType.FullName}: " +
                    $"{string.Join(", ", missingMandatoryPropertyNames.ToArray())}");
            }

            flowResponse.CorrelationId = flowContext.CorrelationId;
            flowResponse.RequestId = flowContext.RequestId;
            flowResponse.FlowInstanceId = flowContext.FlowInstanceId;
            flowResponse.Trace = flowTrace;

            return flowResponse;
        }

        private static void CheckMandatoryFlowObjectProperty(object flowObject, 
            FlowObjectProperty flowObjectProperty, ICollection<string> missingPropertyNames)
        {
            if (flowObjectProperty.IsNotNullValue && (flowObjectProperty.PropertyInfo.GetValue(flowObject) == null))
            {
                missingPropertyNames.Add(flowObjectProperty.PropertyInfo.Name);
            }
        }

        private static void SetFlowResponseProperty(TFlowResponse flowResponse,
            FlowObjectProperty flowResponseProperty, FlowValues flowValues)
        {
            if (flowValues.TryGetValue(flowResponseProperty.PropertyInfo.Name, out var flowValue))
            {
                flowResponseProperty.PropertyInfo.SetValue(flowResponse, flowValue);
            }
        }

        private bool LogFlowError(FlowContext flowContext, IFlowStepRequest flowRequest, Exception ex)
        {
            if (!flowContext.IsRootFlow) return true;

            _logger?.LogFlowException(flowContext, flowRequest, ex);

            var innerEx = ex.InnerException;

            while (innerEx != null)
            {
                _logger?.LogFlowInnerException(flowContext, innerEx);
                innerEx = innerEx.InnerException;
            }

            return true;
        }

        #endregion
    }
}
