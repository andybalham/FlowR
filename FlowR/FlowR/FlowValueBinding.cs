using System;
using System.Collections.Generic;
using System.Linq;

namespace FlowR
{
    public abstract class FlowValueBinding
    {
        protected FlowValueBinding(FlowObjectProperty property)
        {
            this.Property = property ?? throw new ArgumentNullException(nameof(property));
        }

        public FlowObjectProperty Property { get; set; }

        public FlowValueSelector FlowValueSelector { get; set; }

        public Func<object, object> MapValue { get; set; }

        public Func<string, object, string> MapName { get; set; }

        protected object GetMappedValue(object originalValue)
        {
            return this.MapValue == null
                ? originalValue
                : this.MapValue(originalValue);
        }

        protected string GetMappedName(string originalName, object request)
        {
            return (this.MapName == null)
                ? this.FlowValueSelector?.MapName == null
                    ? originalName
                    : this.FlowValueSelector?.MapName(originalName)
                : this.MapName(originalName, request);
        }

        public abstract string GetSummary(IFlowStepRequest request);
    }

    public class FlowValueInputBinding : FlowValueBinding
    {
        public FlowValueInputBinding(FlowObjectProperty property) : base(property)
        {
        }

        internal bool TryGetRequestValue(FlowValues flowValues, object request, out object requestValue)
        {
            requestValue = (object)null;

            var flowValueNames = this.FlowValueSelector.ResolveNames(request, flowValues);

            if (this.Property.IsDictionaryBinding)
            {
                var flowValueDictionary = (IFlowValueDictionary)Activator.CreateInstance(this.Property.DictionaryType);

                foreach (var flowValueName in flowValueNames)
                {
                    if (!flowValues.TryGetValue(flowValueName, out var flowValue)) continue;

                    var value = GetMappedValue(flowValue);
                    var name = GetMappedName(flowValueName, request);

                    flowValueDictionary.SetValue(name, value);
                }

                requestValue = flowValueDictionary;
            }
            else
            {
                var flowValueName = flowValueNames.First();

                if (flowValues.TryGetValue(flowValueName, out var flowValue))
                {
                    requestValue = GetMappedValue(flowValue);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public override string GetSummary(IFlowStepRequest request)
        {
            var selectorSummaries = this.FlowValueSelector.GetSummaries(request);

            if (this.Property.IsDictionaryBinding)
            {
                var summaries = new List<string>();

                foreach (var (sourceName, targetName) in selectorSummaries)
                {
                    var sourceValue = this.MapValue == null ? sourceName : $"Func({sourceName})";
                    var summary = targetName == null ? sourceValue : $"{targetName}: {sourceValue}";
                    summaries.Add(summary);
                }

                return $"{{ {string.Join(", ", summaries)} }}";
            }
            else
            {
                var flowValue = selectorSummaries.First().Item1;
                var requestValue = this.MapValue == null ? flowValue : $"Func({flowValue})";
                return requestValue;
            }
        }
    }

    public class FlowValueOutputBinding : FlowValueBinding
    {
        public FlowValueOutputBinding(FlowObjectProperty property) : base(property)
        {
        }

        internal IDictionary<string, object> GetOutputValues(object responseValue, object request)
        {
            var outputValues = new Dictionary<string, object>();

            if (this.Property.IsDictionaryBinding)
            {
                var responseValueDictionary = (IFlowValueDictionary)responseValue;

                var responseValueNames =
                    this.FlowValueSelector == null
                        ? responseValueDictionary.Keys
                        : this.FlowValueSelector.ResolveNames(request, responseValueDictionary);

                foreach (var responseValueName in responseValueNames)
                {
                    var outputName = GetMappedName(responseValueName, request);
                    var outputValue = GetMappedValue(responseValueDictionary[responseValueName]);

                    outputValues.Add(outputName, outputValue);
                }
            }
            else
            {
                var outputName = GetMappedName(this.Property.Name, request);
                var outputValue = GetMappedValue(responseValue);

                outputValues.Add(outputName, outputValue);
            }

            return outputValues;
        }

        public override string GetSummary(IFlowStepRequest request)
        {
            if (this.Property.IsDictionaryBinding)
            {
                var selectorSummaries = this.FlowValueSelector.GetSummaries(request);

                var summaries = new List<string>();

                foreach (var (sourceName, targetName) in selectorSummaries)
                {
                    var dictionaryValue = $"{this.Property.Name}[{sourceName}]";
                    var sourceValue = this.MapValue == null ? dictionaryValue : $"Func({dictionaryValue})";
                    var summary = targetName == null ? $"{sourceName}: {sourceValue}" : $"{targetName}: {sourceValue}";
                    summaries.Add(summary);
                }

                return $"{{ {string.Join(", ", summaries)} }}";
            }
            else
            {
                var flowValue = this.MapName == null ? this.Property.Name : this.MapName(null, request);
                var responseValue = this.MapValue == null ? this.Property.Name : $"Func({this.Property.Name})";
                return $"{flowValue}: {responseValue}";
            }
        }
    }
}