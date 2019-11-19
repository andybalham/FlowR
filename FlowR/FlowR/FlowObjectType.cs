using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace FlowR
{
    public class FlowObjectType
    {
        public FlowObjectType(Type type)
        {
            var publicInstanceProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var internalPropertyNames = new[]
                {
                        nameof(IFlowStepRequest.FlowContext),
                        nameof(DecisionFlowStepBase.Branches),
                        nameof(FlowResponse.CorrelationId),
                        nameof(FlowResponse.RequestId),
                        nameof(FlowResponse.FlowInstanceId),
                        nameof(FlowResponse.Trace)
                    };

            var externalProperties =
                Array.FindAll(publicInstanceProperties, p =>
                    !Regex.IsMatch(p.Name, $"({string.Join("|", internalPropertyNames)})"));

            var flowObjectProperties = new List<FlowObjectProperty>();

            foreach (var externalProperty in externalProperties)
            {
                var customAttributes = externalProperty.GetCustomAttributes(inherit: false);

                var flowObjectProperty = new FlowObjectProperty
                {
                    Name = externalProperty.Name,
                    PropertyInfo = externalProperty,
                    IsNotNullValue = customAttributes.Any(a => a is NotNullValueAttribute),
                    IsPrivate = customAttributes.Any(a => a is SensitiveValueAttribute),
                    IsDictionaryBinding =
                        externalProperty.PropertyType.IsGenericType
                        && externalProperty.PropertyType.GetGenericTypeDefinition() == typeof(FlowValueDictionary<>),
                    IsOverridableValue = customAttributes.Any(a => a is OverridableValueAttribute),
                    Description =
                        ((DescriptionAttribute)customAttributes.FirstOrDefault(a => a is DescriptionAttribute))?.Description,
                };

                if (flowObjectProperty.IsDictionaryBinding)
                {
                    flowObjectProperty.DictionaryValueType = externalProperty.PropertyType.GenericTypeArguments.First();
                    flowObjectProperty.DictionaryType =
                        typeof(FlowValueDictionary<>).MakeGenericType(flowObjectProperty.DictionaryValueType);
                }

                flowObjectProperties.Add(flowObjectProperty);
            }

            Properties = flowObjectProperties.ToArray();
        }

        public FlowObjectProperty[] Properties { get; }

        // TODO: Make this more efficient?
        public FlowObjectProperty this[string name] => this.Properties.First(s => s.Name == name);
    }
}
