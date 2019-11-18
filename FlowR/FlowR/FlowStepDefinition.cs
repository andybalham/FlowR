using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MediatR;

namespace FlowR
{
    public abstract class FlowStepDefinition
    {
        #region Public properties

        public virtual Type RequestType => null;
        public virtual Type ResponseType => null;

        #endregion

        #region Internal properties

        internal List<Tuple<PropertyInfo, object>> Setters { get; } = new List<Tuple<PropertyInfo, object>>();
        internal List<FlowValueInputBinding> Inputs { get; } = new List<FlowValueInputBinding>();
        internal List<FlowValueOutputBinding> Outputs { get; } = new List<FlowValueOutputBinding>();

        internal FlowValueInputBinding GetInputBinding(FlowObjectProperty flowObjectProperty)
        {
            var binding =
                this.Inputs.Find(i => i.Property.Name == flowObjectProperty.Name)
                ?? (flowObjectProperty.IsDictionaryBinding
                    ? new FlowValueInputBinding(flowObjectProperty)
                    {
                        FlowValueSelector = new FlowValueTypeSelector(flowObjectProperty.DictionaryValueType)
                    }
                    : new FlowValueInputBinding(flowObjectProperty)
                    {
                        FlowValueSelector = new FlowValueSingleSelector(flowObjectProperty.Name)
                    });

            return binding;
        }

        public FlowValueOutputBinding GetOutputBinding(FlowObjectProperty flowObjectProperty)
        {
            var binding =
                this.Outputs.Find(i => i.Property.Name == flowObjectProperty.Name)
                ?? new FlowValueOutputBinding(flowObjectProperty);

            return binding;
        }

        #endregion

        #region Protected methods

        protected void AddInputBinding<TReq, TVal, TFlowVal>(
            Expression<Func<TReq, TVal>> propertyExpression, string flowValueName, Func<TFlowVal, TVal> mapValue)
        {
            var boundInputProperty = GetBoundInputProperty(propertyExpression);

            var binding = new FlowValueInputBinding(boundInputProperty)
            {
                FlowValueSelector = new FlowValueSingleSelector(flowValueName),
                MapValue = mapValue == null ? (Func<object, object>)null : v => mapValue((TFlowVal)v)
            };

            Inputs.Add(binding);
        }

        protected void AddInputBinding<TReq, TVal, TFlowVal>(Expression<Func<TReq, FlowValueDictionary<TVal>>> propertyExpression,
            FlowValueSelector flowValueSelector, Func<TFlowVal, TVal> mapValue)
        {
            var boundInputProperty = GetBoundInputProperty(propertyExpression);

            var binding = new FlowValueInputBinding(boundInputProperty)
            {
                FlowValueSelector = flowValueSelector,
                MapValue = mapValue == null ? (Func<object, object>)null : v => mapValue((TFlowVal)v)
            };

            Inputs.Add(binding);
        }

        protected static PropertyInfo GetProperty<TR, TV>(Expression<Func<TR, TV>> propertyExpression)
        {
            var propertyExpressionMemberInfo = GetMemberInfo(propertyExpression.Body);

            if (propertyExpressionMemberInfo.MemberType != MemberTypes.Property)
            {
                throw new FlowException($"The member {propertyExpressionMemberInfo.Name} on type {typeof(TR).FullName} is not a property as is required.");
            }

            var propertyInfo = (PropertyInfo)propertyExpressionMemberInfo;

            return propertyInfo;
        }

        protected static FlowObjectProperty GetBoundInputProperty<TR, TV>(Expression<Func<TR, TV>> propertyExpression)
        {
            var propertyInfo = GetProperty(propertyExpression);

            var flowObjectProperty = typeof(TR).GetFlowObjectType()[propertyInfo.Name];

            if (flowObjectProperty.IsDesignTimeValue)
            {
                throw new FlowException(
                    $"The property {propertyInfo.Name} is annotated with {nameof(DesignTimeValueAttribute)}");
            }

            return flowObjectProperty;
        }

        protected void AddFlowValueBinding<TReq, TRes>(PropertyInfo setInputProperty)
        {
            var bindingNameAttribute = setInputProperty.GetCustomAttribute<BindingNameAttribute>();

            if (bindingNameAttribute == null) return;

            var boundValuePropertyName = GetBoundValuePropertyName(setInputProperty, bindingNameAttribute);

            switch (bindingNameAttribute)
            {
                case InputBindingNameAttribute _:
                    AddFlowValueInputBinding<TReq>(setInputProperty, boundValuePropertyName);
                    break;

                case OutputBindingNameAttribute _:
                    AddFlowValueOutputBinding<TRes>(setInputProperty, boundValuePropertyName);
                    break;
            }
        }

        #endregion

        #region Private methods

        private static string GetBoundValuePropertyName(PropertyInfo setInputProperty,
            BindingNameAttribute bindingNameAttribute)
        {
            string boundValuePropertyName;

            if (!String.IsNullOrEmpty(bindingNameAttribute.TargetProperty))
            {
                boundValuePropertyName = bindingNameAttribute.TargetProperty;
            }
            else
            {
                var targetNameMatch = Regex.Match(setInputProperty.Name, "^(?<name>.*)Name$");

                if (!targetNameMatch.Success)
                {
                    throw new FlowException($"The binding name property {setInputProperty.Name} does not end with 'Name'");
                }

                boundValuePropertyName = targetNameMatch.Groups["name"].Value;
            }

            return boundValuePropertyName;
        }

        private void AddFlowValueInputBinding<TReq>(PropertyInfo setInputProperty, string boundValuePropertyName)
        {
            var boundProperty = typeof(TReq).GetFlowObjectType()[boundValuePropertyName];

            if (boundProperty == null)
            {
                throw new FlowException(
                    $"The target for binding name property '{setInputProperty.Name}' could not be found: '{boundValuePropertyName}'");
            }

            if (!boundProperty.IsDesignTimeValue)
            {
                throw new FlowException(
                    $"The target for binding name property '{setInputProperty.Name}' is not a bound value: '{boundValuePropertyName}'");
            }

            Inputs.Add(new FlowValueInputBinding(boundProperty)
            {
                FlowValueSelector = new FlowValuePropertySelector(setInputProperty.Name, setInputProperty.GetValue)
            });
        }

        private void AddFlowValueOutputBinding<TRes>(PropertyInfo setInputProperty, string boundValuePropertyName)
        {
            if (typeof(TRes) == typeof(Unit))
            {
                throw new FlowException(
                    $"Property {setInputProperty.Name} was specified as an output binding name on a non-activity definition");
            }

            var flowObjectProperty = typeof(TRes).GetFlowObjectType()[boundValuePropertyName];

            if (flowObjectProperty == null)
            {
                throw new FlowException(
                    $"The target for binding name property '{setInputProperty.Name}' could not be found: '{boundValuePropertyName}'");
            }

            Outputs.Add(new FlowValueOutputBinding(flowObjectProperty)
            {
                MapName = (n, r) => (string)setInputProperty.GetValue(r)
            });
        }

        private static MemberInfo GetMemberInfo(Expression expression)
        {
            switch (expression)
            {
                case null:
                    throw new ArgumentNullException(nameof(expression));

                // Reference type property or field
                case MemberExpression memberExpression:
                    return memberExpression.Member;

                // Property, field of method returning value type
                case UnaryExpression unaryExpression:
                    return GetMemberInfo(unaryExpression);

                default:
                    throw new ArgumentException("Invalid property expression");
            }
        }

        private static MemberInfo GetMemberInfo(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MethodCallExpression methodExpression)
            {
                return methodExpression.Method;
            }

            return ((MemberExpression)unaryExpression.Operand).Member;
        }

        #endregion
    }

    public class FlowActivityDefinition<TReq, TRes> : FlowStepDefinition where TReq : FlowActivityRequest<TRes>
    {
        #region Public properties

        public override Type RequestType => typeof(TReq);
        public override Type ResponseType => typeof(TRes);

        #endregion

        #region Public methods

        public FlowActivityDefinition<TReq, TRes> SetValue<TVal>(Expression<Func<TReq, TVal>> propertyExpression, TVal value)
        {
            var requestSetInputProperty = GetProperty(propertyExpression);
            Setters.Add(new Tuple<PropertyInfo, object>(requestSetInputProperty, value));
            AddFlowValueBinding<TReq, TRes>(requestSetInputProperty);
            return this;
        }

        public FlowActivityDefinition<TReq, TRes> BindInput<TVal>(
            Expression<Func<TReq, TVal>> propertyExpression, string flowValueName)
        {
            AddInputBinding(propertyExpression, flowValueName, mapValue: (Func<object, TVal>)null);
            return this;
        }

        public FlowActivityDefinition<TReq, TRes> BindInput<TFlowVal, TVal>(
            Expression<Func<TReq, TVal>> propertyExpression, string flowValueName, Func<TFlowVal, TVal> mapValue)
        {
            AddInputBinding(propertyExpression, flowValueName, mapValue);
            return this;
        }

        public FlowActivityDefinition<TReq, TRes> BindInputs<TVal>(
            Expression<Func<TReq, FlowValueDictionary<TVal>>> propertyExpression, params string[] flowValueNames)
        {
            return BindInputs(propertyExpression, new FlowValueListSelector(flowValueNames), (Func<object, TVal>)null);
        }

        public FlowActivityDefinition<TReq, TRes> BindInputs<TVal>(
            Expression<Func<TReq, FlowValueDictionary<TVal>>> propertyExpression, IDictionary<string, string> nameMap)
        {
            return BindInputs(propertyExpression, new FlowValueListSelector(nameMap), (Func<object, TVal>)null);
        }

        public FlowActivityDefinition<TReq, TRes> BindInputs<TVal>(
            Expression<Func<TReq, FlowValueDictionary<TVal>>> propertyExpression, FlowValueSelector flowValueSelector)
        {
            return BindInputs(propertyExpression, flowValueSelector, (Func<object, TVal>)null);
        }

        public FlowActivityDefinition<TReq, TRes> BindInputs<TVal, TFlowVal>(
            Expression<Func<TReq, FlowValueDictionary<TVal>>> propertyExpression, FlowValueSelector flowValueSelector,
            Func<TFlowVal, TVal> mapValue)
        {
            AddInputBinding(propertyExpression, flowValueSelector, mapValue);
            return this;
        }

        public FlowActivityDefinition<TReq, TRes> BindOutput<TVal>(
            Expression<Func<TRes, TVal>> propertyExpression, string flowValueName, Func<TVal, object> mapValue = null)
        {
            var propertyInfo = GetProperty(propertyExpression);

            var boundOutputProperty = typeof(TRes).GetFlowObjectType()[propertyInfo.Name];

            var binding = new FlowValueOutputBinding(boundOutputProperty)
            {
                MapName = (n, r) => flowValueName
            };

            if (mapValue != null)
            {
                binding.MapValue = fv => mapValue((TVal)fv);
            }

            Outputs.Add(binding);

            return this;
        }

        public FlowActivityDefinition<TReq, TRes> BindOutputs<TVal>(Expression<Func<TRes, FlowValueDictionary<TVal>>> propertyExpression,
            params string[] outputValueNames)
        {
            return BindOutputs(propertyExpression, new FlowValueListSelector(outputValueNames));
        }

        public FlowActivityDefinition<TReq, TRes> BindOutputs<TVal>(Expression<Func<TRes, FlowValueDictionary<TVal>>> propertyExpression,
            IDictionary<string, string> nameMap)
        {
            return BindOutputs(propertyExpression, new FlowValueListSelector(nameMap));
        }

        public FlowActivityDefinition<TReq, TRes> BindOutputs<TVal>(Expression<Func<TRes, FlowValueDictionary<TVal>>> propertyExpression,
            FlowValueSelector flowValueSelector, Func<TVal, object> mapValue = null)
        {
            var propertyInfo = GetProperty(propertyExpression);

            var boundOutputProperty = typeof(TRes).GetFlowObjectType()[propertyInfo.Name];

            var binding = new FlowValueOutputBinding(boundOutputProperty)
            {
                FlowValueSelector = flowValueSelector
            };

            if (mapValue != null)
            {
                binding.MapValue = fv => mapValue((TVal)fv);
            }

            Outputs.Add(binding);

            return this;
        }

        #endregion
    }

    public abstract class FlowDecisionDefinitionBase : FlowStepDefinition
    {
        public abstract Type SwitchType { get; }
    }

    public class FlowDecisionDefinition<TReq, TSwitch> : FlowDecisionDefinitionBase where TReq : FlowDecision<TSwitch>
    {
        #region Public properties

        public override Type RequestType => typeof(TReq);

        public override Type SwitchType => typeof(TSwitch);

        #endregion

        #region Public methods

        public FlowDecisionDefinition<TReq, TSwitch> SetValue<TVal>(Expression<Func<TReq, TVal>> propertyExpression, TVal value)
        {
            var requestSetInputProperty = GetProperty(propertyExpression);

            Setters.Add(new Tuple<PropertyInfo, object>(requestSetInputProperty, value));

            AddFlowValueBinding<TReq, Unit>(requestSetInputProperty);

            return this;
        }

        public FlowDecisionDefinition<TReq, TSwitch> BindInput<TVal>(
            Expression<Func<TReq, TVal>> propertyExpression, string flowValueName)
        {
            AddInputBinding(propertyExpression, flowValueName, mapValue : (Func<object, TVal>)null);
            return this;
        }

        public FlowDecisionDefinition<TReq, TSwitch> BindInput<TVal, TFlowVal>(
            Expression<Func<TReq, TVal>> propertyExpression, string flowValueName, Func<TFlowVal, TVal> mapValue)
        {
            AddInputBinding(propertyExpression, flowValueName, mapValue);
            return this;
        }

        public FlowDecisionDefinition<TReq, TSwitch> BindInputs<TVal>(
            Expression<Func<TReq, FlowValueDictionary<TVal>>> propertyExpression, params string[] flowValueNames)
        {
            return BindInputs(propertyExpression, new FlowValueListSelector(flowValueNames), (Func<object, TVal>)null);
        }

        public FlowDecisionDefinition<TReq, TSwitch> BindInputs<TVal>(
            Expression<Func<TReq, FlowValueDictionary<TVal>>> propertyExpression, IDictionary<string, string> nameMap)
        {
            return BindInputs(propertyExpression, new FlowValueListSelector(nameMap), (Func<object, TVal>)null);
        }

        public FlowDecisionDefinition<TReq, TSwitch> BindInputs<TVal>(
            Expression<Func<TReq, FlowValueDictionary<TVal>>> propertyExpression, FlowValueSelector flowValueSelector)
        {
            return BindInputs(propertyExpression, flowValueSelector, (Func<object, TVal>)null);
        }

        public FlowDecisionDefinition<TReq, TSwitch> BindInputs<TVal, TFlowVal>(
            Expression<Func<TReq, FlowValueDictionary<TVal>>> propertyExpression, FlowValueSelector flowValueSelector,
            Func<TFlowVal, TVal> mapValue)
        {
            AddInputBinding(propertyExpression, flowValueSelector, mapValue);
            return this;
        }

        #endregion

        #region Private methods

        #endregion
    }
}
