using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FlowR.Discovery;

namespace FlowR
{
    // TODO: Think about making this class public

    internal class FlowDiagramBuilder
    {
        #region Member declarations

        private readonly IFlowOverrideProvider _overrideProvider;

        #endregion

        #region Constructors

        public FlowDiagramBuilder(IFlowOverrideProvider overrideProvider)
        {
            _overrideProvider = overrideProvider;
        }

        #endregion

        #region Public methods

        public FlowDiagram BuildDiagram(Type requestType, Type responseType, FlowDefinition flowDefinition, bool isOverride = false)
        {
            var requestInfo = new FlowRequestInfo
            {
                RequestType = requestType,
                Description = GetDescriptionAttributeValue(requestType),
                Properties = GetFlowPropertyInfos(requestType),
            };

            var responseInfo = new FlowResponseInfo
            {
                ResponseType = responseType,
                Description = GetDescriptionAttributeValue(responseType),
                Properties = GetFlowPropertyInfos(responseType),
            };

            var flowDiagram = new FlowDiagram(flowDefinition.Criteria, requestInfo, responseInfo, isOverride);

            var startNode = flowDiagram.AddNode(new FlowDiagramNode
            {
                NodeType = FlowDiagramNodeType.Start,
                Name = "Start"
            });

            var endCount = AddFlowDiagramDefaultLink(startNode, -1, flowDefinition, flowDiagram, 0);

            var previousNode = startNode;

            for (var stepIndex = 0; stepIndex < flowDefinition.Steps.Count; stepIndex++)
            {
                previousNode = AddFlowDiagramNode(stepIndex, flowDefinition, previousNode, flowDiagram, ref endCount);
            }

            return flowDiagram;
        }

        #endregion

        #region Private methods

        private string GetDescriptionAttributeValue(Type type)
        {
            var descriptionAttribute = type.GetCustomAttribute<DescriptionAttribute>();

            return descriptionAttribute?.Description;
        }

        private IEnumerable<FlowPropertyInfo> GetFlowPropertyInfos(Type type)
        {
            var flowObjectProperties = type.GetFlowObjectType();

            var flowPropertyInfos =
                flowObjectProperties.Properties.ToList().ConvertAll(p => new FlowPropertyInfo
                {
                    Name = p.PropertyInfo.Name,
                    PropertyType = p.PropertyInfo.PropertyType,
                    Description = p.Description,
                });

            return flowPropertyInfos;
        }

        private FlowDiagramNode AddFlowDiagramNode(int stepIndex, FlowDefinition flowDefinition,
            FlowDiagramNode previousNode, FlowDiagram flowDiagram, ref int endCount)
        {
            var flowStep = flowDefinition.Steps[stepIndex];

            switch (flowStep)
            {
                case ActivityFlowStep _:
                    var activityNode = AddFlowDiagramActivityNode(stepIndex, flowDefinition, flowDiagram, flowStep, ref endCount);
                    previousNode = activityNode;
                    break;

                case LabelFlowStep _:
                    var labelNode = AddFlowDiagramLabelNode(stepIndex, flowDefinition, flowDiagram, flowStep, ref endCount);
                    previousNode = labelNode;
                    break;

                case EndFlowStep _:
                    var endFlowNode = AddFlowDiagramEndNode(flowDiagram, ref endCount);
                    previousNode.Links.Add(new FlowDiagramLink { TargetNodeName = endFlowNode.Name });
                    break;

                case GotoFlowStep gotoFlowStep:
                    previousNode.Links.Add(new FlowDiagramLink { TargetNodeName = gotoFlowStep.NextStepName });
                    break;

                case DecisionFlowStepBase decisionFlowStep:
                    var decisionNode = AddFlowDiagramDecisionNode(stepIndex, flowDefinition, flowDiagram, flowStep, decisionFlowStep, ref endCount);
                    previousNode = decisionNode;
                    break;

                default:
                    throw new FlowException($"Unhandled flow step type: {flowStep.GetType().FullName}");
            }

            return previousNode;
        }

        private FlowDiagramNode AddFlowDiagramDecisionNode(int stepIndex, FlowDefinition flowDefinition, FlowDiagram flowDiagram,
            FlowStep flowStep, DecisionFlowStepBase decisionFlowStep, ref int endCount)
        {
            var decisionRequest = GetFlowStepRequest(flowStep.Definition);

            var decisionNode = flowDiagram.AddNode(new FlowDiagramNode
            {
                NodeType = FlowDiagramNodeType.Decision,
                Name = flowStep.Name,
                Text = GetFlowDiagramNodeText(flowStep, decisionRequest),
                OverrideKey = flowStep.OverrideKey?.Value,
                OverrideDescription = flowStep.OverrideKey?.Description,
            });

            SetFlowDiagramNodeInputSummaries(flowStep, decisionRequest, decisionNode);

            foreach (var branch in decisionFlowStep.Branches)
            {
                var linkCriteria = branch.Criteria?.ToList().ConvertAll(c => c?.ToString());

                if (branch.IsEnd)
                {
                    var endNode = AddFlowDiagramEndNode(flowDiagram, ref endCount);

                    decisionNode.Links.Add(new FlowDiagramLink
                    {
                        TargetNodeName = endNode.Name,
                        Criteria = linkCriteria
                    });
                }
                else
                {
                    if (String.IsNullOrEmpty(branch.NextStepName))
                    {
                        endCount = AddFlowDiagramDefaultLink(decisionNode, stepIndex, flowDefinition, flowDiagram, endCount);
                    }
                    else
                    {
                        decisionNode.Links.Add(new FlowDiagramLink
                        {
                            TargetNodeName = branch.NextStepName,
                            Criteria = linkCriteria
                        });
                    }
                }
            }

            return decisionNode;
        }

        private FlowDiagramNode AddFlowDiagramLabelNode(int stepIndex, FlowDefinition flowDefinition, FlowDiagram flowDiagram,
            FlowStep flowStep, ref int endCount)
        {
            var labelNode = flowDiagram.AddNode(new FlowDiagramNode
            {
                NodeType = FlowDiagramNodeType.Label,
                Name = flowStep.Name,
                Text = flowStep.Text ?? flowStep.Name
            });

            endCount = AddFlowDiagramDefaultLink(labelNode, stepIndex, flowDefinition, flowDiagram, endCount);
            return labelNode;
        }

        private FlowDiagramNode AddFlowDiagramActivityNode(int stepIndex, FlowDefinition flowDefinition, FlowDiagram flowDiagram,
            FlowStep flowStep, ref int endCount)
        {
            var activityRequest = GetFlowStepRequest(flowStep.Definition);

            var activityNode = flowDiagram.AddNode(new FlowDiagramNode
            {
                NodeType = FlowDiagramNodeType.Activity,
                Name = flowStep.Name,
                Text = GetFlowDiagramNodeText(flowStep, activityRequest),
                OverrideKey = flowStep.OverrideKey?.Value,
                OverrideDescription = flowStep.OverrideKey?.Description,
            });

            SetFlowDiagramNodeInputSummaries(flowStep, activityRequest, activityNode);

            activityNode.OutputBindings = GetBoundOutputSummary(flowStep, activityRequest);

            endCount = AddFlowDiagramDefaultLink(activityNode, stepIndex, flowDefinition, flowDiagram, endCount);

            return activityNode;
        }

        private static IFlowStepRequest GetFlowStepRequest(FlowStepDefinition flowStepDefinition, FlowContext flowContext = null)
        {
            var request = (IFlowStepRequest)Activator.CreateInstance(flowStepDefinition.RequestType);

            request.FlowContext = flowContext;

            // Design-time values

            foreach (var (requestPropertyInfo, requestPropertyValue) in flowStepDefinition.Setters)
            {
                requestPropertyInfo.SetValue(request, requestPropertyValue);
            }

            return request;
        }

        private static Dictionary<string, string> GetBoundOutputSummary(FlowStep flowStep, IFlowStepRequest request)
        {
            var responseType = flowStep.Definition.ResponseType.GetFlowObjectType();

            var boundOutputs = new Dictionary<string, string>();

            foreach (var responseProperty in responseType.Properties)
            {
                var binding = flowStep.Definition.GetOutputBinding(responseProperty);
                var summary = binding.GetSummary(request);

                boundOutputs[responseProperty.PropertyInfo.Name] = summary;
            }

            return boundOutputs;
        }

        private static string GetFlowDiagramNodeText(FlowStep flowStep, IFlowStepRequest flowStepRequest)
        {
            return flowStep.Text ?? flowStepRequest.GetText() ?? flowStep.Name;
        }

        private void SetFlowDiagramNodeInputSummaries(FlowStep flowStep, IFlowStepRequest request, FlowDiagramNode node)
        {
            var setInputs = new Dictionary<string, string>();
            var boundInputs = new Dictionary<string, string>();
            var inputOverrides = new Dictionary<string, IList<FlowInputOverride>>();

            var requestType = request.GetType().GetFlowObjectType();

            foreach (var requestProperty in requestType.Properties)
            {
                if (requestProperty.IsBoundValue)
                {
                    var binding = flowStep.Definition.GetInputBinding(requestProperty);
                    var summary = binding.GetSummary(request);

                    boundInputs[requestProperty.PropertyInfo.Name] = summary;
                }
                else
                {
                    setInputs[requestProperty.PropertyInfo.Name] = requestProperty.PropertyInfo.GetValue(request)?.ToString();
                }
            }

            if (!string.IsNullOrEmpty(flowStep.OverrideKey?.Value))
            {
                var flowRequestOverrides = _overrideProvider?.GetRequestOverrides(flowStep.OverrideKey?.Value);

                if (flowRequestOverrides != null)
                {
                    foreach (var flowRequestOverride in flowRequestOverrides)
                    {
                        var inputOverride = new FlowInputOverride
                        {
                            Value = flowRequestOverride.Value?.ToString(),
                            Criteria = flowRequestOverride.Criteria
                        };

                        if (inputOverrides.TryGetValue(flowRequestOverride.Name, out var propertyInputOverrides))
                        {
                            propertyInputOverrides.Add(inputOverride);
                        }
                        else
                        {
                            inputOverrides.Add(flowRequestOverride.Name, new List<FlowInputOverride>(new[] { inputOverride }));
                        }
                    }
                }
            }

            node.InputSetters = setInputs;
            node.InputBindings = boundInputs;
            node.InputOverrides = inputOverrides;
        }

        private int AddFlowDiagramDefaultLink(FlowDiagramNode node, int stepIndex, FlowDefinition flowDefinition,
            FlowDiagram flowDiagram, int endCount)
        {
            if (stepIndex + 1 == flowDefinition.Steps.Count)
            {
                var endNode = AddFlowDiagramEndNode(flowDiagram, ref endCount);
                node.Links.Add(new FlowDiagramLink { TargetNodeName = endNode.Name });
            }
            else
            {
                var nextStep = flowDefinition.Steps[stepIndex + 1];

                switch (nextStep)
                {
                    case GotoFlowStep _:
                    case EndFlowStep _:
                        // These steps add a link to the previous node, so we don't add one here
                        break;

                    default:
                        node.Links.Add(new FlowDiagramLink { TargetNodeName = nextStep.Name });
                        break;
                }
            }

            return endCount;
        }

        private static FlowDiagramNode AddFlowDiagramEndNode(FlowDiagram flowDiagram, ref int endCount)
        {
            endCount += 1;

            var endNode = flowDiagram.AddNode(new FlowDiagramNode
            {
                NodeType = FlowDiagramNodeType.End,
                Name = "End_" + endCount
            });

            return endNode;
        }

        #endregion
    }
}