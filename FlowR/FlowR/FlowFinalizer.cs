using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FlowR
{
    public class FlowFinalizer<TRes>
    {
        internal List<FlowValueInputBinding> Inputs { get; } = new List<FlowValueInputBinding>();

        public FlowFinalizer<TRes> BindValue<TVal>(Expression<Func<TRes, TVal>> propertyExpression, string flowValueName)
        {
            AddInputBinding(propertyExpression, flowValueName, mapValue: (Func<object, TVal>)null);
            return this;
        }

        protected void AddInputBinding<TVal, TFlowVal>(
            Expression<Func<TRes, TVal>> propertyExpression, string flowValueName, Func<TFlowVal, TVal> mapValue)
        {
            var boundInputProperty = GetBoundInputProperty(propertyExpression);

            var binding = new FlowValueInputBinding(boundInputProperty)
            {
                FlowValueSelector = new FlowValueSingleSelector(flowValueName),
                MapValue = mapValue == null ? (Func<object, object>)null : v => mapValue((TFlowVal)v)
            };

            Inputs.Add(binding);
        }

        protected static FlowObjectProperty GetBoundInputProperty<TR, TV>(Expression<Func<TR, TV>> propertyExpression)
        {
            var propertyInfo = ReflectionUtils.GetProperty(propertyExpression);

            var flowObjectProperty = typeof(TR).GetFlowObjectType()[propertyInfo.Name];

            return flowObjectProperty;
        }
    }
}